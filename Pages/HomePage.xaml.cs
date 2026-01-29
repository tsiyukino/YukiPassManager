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

public partial class HomePage : Page
{
    private VaultService _vaultService;
    private Category _selectedCategory;

    public HomePage()
    {
        InitializeComponent();
        Loaded += HomePage_Loaded;
    }

    private void HomePage_Loaded(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this) is MainWindow mainWindow)
        {
            _vaultService = mainWindow.VaultService;
            LoadCategories();
        }
    }

    private void LoadCategories(string searchText = null)
    {
        var categories = _vaultService.CurrentVault?.Categories ?? new System.Collections.ObjectModel.ObservableCollection<Category>();
        
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            categories = new System.Collections.ObjectModel.ObservableCollection<Category>(
                categories.Where(c => c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
        }
        
        CategoriesItemsControl.ItemsSource = categories;
        
        // Clear selection when reloading
        _selectedCategory = null;
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        EditCategoryButton.IsEnabled = _selectedCategory != null;
        DeleteCategoryButton.IsEnabled = _selectedCategory != null;
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        LoadCategories(SearchBox.Text);
    }

    private void AddCategory_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AddCategoryDialog() { Owner = Window.GetWindow(this) };
        var result = dialog.ShowDialog();
        
        if (result == true && !string.IsNullOrWhiteSpace(dialog.CategoryName))
        {
            var category = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = dialog.CategoryName,
                Entries = new System.Collections.ObjectModel.ObservableCollection<VaultEntry>()
            };
            
            _vaultService.CurrentVault?.Categories.Add(category);
            _vaultService.SaveVault();
            LoadCategories();
        }
    }

    private void CategoryCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is CardAction card && card.Tag is string categoryId)
        {
            var category = _vaultService.CurrentVault?.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category != null && Window.GetWindow(this) is MainWindow mainWindow)
            {
                // Store category for VaultPage to pick up
                mainWindow.Tag = category;
                
                // Navigate to VaultPage
                mainWindow.NavigationView.Navigate(typeof(VaultPage));
            }
        }
    }

    private void CategoryCard_RightClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is CardAction card && card.Tag is string categoryId)
        {
            var category = _vaultService.CurrentVault?.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category != null)
            {
                _selectedCategory = category;
                UpdateButtonStates();
            }
        }
    }

    private void EditSelectedCategory_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedCategory == null) return;
        
        var dialog = new AddCategoryDialog(_selectedCategory.Name) { Owner = Window.GetWindow(this) };
        var result = dialog.ShowDialog();
        
        if (result == true && !string.IsNullOrWhiteSpace(dialog.CategoryName))
        {
            _selectedCategory.Name = dialog.CategoryName;
            _vaultService.SaveVault();
            LoadCategories();
        }
    }

    private void DeleteSelectedCategory_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedCategory == null) return;
        
        var confirmDialog = new Wpf.Ui.Controls.MessageBox
        {
            Title = "Delete Category",
            Content = $"Are you sure you want to delete '{_selectedCategory.Name}'? All entries in this category will be deleted.",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            Owner = Window.GetWindow(this)
        };
        
        var result = confirmDialog.ShowDialogAsync().Result;
        
        if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
        {
            _vaultService.CurrentVault?.Categories.Remove(_selectedCategory);
            _vaultService.SaveVault();
            _selectedCategory = null;
            LoadCategories();
        }
    }
}
