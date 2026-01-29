using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using YukiManager.Dialogs;
using YukiManager.Services;
using YukiManager.Windows;

namespace YukiManager.Pages;

public partial class SettingsPage : Page
{
    private VaultService _vaultService;

    public SettingsPage()
    {
        InitializeComponent();
        Loaded += SettingsPage_Loaded;
    }

    private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this) is MainWindow mainWindow)
        {
            _vaultService = mainWindow.VaultService;
        }
    }

    private async void ChangeMasterPassword_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ChangeMasterPasswordDialog() { Owner = Window.GetWindow(this) };
        if (dialog.ShowDialog() == true)
        {
            var oldPassword = dialog.OldPassword;
            var newPassword = dialog.NewPassword;
            
            // Verify old password
            if (_vaultService.VerifyMasterPassword(oldPassword))
            {
                // Change password
                _vaultService.ChangeMasterPassword(oldPassword, newPassword);
                
                var successMsg = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Success",
                    Content = "Master password changed successfully!",
                    Owner = Window.GetWindow(this)
                };
                await successMsg.ShowDialogAsync();
            }
            else
            {
                var errorMsg = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Error",
                    Content = "Current password is incorrect.",
                    Owner = Window.GetWindow(this)
                };
                await errorMsg.ShowDialogAsync();
            }
        }
    }
}
