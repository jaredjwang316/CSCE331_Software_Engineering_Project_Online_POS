using Microsoft.CognitiveServices.Speech;

namespace WebApp.AI;
public class TTS {
    public TTS() {}

    public async Task Run(string text) {
        var speechConfig = SpeechConfig.FromSubscription(
            Config.AZURE_SPEECH_KEY,
            Config.AZURE_SPEECH_REGION
        );

        speechConfig.SpeechSynthesisVoiceName = "en-US-JennyMultilingualV2Neural";

        using var synthesizer = new SpeechSynthesizer(speechConfig);

        var result = await synthesizer.SpeakTextAsync(text);
        OutputSpeechSythensisResult(result, text);
    }

    static void OutputSpeechSythensisResult(SpeechSynthesisResult speechSynthesisResult, string text)
        {
            switch (speechSynthesisResult.Reason)
            {
                case ResultReason.SynthesizingAudioCompleted:
                    Console.WriteLine($"Speech synthesized to speaker for text [{text}]");
                    break;
                case ResultReason.Canceled:
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        Console.WriteLine($"CANCELED: Are the environment variables set correctly?");
                    }
                    break;
                default:
                    break;
            }
        }
}
