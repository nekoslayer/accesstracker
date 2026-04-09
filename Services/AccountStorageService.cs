using System.Security.Cryptography;
using System.Text.Json;
using AccessTracker.Models;

namespace AccessTracker.Services;

public class AccountStorageService
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly object _lock = new();

    public AccountStorageService(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "accounts.json");
        SeedDefaultAdmin();
    }

    private void SeedDefaultAdmin()
    {
        if (File.Exists(_filePath))
            return;

        var defaultAdmin = new AdminAccount
        {
            Id = Guid.NewGuid(),
            Username = "ADMIN",
            PasswordHash = HashPassword("Pass123!!"),
            CreatedAt = DateTime.Now
        };

        var json = JsonSerializer.Serialize(new List<AdminAccount> { defaultAdmin }, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }

    public List<AdminAccount> GetAll()
    {
        lock (_lock)
        {
            if (!File.Exists(_filePath))
                return new List<AdminAccount>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<AdminAccount>>(json, _jsonOptions) ?? new List<AdminAccount>();
        }
    }

    public bool Validate(string username, string password)
    {
        var accounts = GetAll();
        var account = accounts.FirstOrDefault(a =>
            a.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        return account != null && VerifyPassword(password, account.PasswordHash);
    }

    public bool UsernameExists(string username)
    {
        return GetAll().Any(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public void Add(string username, string password)
    {
        lock (_lock)
        {
            var accounts = GetAll();
            accounts.Add(new AdminAccount
            {
                Id = Guid.NewGuid(),
                Username = username,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.Now
            });
            File.WriteAllText(_filePath, JsonSerializer.Serialize(accounts, _jsonOptions));
        }
    }

    public bool Delete(Guid id)
    {
        lock (_lock)
        {
            var accounts = GetAll();
            if (accounts.Count <= 1)
                return false; // refuse to delete last admin

            accounts.RemoveAll(a => a.Id == id);
            File.WriteAllText(_filePath, JsonSerializer.Serialize(accounts, _jsonOptions));
            return true;
        }
    }

    private static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string stored)
    {
        var parts = stored.Split(':');
        if (parts.Length != 2)
            return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] expected = Convert.FromBase64String(parts[1]);
        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, 100_000, HashAlgorithmName.SHA256, 32);

        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }
}
