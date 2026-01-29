using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;
using YukiManager.Services;

namespace YukiManager.Windows;

public partial class LoginWindow : FluentWindow
{
    private readonly VaultService _vaultService;

    public LoginWindow()
    {
        InitializeComponent();
        _vaultService = new VaultService();
        
        if (!_vaultService.VaultExists())
        {
            CreateVaultButton.Visibility = Visibility.Visible;
            UnlockButton.Visibility = Visibility.Collapsed;
        }
    }

    private async void UnlockButton_Click(object sender, RoutedEventArgs e)
    {
        var password = MasterPasswordBox.Password;

        if (string.IsNullOrWhiteSpace(password))
        {
            var emptyDialog = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Error",
                Content = "Please enter your master password.",
            };
            await emptyDialog.ShowDialogAsync();
            return;
        }

        if (_vaultService.UnlockVault(password))
        {
            var mainWindow = new MainWindow(_vaultService);
            mainWindow.Show();
            Close();
        }
        else
        {
            var errorDialog = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Error",
                Content = "Incorrect master password. Please try again.",
            };
            await errorDialog.ShowDialogAsync();
            MasterPasswordBox.Clear();
        }
    }

    private async void CreateVaultButton_Click(object sender, RoutedEventArgs e)
    {
        var password = MasterPasswordBox.Password;

        if (string.IsNullOrWhiteSpace(password))
        {
            var emptyDialog = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Error",
                Content = "Please enter a master password.",
            };
            await emptyDialog.ShowDialogAsync();
            return;
        }

        if (password.Length < 8)
        {
            var weakDialog = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Weak Password",
                Content = "Master password should be at least 8 characters long.",
            };
            await weakDialog.ShowDialogAsync();
            return;
        }

        if (_vaultService.CreateVault(password))
        {
            var successDialog = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Success",
                Content = "Vault created successfully!",
            };
            await successDialog.ShowDialogAsync();
            
            CreateVaultButton.Visibility = Visibility.Collapsed;
            UnlockButton.Visibility = Visibility.Visible;
            MasterPasswordBox.Clear();
        }
        else
        {
            var errorDialog = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Error",
                Content = "Failed to create vault. Please try again.",
            };
            await errorDialog.ShowDialogAsync();
        }
    }

    private void MasterPasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (UnlockButton.Visibility == Visibility.Visible)
                UnlockButton_Click(sender, e);
            else
                CreateVaultButton_Click(sender, e);
        }
    }
}
