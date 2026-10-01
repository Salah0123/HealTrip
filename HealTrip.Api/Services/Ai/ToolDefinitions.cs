using OpenAI.Responses;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

#pragma warning disable OPENAI001
namespace HealTrip.Api.Services.Ai;

public static class ToolDefinitions
{
    [SuppressMessage("Evaluation", "OPENAI001")]
    public static FunctionTool CreateSearchDoctorsTool()
    {
        return ResponseTool.CreateFunctionTool(
            functionName: "search_doctors",
            functionDescription:
                "Search for doctors that actually exist in the HealTrip database. " +
                "Use this tool when the user asks to find doctors or specialists.",
            functionParameters: BinaryData.FromBytes("""
            {
              "type": "object",
              "properties": {
                "specialty": {
                  "type": "string",
                  "description": "Medical specialty, for example Cardiology."
                },
                "city": {
                  "type": "string",
                  "description": "City where the doctor should be located."
                }
              },
              "required": []
            }
            """u8.ToArray()),
            strictModeEnabled: false
        );
    }
    public static FunctionTool CreateSearchHospitalsTool()
    {
        return ResponseTool.CreateFunctionTool(
            functionName: "search_hospitals",
            functionDescription:
                "Search hospitals that actually exist in the HealTrip database. " +
                "Use this when the user asks for hospitals, emergency facilities, " +
                "or hospitals with a specific medical specialty.",
            functionParameters: BinaryData.FromBytes("""
        {
          "type": "object",
          "properties": {
            "city": {
              "type": "string",
              "description": "City where the hospital should be located."
            },
            "specialty": {
              "type": "string",
              "description": "Medical specialty available at the hospital."
            },
            "hasEmergency": {
              "type": "boolean",
              "description": "Whether the hospital should have emergency services."
            }
          },
          "required": []
        }
        """u8.ToArray()),
            strictModeEnabled: false
        );
    }
}