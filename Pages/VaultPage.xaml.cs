using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;
using YukiManager.Dialogs;
using YukiManager.Models;
using YukiManager.Services;
using YukiManager.Windows;

namespace YukiManager.Pages;

public partial class VaultPage : Page
{
    private VaultService _vaultService;
    private Category _currentCategory;

    public VaultPage()
    {
        InitializeComponent();
        Loaded += VaultPage_Loaded;
    }

    private void VaultPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this) is MainWindow mainWindow)
        {
            _vaultService = mainWindow.VaultService;
            
            // Check if a category was passed via Tag
            if (mainWindow.Tag is Category category)
            {
                _currentCategory = category;
                mainWindow.Tag = null; // Clear it
                LoadEntries();
            }
            else
            {
                // Show all entries
                LoadAllEntries();
            }
        }
    }
    
    public void SetCategory(Category category)
    {
        _currentCategory = category;
        if (_currentCategory != null)
            LoadEntries();
        else
            LoadAllEntries();
    }

    private void LoadEntries(string searchText = null)
    {
        if (_currentCategory == null) return;
        
        var entries = _currentCategory.Entries.AsEnumerable();
        
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            entries = entries.Where(e => 
                e.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                e.Username.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }
        
        EntriesListControl.ItemsSource = entries.OrderByDescending(e => e.ModifiedAt);
    }

    private void LoadAllEntries(string searchText = null)
    {
        var allEntries = _vaultService.CurrentVault?.Categories
            .SelectMany(c => c.Entries) ?? Enumerable.Empty<VaultEntry>();
        
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            allEntries = allEntries.Where(e => 
                e.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                e.Username.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }
        
        EntriesListControl.ItemsSource = allEntries.OrderByDescending(e => e.ModifiedAt);
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this) is MainWindow mainWindow)
        {
            mainWindow.NavigationView.Navigate(typeof(HomePage));
        }
    }

    private void AddEntry_Click(object sender, RoutedEventArgs e)
    {
        // If no category exists, create one first
        if (_vaultService.CurrentVault?.Categories.Count == 0)
        {
            var msg = new Wpf.Ui.Controls.MessageBox
            {
                Title = "No Categories",
                Content = "Please create a category first from the Home page.",
                Owner = Window.GetWindow(this)
            };
            msg.ShowDialogAsync();
            return;
        }
        
        // Get the target category
        var category = _currentCategory ?? _vaultService.CurrentVault.Categories.FirstOrDefault();
        if (category == null) return;

        var dialog = new AddEntryDialog(_vaultService) { Owner = Window.GetWindow(this) };
        if (dialog.ShowDialog() == true && dialog.Entry != null)
        {
            category.Entries.Add(dialog.Entry);
            _vaultService.SaveVault();
            
            if (_currentCategory != null)
                LoadEntries();
            else
                LoadAllEntries();
        }
    }

    private void ViewEntry_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Wpf.Ui.Controls.Button btn && btn.Tag is string entryId)
        {
            var entry = FindEntry(entryId);
            if (entry != null)
            {
                var dialog = new ViewEntryDialog(entry) { Owner = Window.GetWindow(this) };
                dialog.ShowDialog();
            }
        }
    }

    private void EditEntry_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Wpf.Ui.Controls.Button btn && btn.Tag is string entryId)
        {
            var entry = FindEntry(entryId);
            if (entry != null)
            {
                var dialog = new AddEntryDialog(_vaultService, entry) { Owner = Window.GetWindow(this) };
                if (dialog.ShowDialog() == true && dialog.Entry != null)
                {
                    // Entry is updated in place by the dialog
                    entry.ModifiedAt = DateTime.Now;
                    _vaultService.SaveVault();
                    
                    if (_currentCategory != null)
                        LoadEntries();
                    else
                        LoadAllEntries();
                }
            }
        }
    }

    private void DeleteEntry_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Wpf.Ui.Controls.Button btn && btn.Tag is string entryId)
        {
            var entry = FindEntry(entryId);
            if (entry != null)
            {
                var confirmDialog = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Delete Entry",
                    Content = $"Are you sure you want to delete '{entry.Title}'?",
                    PrimaryButtonText = "Delete",
                    CloseButtonText = "Cancel",
                    Owner = Window.GetWindow(this)
                };
                
                var result = confirmDialog.ShowDialogAsync().Result;
                
                if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
                {
                    RemoveEntry(entryId);
                    _vaultService.SaveVault();
                    
                    if (_currentCategory != null)
                        LoadEntries();
                    else
                        LoadAllEntries();
                }
            }
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_currentCategory != null)
            LoadEntries(SearchBox.Text);
        else
            LoadAllEntries(SearchBox.Text);
    }

    private VaultEntry FindEntry(string entryId)
    {
        if (_currentCategory != null)
            return _currentCategory.Entries.FirstOrDefault(e => e.Id == entryId);
        
        return _vaultService.CurrentVault?.Categories
            .SelectMany(c => c.Entries)
            .FirstOrDefault(e => e.Id == entryId);
    }

    private void RemoveEntry(string entryId)
    {
        if (_currentCategory != null)
        {
            var entry = _currentCategory.Entries.FirstOrDefault(e => e.Id == entryId);
            if (entry != null)
                _currentCategory.Entries.Remove(entry);
        }
        else
        {
            foreach (var category in _vaultService.CurrentVault?.Categories ?? new())
            {
                var entry = category.Entries.FirstOrDefault(e => e.Id == entryId);
                if (entry != null)
                {
                    category.Entries.Remove(entry);
                    break;
                }
            }
        }
    }
}
