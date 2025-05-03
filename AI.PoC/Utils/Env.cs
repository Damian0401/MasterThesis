using Microsoft.Extensions.Configuration;

namespace Utils;

internal static class Env
{
    private static Config _config = new();
    public static Config Current => _config;
    public class Config
    {
        private static IConfigurationRoot? _config;

        public string this[string key] 
        {
            get
            {
                if (_config is null)
                {
                    _config = new ConfigurationBuilder()
                        .AddUserSecrets<Program>()
                        .Build();
                }

                return _config[key] ?? throw new InvalidOperationException($"Key {key} not found.");
            }
        }
    }
}
