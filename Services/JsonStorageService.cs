using System.Text.Json;
using AccessTracker.Models;

namespace AccessTracker.Services;

public class JsonStorageService
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
    private readonly object _lock = new();

    public JsonStorageService(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "submissions.json");
    }

    public List<Submission> GetAll()
    {
        lock (_lock)
        {
            if (!File.Exists(_filePath))
                return new List<Submission>();

            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<Submission>();

            return JsonSerializer.Deserialize<List<Submission>>(json, _options) ?? new List<Submission>();
        }
    }

    public Submission? GetById(Guid id)
    {
        return GetAll().FirstOrDefault(s => s.Id == id);
    }

    public void Add(Submission submission)
    {
        lock (_lock)
        {
            var list = GetAll();
            list.Add(submission);
            var json = JsonSerializer.Serialize(list, _options);
            File.WriteAllText(_filePath, json);
        }
    }
}
