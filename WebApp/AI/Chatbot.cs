using Azure.AI.OpenAI;

namespace WebApp.AI;

/*
The Chatbot.cs file implements the chatbot feature within the Point of Sale (POS) system for the Gong Cha restaurant. 
This file defines the Chatbot class responsible for interacting with the OpenAI GPT-3 API to facilitate conversation 
between users and the AI-powered assistant.
*/

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
        _options.Messages.Clear();
        _systemPrompt += $"\nUser: {query}\nAssistant: {response}";
        _options.Messages.Add(new ChatMessage(ChatRole.System, _systemPrompt));

        // Return response
        return response;
    }
}
