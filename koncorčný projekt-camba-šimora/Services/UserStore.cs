using System;
using System.IO;
using System.Linq;

namespace koncorèný_projekt_camba_šimora.Services
{
    public class UserStore
    {
        // Súbory sa uložia ved¾a .exe súboru
        private readonly string _usersFile = "users.txt";
        private readonly string _rememberFile = "remember.txt";

        // ??? Registrácia ???????????????????????????????????????????????
        public bool CreateUser(string name, string email, string password, out string error)
        {
            email = email.ToLower();

            // Skontroluj, èi email už existuje
            if (File.Exists(_usersFile))
            {
                var existing = File.ReadAllLines(_usersFile)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(l => l.Split('|'))
                    .Any(p => p.Length >= 2 && p[1].Equals(email, StringComparison.OrdinalIgnoreCase));

                if (existing)
                {
                    error = "An account with this email already exists.";
                    return false;
                }
            }

            // Ulož:  meno|email|heslo
            File.AppendAllText(_usersFile, $"{name}|{email}|{password}{Environment.NewLine}");
            error = string.Empty;
            return true;
        }

        // ??? Prihlásenie ???????????????????????????????????????????????
        public bool Authenticate(string email, string password, out string userName)
        {
            userName = string.Empty;
            email = email.ToLower();

            if (!File.Exists(_usersFile))
                return false;

            foreach (var line in File.ReadAllLines(_usersFile))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split('|');
                if (parts.Length < 3) continue;

                string storedEmail = parts[1].Trim().ToLower();
                string storedPassword = parts[2].Trim();

                if (storedEmail == email && storedPassword == password)
                {
                    userName = parts[0].Trim(); // meno používate¾a
                    return true;
                }
            }

            return false;
        }

        // ??? Zapamätanie emailu ????????????????????????????????????????
        public void RememberEmail(string? email)
        {
            if (string.IsNullOrEmpty(email))
            {
                if (File.Exists(_rememberFile))
                    File.Delete(_rememberFile);
            }
            else
            {
                File.WriteAllText(_rememberFile, email);
            }
        }

        public string GetRememberedEmail()
        {
            if (!File.Exists(_rememberFile))
                return string.Empty;

            return File.ReadAllText(_rememberFile).Trim();
        }
    }
}