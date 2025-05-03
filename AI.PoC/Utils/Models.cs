using Microsoft.Extensions.AI;

namespace Utils;

internal static class Models
{
    private static Inference _inference = new();

    public static Inference AzureAiInference => _inference;

    public class Inference
    {
        private const string _endpoint = "https://models.inference.ai.azure.com";

        public IChatClient this[string model]
        {
            get
            {
                var accessToken = Env.Current[Consts.Environments.GithubToken];
                var credential = new Azure.AzureKeyCredential(accessToken);
                return new Azure.AI.Inference.ChatCompletionsClient(
                        new Uri(_endpoint),
                        credential)
                    .AsChatClient(model);
            }
        }
    }
}