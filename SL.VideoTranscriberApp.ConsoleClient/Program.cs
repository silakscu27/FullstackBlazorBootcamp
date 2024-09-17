using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using System;
using System.Resources;
using Xabe.FFmpeg;
using OpenAI;

var videoFilePath = "";

string audioFilePath = Path.ChangeExtension(videoFilePath, "mp3");

CancellationTokenSource cts = new CancellationTokenSource();

var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(videoFilePath, audioFilePath);

await conversion.Start(cts.Token);

const string fileName = "micro-machines.mp3";
var sampleFile = await File.ReadAllBytesAsync(audioFilePath);

var openAiService = new OpenAIService(new OpenAiOptions()
{
    ApiKey = "sk-",
});

var audioResult = await openAiService.Audio.CreateTranscription(new AudioCreateTranscriptionRequest
{
    FileName = fileName,
    File = sampleFile,
    Model = Models.WhisperV1,
    ResponseFormat = StaticValues.AudioStatics.ResponseFormat.Srt
});

var transcription = audioResult.Text;
var language = "turkish";

var url = "";
await File.WriteAllLinesAsync(url, transcription, cts.Token);

var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
{
    Messages = new List<ChatMessage>
    {
        ChatMessage.FromSystem($"You are a translator english to {language}. You've worked as a translator your whole life and you are very good at it. Don't include any extra explanations your answer. "),
        ChatMessage.FromUser($"Could you please translate the text given below to {language}. the text is a \".srt\" file. Therefore, don't change the time values.  \n {transcription}"),
    },
    Model = Models.Gpt_4o,
});
if (completionResult.Successful)
{
    var turkishTranscription = completionResult.Choices.First().Message.Content;
    Console.WriteLine(turkishTranscription);
    await File.WriteAllLinesAsync(url, turkishTranscription, cts.Token);
}