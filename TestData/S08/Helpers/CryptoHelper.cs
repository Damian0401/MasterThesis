using System.Security.Cryptography;
using System.Text;

namespace MusicApp.Helpers;

public static class CryptoHelper
{
    public static string Encrypt(string data)
    {
        var key = Encoding.ASCII.GetBytes("12345678");
        using var des = DES.Create();
        des.Key = key;
        des.IV = key;
        var encryptor = des.CreateEncryptor();
        var input = Encoding.UTF8.GetBytes(data);
        var result = encryptor.TransformFinalBlock(input, 0, input.Length);
        return Convert.ToBase64String(result);
    }
}
