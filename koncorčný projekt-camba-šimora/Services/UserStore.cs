using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace koncorèný_projekt_camba_šimora.Services
{
    public class UserRecord
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Salt { get; set; } = "";
    }

    public class UserStore
    {
        private readonly string _dataDir;
        private readonly string _userFile;
        private readonly string _rememberFile;

        public UserStore()
        {
            _dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TodoListApp");
            Directory.CreateDirectory(_dataDir);
            _userFile = Path.Combine(_dataDir, "users.json");
            _rememberFile = Path.Combine(_dataDir, "remember.json");
        }

        private List<UserRecord> LoadUsers()
        {
            if (!File.Exists(_userFile))
                return new List<UserRecord>();

            var json = File.ReadAllText(_userFile);
            try
            {
                return JsonSerializer.Deserialize<List<UserRecord>>(json) ?? new List<UserRecord>();
            }
            catch
            {
                return new List<UserRecord>();
            }
        }

        private void SaveUsers(List<UserRecord> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_userFile, json);
        }

        public bool CreateUser(string name, string email, string password, out string error)
        {
            error = "";
            email = email.Trim().ToLowerInvariant();
            var users = LoadUsers();

            if (users.Any(u => u.Email == email))
            {
                error = "An account with that email already exists.";
                return false;
            }

            var salt = CreateSalt();
            var hash = HashPassword(password, salt);

            users.Add(new UserRecord
            {
                Name = name,
                Email = email,
                PasswordHash = Convert.ToBase64String(hash),
                Salt = Convert.ToBase64String(salt)
            });

            SaveUsers(users);
            return true;
        }

        public bool Authenticate(string email, string password, out string userName)
        {
            userName = "";
            email = email.Trim().ToLowerInvariant();
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => u.Email == email);
            if (user == null) return false;

            var salt = Convert.FromBase64String(user.Salt);
            var expected = Convert.FromBase64String(user.PasswordHash);
            var actual = HashPassword(password, salt);

            var ok = CryptographicOperations.FixedTimeEquals(expected, actual);
            if (ok) userName = user.Name;
            return ok;
        }

        private static byte[] CreateSalt(int size = 16)
        {
            var salt = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }

        private static byte[] HashPassword(string password, byte[] salt, int iterations = 100_000, int length = 32)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(length);
        }

        // Remember-me helpers (stores last email in a small json)
        public void RememberEmail(string? email)
        {
            if (string.IsNullOrEmpty(email))
            {
                if (File.Exists(_rememberFile)) File.Delete(_rememberFile);
                return;
            }

            var obj = new { email = email.Trim().ToLowerInvariant() };
            File.WriteAllText(_rememberFile, JsonSerializer.Serialize(obj));
        }

        public string? GetRememberedEmail()
        {
            if (!File.Exists(_rememberFile)) return null;
            try
            {
                var json = File.ReadAllText(_rememberFile);
                var doc = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (doc != null && doc.TryGetValue("email", out var e)) return e;
            }
            catch { }
            return null;
        }
    }
}