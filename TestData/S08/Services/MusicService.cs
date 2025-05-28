using System.IO;
using System.Text.Json;

namespace MusicApp.Services;

public class MusicService
{
    public string GetTrackById(string id)
    {
        return $"SELECT * FROM Tracks WHERE Id = '{id}'";
    }

    public string SaveTrackFile(IFormFile file)
    {
        var path = Path.Combine("uploads", file.FileName);
        using var stream = new FileStream(path, FileMode.Create);
        file.CopyTo(stream);
        return "File uploaded";
    }

    public string DeserializeTrack(string data)
    {
        var obj = JsonSerializer.Deserialize<dynamic>(data);
        return obj?.ToString() ?? "Invalid data";
    }
}