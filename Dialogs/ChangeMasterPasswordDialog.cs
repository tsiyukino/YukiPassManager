using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using PasswordBox = Wpf.Ui.Controls.PasswordBox;

namespace YukiManager.Dialogs;

public class ChangeMasterPasswordDialog : FluentWindow
{
    private readonly PasswordBox _oldPasswordBox;
    private readonly PasswordBox _newPasswordBox;
    private readonly PasswordBox _confirmPasswordBox;
    
    public string OldPassword => _oldPasswordBox.Password;
    public string NewPassword => _newPasswordBox.Password;

    public ChangeMasterPasswordDialog()
    {
        Title = "Change Master Password";
        Width = 450;
        Height = 440;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        ResizeMode = ResizeMode.NoResize;

        var mainGrid = new Grid { Margin = new Thickness(30) };
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var titleBlock = new System.Windows.Controls.TextBlock
        {
            Text = "Change Master Password",
            FontSize = 20,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 20)
        };
        Grid.SetRow(titleBlock, 0);
        mainGrid.Children.Add(titleBlock);

        var warningBlock = new System.Windows.Controls.TextBlock
        {
            Text = "Make sure you remember your new master password. It cannot be recovered if lost.",
            TextWrapping = TextWrapping.Wrap,
            Opacity = 0.7,
            Margin = new Thickness(0, 0, 0, 20)
        };
        Grid.SetRow(warningBlock, 1);
        mainGrid.Children.Add(warningBlock);

        var fieldsStack = new StackPanel();
        
        var oldLabel = new System.Windows.Controls.TextBlock
        {
            Text = "Current Password:",
            Margin = new Thickness(0, 0, 0, 8)
        };
        fieldsStack.Children.Add(oldLabel);

        _oldPasswordBox = new PasswordBox
        {
            PlaceholderText = "Enter current password",
            Margin = new Thickness(0, 0, 0, 15)
        };
        fieldsStack.Children.Add(_oldPasswordBox);

        var newLabel = new System.Windows.Controls.TextBlock
        {
            Text = "New Password:",
            Margin = new Thickness(0, 0, 0, 8)
        };
        fieldsStack.Children.Add(newLabel);

        _newPasswordBox = new PasswordBox
        {
            PlaceholderText = "Enter new password",
            Margin = new Thickness(0, 0, 0, 15)
        };
        fieldsStack.Children.Add(_newPasswordBox);

        var confirmLabel = new System.Windows.Controls.TextBlock
        {
            Text = "Confirm New Password:",
            Margin = new Thickness(0, 0, 0, 8)
        };
        fieldsStack.Children.Add(confirmLabel);

        _confirmPasswordBox = new PasswordBox
        {
            PlaceholderText = "Re-enter new password",
            Margin = new Thickness(0, 0, 0, 15)
        };
        fieldsStack.Children.Add(_confirmPasswordBox);

        Grid.SetRow(fieldsStack, 2);
        mainGrid.Children.Add(fieldsStack);

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 20, 0, 0)
        };

        var saveButton = new Wpf.Ui.Controls.Button
        {
            Content = "Change Password",
            Margin = new Thickness(0, 0, 10, 0),
            MinWidth = 120,
            Padding = new Thickness(16, 8, 16, 8)
        };
        saveButton.Click += SaveButton_Click;

        var cancelButton = new Wpf.Ui.Controls.Button
        {
            Content = "Cancel",
            Appearance = Wpf.Ui.Controls.ControlAppearance.Secondary,
            MinWidth = 100,
            Padding = new Thickness(16, 8, 16, 8)
        };
        cancelButton.Click += (s, e) => { DialogResult = false; Close(); };

        buttonPanel.Children.Add(saveButton);
        buttonPanel.Children.Add(cancelButton);
        Grid.SetRow(buttonPanel, 3);
        mainGrid.Children.Add(buttonPanel);

        Content = mainGrid;

        _oldPasswordBox.Loaded += (s, e) => _oldPasswordBox.Focus();
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_oldPasswordBox.Password))
        {
            var msg = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Error",
                Content = "Please enter your current password.",
                Owner = this
            };
            await msg.ShowDialogAsync();
            return;
        }

        if (string.IsNullOrWhiteSpace(_newPasswordBox.Password))
        {
            var msg = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Error",
                Content = "Please enter a new password.",
                Owner = this
            };
            await msg.ShowDialogAsync();
            return;
        }

        if (_newPasswordBox.Password.Length < 8)
        {
            var msg = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Error",
                Content = "New password must be at least 8 characters long.",
                Owner = this
            };
            await msg.ShowDialogAsync();
            return;
        }

        if (_newPasswordBox.Password != _confirmPasswordBox.Password)
        {
            var msg = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Error",
                Content = "New passwords do not match.",
                Owner = this
            };
            await msg.ShowDialogAsync();
            return;
        }

        DialogResult = true;
        Close();
    }
}
