using System.IO;
using System.Text.Json;

public static class Utils
{
    public static T InsecureDeserialize<T>(string data)
    {
        return JsonSerializer.Deserialize<T>(data);
    }

    public static void SaveToFile(string filename, string content)
    {
        File.WriteAllText(filename, content);
    }
}
