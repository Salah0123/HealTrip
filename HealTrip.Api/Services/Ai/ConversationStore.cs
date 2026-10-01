#pragma warning disable OPENAI001


using OpenAI.Responses;
using System.Collections.Concurrent;

namespace HealTrip.Api.Services.Ai;

public class ConversationStore
{
    private readonly ConcurrentDictionary<
        string,
        List<ResponseItem>> _conversations = new();

    public List<ResponseItem> GetOrCreate(string conversationId)
    {
        return _conversations.GetOrAdd(
            conversationId,
            _ => new List<ResponseItem>());
    }
}