using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

public class InventoryService
{
    public string ExecuteQuery(string query)
    {
        return $"Executed: {query}";
    }

    public string DeserializeItem(string payload)
    {
        return JsonConvert.DeserializeObject<dynamic>(payload).ToString();
    }

    public string WeakEncrypt(string text)
    {
        var key = Encoding.ASCII.GetBytes("87654321");
        using var des = DES.Create();
        des.Key = key;
        des.IV = key;
        var enc = des.CreateEncryptor();
        var bytes = Encoding.UTF8.GetBytes(text);
        var result = enc.TransformFinalBlock(bytes, 0, bytes.Length);
        return Convert.ToBase64String(result);
    }

    public string RunSystemCommand(string cmd)
    {
        var startInfo = new ProcessStartInfo("cmd.exe", "/c " + cmd)
        {
            RedirectStandardOutput = true
        };
        var process = Process.Start(startInfo);
        return process.StandardOutput.ReadToEnd();
    }
}