using Microsoft.Extensions.AI;
using Utils;

var model = Consts.GitHubModels.MetaLlama3_70BInstruct;
var client = Models.AzureAiInference[model];

var systemMessage = new ChatMessage(ChatRole.System, Consts.SystemMessage);
var inputFileName = "Program.cs";
var fullInputFilePath = Path.Combine("Examples", inputFileName);
var codeMessage = new ChatMessage(ChatRole.User, File.ReadAllText(fullInputFilePath));
var response = await client.CompleteAsync([systemMessage, codeMessage]);

var outputFileName = Helper.SanitizeFileName($"result_{inputFileName}_{model}_{DateTime.Now.ToString("dd-MM-yyyy_HHmmss")}.txt");
var fullOutputFileName = Path.Combine("Results", outputFileName);
await File.WriteAllTextAsync(fullOutputFileName, response.Message.Text);
