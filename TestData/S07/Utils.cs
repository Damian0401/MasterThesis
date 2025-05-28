using System.IO;
using System.Text.Json;

public static class Utils
{
    public static T UnsafeDeserialize<T>(string input)
    {
        return JsonSerializer.Deserialize<T>(input);
    }

    public static void WriteContentToFile(string filename, string content)
    {
        File.WriteAllText(filename, content);
    }
}
