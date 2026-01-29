using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using YukiManager.Models;

namespace YukiManager.Services;

public class VaultService
{
    private static readonly string VaultPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "YukiManager",
        "vault.dat"
    );

    public Vault CurrentVault { get; private set; }
    private string _masterPassword;

    public VaultService()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(VaultPath)!);
    }

    public bool VaultExists() => File.Exists(VaultPath);

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool CreateVault(string masterPassword)
    {
        try
        {
            CurrentVault = new Vault
            {
                MasterPasswordHash = HashPassword(masterPassword)
            };
            _masterPassword = masterPassword;
            
            // Add default category
            CurrentVault.Categories.Add(new Category
            {
                Name = "General",
                Icon = "Folder24"
            });
            
            SaveVault();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool UnlockVault(string masterPassword)
    {
        try
        {
            if (!File.Exists(VaultPath)) return false;

            var encryptedData = File.ReadAllBytes(VaultPath);
            var decryptedJson = Decrypt(encryptedData, masterPassword);
            
            CurrentVault = JsonConvert.DeserializeObject<Vault>(decryptedJson);
            
            if (CurrentVault == null) return false;
            
            // Verify password
            if (CurrentVault.MasterPasswordHash != HashPassword(masterPassword))
                return false;

            _masterPassword = masterPassword;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void SaveVault()
    {
        if (CurrentVault == null || string.IsNullOrEmpty(_masterPassword)) return;

        var json = JsonConvert.SerializeObject(CurrentVault, Formatting.Indented);
        var encryptedData = Encrypt(json, _masterPassword);
        File.WriteAllBytes(VaultPath, encryptedData);
    }

    public bool VerifyMasterPassword(string password)
    {
        if (CurrentVault == null) return false;
        return CurrentVault.MasterPasswordHash == HashPassword(password);
    }

    public void ChangeMasterPassword(string oldPassword, string newPassword)
    {
        if (CurrentVault == null || !VerifyMasterPassword(oldPassword))
            return;

        // Update the master password hash
        CurrentVault.MasterPasswordHash = HashPassword(newPassword);
        
        // Update the encryption password
        _masterPassword = newPassword;
        
        // Save with new password
        SaveVault();
    }

    public void LockVault()
    {
        CurrentVault = null;
        _masterPassword = null;
    }

    private byte[] Encrypt(string plainText, string password)
    {
        using var aes = Aes.Create();
        var key = new Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64 }, 10000, HashAlgorithmName.SHA256);
        aes.Key = key.GetBytes(32);
        aes.IV = key.GetBytes(16);

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
    }

    private string Decrypt(byte[] cipherText, string password)
    {
        using var aes = Aes.Create();
        var key = new Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64 }, 10000, HashAlgorithmName.SHA256);
        aes.Key = key.GetBytes(32);
        aes.IV = key.GetBytes(16);

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }

    public string GeneratePassword(int length = 16)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
        var random = new Random();
        return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }
}
