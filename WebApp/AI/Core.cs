using Azure;
using Azure.AI.OpenAI;

namespace WebApp.AI;
public class Core {
    protected string _systemPrompt;
    protected readonly OpenAIClient _client;
    protected ChatCompletionsOptions _options { get; }

    protected Core(string systemPrompt, int maxTokens, float temperature) {
        _systemPrompt = systemPrompt;
        _client = new OpenAIClient(Config.OPENAI_API_KEY);
        _options = new ChatCompletionsOptions {
            Messages = { new ChatMessage(ChatRole.System, _systemPrompt) },
            MaxTokens = maxTokens,
            Temperature = temperature,
            DeploymentName = "gpt-3.5-turbo"
        };
    }

    //==========================//
    // Core methods (protected) //
    //==========================//
    protected async Task<string> Run(
        string query,
        int timeoutMillisecodns,
        int retryDelayMilliseconds,
        int maxRetries,
        Action<string>? callback = null
    ) {
        if (string.IsNullOrEmpty(query)) {
            throw new ArgumentException("Query cannot be null or empty", nameof(query));
        }

        // Save query to memory (required to generate response)
        _options.Messages.Add(new ChatMessage(ChatRole.User, query));

        int currentRetry = 0;
        while (currentRetry <= maxRetries) {
            try {
                // Create response task
                var responseTask = _client.GetChatCompletionsStreamingAsync(
                    _options
                );

                // Create a cancellation token to enforce timeout
                using var cts = new CancellationTokenSource();

                // Combine the original task with the timeout task
                var completedTask = await Task.WhenAny(
                    responseTask,
                    Task.Delay(timeoutMillisecodns, cts.Token)
                );

                if (completedTask == responseTask) {
                    // The response task was successful
                    var response = responseTask.Result;
                    string fullResponse = "";

                    // Stream response to callback funciton if provided
                    await foreach (StreamingChatCompletionsUpdate update in response) {
                        if (update.ContentUpdate != null) {
                            fullResponse += update.ContentUpdate.ToString();
                            callback?.Invoke(update.ContentUpdate.ToString());
                        }
                    }

                    // Return full response
                    _options.Messages.Add(new ChatMessage(ChatRole.Assistant, fullResponse));
                    return fullResponse;
                } else {
                    // The response task timed out
                    ++currentRetry;
                    Backoff(retryDelayMilliseconds, currentRetry);
                }
            } catch (TaskCanceledException tce) when (tce.CancellationToken.IsCancellationRequested) {
                // The response task timed out and threw a TaskCanceledException
                ++currentRetry;
                Backoff(retryDelayMilliseconds, currentRetry);
            } catch (Exception exc) {
                // Unknown exception
                _options.Messages.Add(new ChatMessage(ChatRole.Assistant, exc.Message));
                callback?.Invoke(exc.Message);
                return exc.Message;
            }
        }

        // Operation timed out and failed all retries
        _options.Messages.Add(new ChatMessage(ChatRole.Assistant, "Operation timed out."));
        callback?.Invoke("Operation timed out.");
        return "Operation timed out.";
    }

    //==========================//
    //  Core methods (private)  //
    //==========================//
    private protected static async void Backoff(int retryDelayMilliseconds, int currentRetry)
    {
        int backoff = (int)((Math.Pow(2, currentRetry) - 1) * retryDelayMilliseconds);
        await Task.Delay(backoff);
    }
}
