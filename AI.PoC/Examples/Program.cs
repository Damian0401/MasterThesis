using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Enter username:");
        string username = Console.ReadLine();

        Console.WriteLine("Enter password:");
        string password = Console.ReadLine();

        string connectionString = "Server=myServerAddress;Database=myDataBase;User Id=admin;Password=12345;";

        string query = "SELECT * FROM Users WHERE Username = '" + username + "' AND Password = '" + password + "';";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                Console.WriteLine("Login successful!");
            }
            else
            {
                Console.WriteLine("Login failed.");
            }
        }

        Console.WriteLine("Enter file name to save log:");
        string fileName = Console.ReadLine();
        File.WriteAllText(fileName, "User logged in: " + username);

        string hardcodedKey = "1234567890123456";
        byte[] data = Encoding.UTF8.GetBytes("Sensitive data");
        byte[] encryptedData = EncryptData(data, hardcodedKey);
        Console.WriteLine("Encrypted Data: " + Convert.ToBase64String(encryptedData));

        WebClient client = new WebClient();
        string response = client.DownloadString("http://example.com/api?query=" + username);
        Console.WriteLine("Response: " + response);
    }

    private static byte[] EncryptData(byte[] data, string key)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= (byte)key[i % key.Length];
        }
        return data;
    }

    public static Task<string> FetchDataAsync(string url)
    {
        WebClient client = new WebClient();
        return Task.FromResult(client.DownloadString(url));
    }
}
