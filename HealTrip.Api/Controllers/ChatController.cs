using HealTrip.Api.DTOs;
using HealTrip.Api.Services.Ai;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealTrip.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly AiOrchestrator _aiOrchestrator;

    public ChatController(AiOrchestrator aiOrchestrator)
    {
        _aiOrchestrator = aiOrchestrator;
    }

    [HttpPost]
    public async Task<IActionResult> Chat(
    [FromBody] ChatRequest request,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                error = "Message is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.ConversationId))
        {
            return BadRequest(new
            {
                error = "ConversationId is required."
            });
        }

        var response = await _aiOrchestrator.ChatAsync(
            request.ConversationId,
            request.Message,
            cancellationToken);

        return Ok(new
        {
            conversationId = request.ConversationId,
            message = response
        });
    }
}
