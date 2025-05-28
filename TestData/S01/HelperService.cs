using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class HelperService
{
    public string RunQuery(string query)
    {
        return $"Executed: {query}";
    }

    public string DeserializeData(string input)
    {
        return JsonSerializer.Deserialize<dynamic>(input);
    }

    public string EncryptData(string data)
    {
        var key = Encoding.UTF8.GetBytes("12345678");
        var des = DES.Create();
        des.Key = key;
        des.IV = key;
        var encryptor = des.CreateEncryptor();
        var bytes = Encoding.UTF8.GetBytes(data);
        var result = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
        return Convert.ToBase64String(result);
    }

    public string ExecuteCommand(string command)
    {
        var proc = new ProcessStartInfo("cmd.exe", "/c " + command)
        {
            RedirectStandardOutput = true
        };
        var process = Process.Start(proc);
        return process.StandardOutput.ReadToEnd();
    }
}