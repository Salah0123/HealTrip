#pragma warning disable OPENAI001
using HealTrip.Api.Options;
using HealTrip.Api.Services.Ai.Tools;
using Microsoft.Extensions.Options;
using OpenAI.Responses;

namespace HealTrip.Api.Services.Ai;

public class AiOrchestrator
{
    private readonly ResponsesClient _client;
    private readonly DoctorSearchTool _doctorSearchTool;
    private readonly HospitalSearchTool _hospitalSearchTool;
    private readonly OpenAiOptions _options;
    private readonly ConversationStore _conversationStore;
    private readonly SafetyTriageService _safetyTriageService;

    public AiOrchestrator(
        DoctorSearchTool doctorSearchTool,
        HospitalSearchTool hospitalSearchTool,
        IOptions<OpenAiOptions> options,
        ConversationStore conversationStore,
        SafetyTriageService safetyTriageService)
    {
        _doctorSearchTool = doctorSearchTool;
        _hospitalSearchTool = hospitalSearchTool;
        _options = options.Value;
        _conversationStore = conversationStore;
        _safetyTriageService = safetyTriageService;

        _client = new ResponsesClient(_options.ApiKey);
    }

    public async Task<string> ChatAsync(
        string conversationId,
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        if (_safetyTriageService.MayRequireUrgentCare(userMessage))
        {
            var isArabic = userMessage.Any(c => c >= 0x0600 && c <= 0x06FF);

            return isArabic
                ? """
          ألم شديد في الصدر مع صعوبة في التنفس قد يتطلب تقييمًا طبيًا عاجلًا.

          يرجى طلب الرعاية الطبية الطارئة الآن أو الاتصال برقم الطوارئ المحلي.

          لا أستطيع تشخيص سبب هذه الأعراض.
          """
                : """
          Chest pain with difficulty breathing can require urgent medical evaluation.

          Please seek emergency medical care now or contact your local emergency service.

          I cannot diagnose the cause of these symptoms.
          """;
        }
        var inputItems =
            _conversationStore.GetOrCreate(conversationId);

        inputItems.Add(
            ResponseItem.CreateUserMessageItem(userMessage));

        var tools = new List<FunctionTool>
        {
            ToolDefinitions.CreateSearchDoctorsTool(),
            ToolDefinitions.CreateSearchHospitalsTool()
        };

        while (true)
        {
            var options = new CreateResponseOptions
            {
                Model = _options.Model,
                Instructions = AiSystemPrompt.Instructions
            };

            foreach (var item in inputItems)
            {
                options.InputItems.Add(item);
            }

            foreach (var tool in tools)
            {
                options.Tools.Add(tool);
            }

            var response = await _client.CreateResponseAsync(
                options,
                cancellationToken);

            inputItems.AddRange(response.Value.OutputItems);

            var functionCallFound = false;

            foreach (var outputItem in response.Value.OutputItems)
            {
                if (outputItem is not FunctionCallResponseItem functionCall)
                    continue;

                functionCallFound = true;

                if (functionCall.FunctionName == "search_doctors")
                {
                    var result = await _doctorSearchTool.ExecuteAsync(
                        functionCall.FunctionArguments.ToString(),
                        cancellationToken);

                    inputItems.Add(
                        new FunctionCallOutputResponseItem(
                            functionCall.CallId,
                            result));
                }
                if (functionCall.FunctionName == "search_hospitals")
                {
                    var result = await _hospitalSearchTool.ExecuteAsync(
                        functionCall.FunctionArguments.ToString(),
                        cancellationToken);

                    inputItems.Add(
                        new FunctionCallOutputResponseItem(
                            functionCall.CallId,
                            result));
                }
            }

            if (!functionCallFound)
            {
                return response.Value.GetOutputText();
            }
        }
    }
}
