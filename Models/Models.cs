using System;
using System.Collections.ObjectModel;

namespace YukiManager.Models;

public class Category
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = "Folder24"; // FluentUI icon
    public ObservableCollection<VaultEntry> Entries { get; set; } = new();
}

public class VaultEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public EntryType Type { get; set; } = EntryType.Password;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime ModifiedAt { get; set; } = DateTime.Now;
    
    // Secure Note fields
    public string Content { get; set; } = string.Empty;
    
    // Credit Card fields
    public string CardNumber { get; set; } = string.Empty;
    public string CardHolder { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
}

public enum EntryType
{
    Password,
    SecureNote,
    CreditCard
}

public class Vault
{
    public ObservableCollection<Category> Categories { get; set; } = new();
    public string MasterPasswordHash { get; set; } = string.Empty;
}
