# Yuki Manager - Secure Password Manager

A modern, secure password manager built with WPF and WPF UI 3.0.

## Features

### ✅ Fixed Issues
- **Proper Master Password Authentication** - Uses SHA-256 hashing for secure password verification
- **WPF UI NavigationView** - Proper sidebar navigation with Home and Vault pages
- **Category Management** - User-creatable categories with add/edit/delete functionality
- **No Primary (Blue) Buttons** - Uses standard appearance throughout
- **WPF UI MessageBox** - Replaced System.Windows.MessageBox with WPF UI dialogs

### 🔐 Security Features
- AES-256 encryption for vault data
- SHA-256 password hashing
- Encrypted file storage
- Password generator

### 📁 Category System
- Create unlimited categories
- Add multiple entries per category
- Category cards on homepage showing entry counts
- Search and filter categories
- Edit/delete categories

### 📝 Entry Types
1. **Password Entries**
   - Title, Username, Password
   - URL and Notes fields
   - Built-in password generator

2. **Secure Notes**
   - Title and Content
   - Encrypted text storage

3. **Credit Cards**
   - Card Number, Holder Name
   - Expiry Date and CVV

### 🎨 User Interface
- Modern WPF UI 3.0 design
- Dark theme support
- NavigationView sidebar
  - Home page with category cards
  - Vault page with entry list
  - Lock button in footer
- Search functionality
- Responsive layout

## Build Instructions

### Prerequisites
- .NET 8.0 SDK or later
- Windows OS

### Building
```bash
cd Yuki-Manager-New
dotnet restore
dotnet build
dotnet run
```

### Publishing
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

The executable will be in: `bin/Release/net8.0-windows/win-x64/publish/`

## Project Structure

```
Yuki-Manager-New/
├── Models/
│   └── Models.cs          # Data models (Category, VaultEntry, Vault)
├── Services/
│   └── VaultService.cs    # Encryption, vault management
├── Windows/
│   ├── LoginWindow.xaml   # Login/create vault window
│   └── MainWindow.xaml    # Main application window with NavigationView
├── Pages/
│   ├── HomePage.xaml      # Category cards homepage
│   └── VaultPage.xaml     # Entry list page
├── Dialogs/
│   ├── AddCategoryDialog.cs    # Add/edit category dialog
│   ├── AddEntryDialog.cs       # Add/edit entry dialog
│   └── ViewEntryDialog.cs      # View entry details dialog
├── App.xaml               # Application resources
└── YukiManager.csproj     # Project file
```

## Usage

### First Time Setup
1. Launch the application
2. Enter a master password (minimum 8 characters)
3. Click "Create New Vault"

### Daily Use
1. Enter your master password to unlock
2. Browse categories on the Home page
3. Click a category to view entries
4. Add new entries using the "Add Entry" button
5. Lock vault when done using the Lock button in sidebar footer

### Category Management
- **Add Category**: Click "Add Category" on Home page
- **Edit Category**: Click edit icon on category card
- **Delete Category**: Click delete icon on category card (deletes all entries in category)

### Entry Management
- **Add Entry**: Select category, click "Add Entry"
- **View Entry**: Double-click entry in list
- **Edit Entry**: Click "Edit" button on entry row
- **Delete Entry**: Click "Delete" button on entry row
- **Search**: Use search box to filter entries by title/username

## Security Notes

1. **Master Password**: Choose a strong master password. If forgotten, vault cannot be recovered
2. **Data Location**: Vault stored at `%AppData%/YukiManager/vault.dat`
3. **Encryption**: AES-256 encryption with password-derived key
4. **Password Hashing**: SHA-256 for master password verification

## Dependencies

- WPF-UI 3.0.5 - Modern WPF controls and themes
- Newtonsoft.Json 13.0.3 - JSON serialization
- .NET 8.0 - Framework

## License

MIT License - Feel free to use and modify.

## Changelog

### Version 2.0 (Current)
- ✅ Fixed master password authentication bug
- ✅ Implemented WPF UI NavigationView sidebar
- ✅ Added category management system
- ✅ Created homepage with category cards
- ✅ Replaced MessageBox with WPF UI dialogs
- ✅ Removed primary button appearance
- ✅ Added search functionality
- ✅ Improved UI/UX throughout

### Version 1.0
- Initial release with basic vault functionality
