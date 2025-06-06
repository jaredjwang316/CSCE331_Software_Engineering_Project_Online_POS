using Azure.AI.OpenAI;

namespace WebApp.AI;
public class Chatbot : Core {
    public Chatbot(
        string? systemPrompt = null,
        int maxTokens = 400, float temperature = 0.9f
    ) : base(systemPrompt ?? "", maxTokens, temperature) {}

    public new async Task<string> Run(
        string query,
        int timeoutMillisecodns = 10000,
        int retryDelayMilliseconds = 1000,
        int maxRetries = 3,
        Action<string>? callback = null
    ) {
        string response = await base.Run(
            query,
            timeoutMillisecodns,
            retryDelayMilliseconds,
            maxRetries,
            callback
        );

        // Clear memory and update system prompt (this is to prevent the AI from deviating from the system prompt)
        // _options.Messages.Clear();
        // _systemPrompt += $"\nUser: {query}\nAssistant: {response}";
        // _options.Messages.Add(new ChatMessage(ChatRole.System, _systemPrompt));

        // Return response
        return response;
    }

    public void AddHistory(string message) {
        if (message == "") return;

        List<string> messages = message.Split("\n").ToList();
        foreach (string msg in messages) {
            string[]  parts = msg.Split(": ");
            if (parts.Length != 2) continue;
            string role = parts[0];
            string text = parts[1];

            if (role == "User") {
                _options.Messages.Add(new ChatMessage(ChatRole.User, text));
            } else if (role == "Assistant") {
                _options.Messages.Add(new ChatMessage(ChatRole.Assistant, text));
            }
        }
    }
}
