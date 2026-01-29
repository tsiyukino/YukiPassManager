using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using YukiManager.Pages;
using YukiManager.Services;

namespace YukiManager.Windows;

public partial class MainWindow : FluentWindow
{
    public VaultService VaultService { get; }
    public ContentPresenter DialogHost => DialogPresenter;
    private bool _isClosingToTray = true;

    public MainWindow(VaultService vaultService)
    {
        InitializeComponent();
        VaultService = vaultService;
        
        // Set DataContext so pages can access VaultService
        DataContext = this;
        
        // Navigate after window is loaded
        Loaded += MainWindow_Loaded;
        
        // Handle window closing and state changes for system tray
        Closing += MainWindow_Closing;
        StateChanged += MainWindow_StateChanged;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Navigate to Home by default (this will automatically select the item)
        NavigationView.Navigate(typeof(HomePage));
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        if (_isClosingToTray)
        {
            // Minimize to tray instead of closing
            e.Cancel = true;
            Hide();
            SystemTrayIcon.ShowBalloonTip("Yuki Manager", "Application minimized to system tray", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }
        else
        {
            // Actually close
            SystemTrayIcon?.Dispose();
        }
    }

    private void MainWindow_StateChanged(object sender, EventArgs e)
    {
        // Hide to tray when minimized
        if (WindowState == WindowState.Minimized)
        {
            Hide();
        }
    }

    private void NavigationView_SelectionChanged(NavigationView sender, RoutedEventArgs args)
    {
        // This handles automatic selection changes
        // The actual navigation is handled by Click events now
    }

    private void NavigationItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();
            
            // Clear any category parameter when navigating via sidebar
            Tag = null;
            
            switch (tag)
            {
                case "Home":
                    NavigationView.Navigate(typeof(HomePage));
                    break;
                case "Vault":
                    NavigationView.Navigate(typeof(VaultPage));
                    break;
                case "Settings":
                    NavigationView.Navigate(typeof(SettingsPage));
                    break;
            }
        }
    }

    private void SystemTrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
    {
        ShowWindowFromTray();
    }

    private void ShowWindow_Click(object sender, RoutedEventArgs e)
    {
        ShowWindowFromTray();
    }

    private void ShowWindowFromTray()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _isClosingToTray = false;
        Close();
    }

    private void LockVault_Click(object sender, RoutedEventArgs e)
    {
        VaultService.SaveVault();
        VaultService.LockVault();
        
        _isClosingToTray = false;
        
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        Close();
    }
}
