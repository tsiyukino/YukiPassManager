using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace YukiManager.Dialogs;

public class AddCategoryDialog : FluentWindow
{
    private readonly TextBox _nameTextBox;
    public string CategoryName => _nameTextBox.Text;
    public bool Success { get; private set; }

    public AddCategoryDialog(string existingName = null)
    {
        Title = string.IsNullOrEmpty(existingName) ? "Add Category" : "Edit Category";
        Width = 450;
        Height = 250;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        ResizeMode = ResizeMode.NoResize;

        var mainGrid = new Grid { Margin = new Thickness(30) };
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var titleBlock = new System.Windows.Controls.TextBlock
        {
            Text = Title,
            FontSize = 20,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 20)
        };
        Grid.SetRow(titleBlock, 0);
        mainGrid.Children.Add(titleBlock);

        var label = new System.Windows.Controls.TextBlock
        {
            Text = "Category name:",
            Margin = new Thickness(0, 0, 0, 8)
        };
        Grid.SetRow(label, 1);
        mainGrid.Children.Add(label);

        _nameTextBox = new TextBox
        {
            PlaceholderText = "Enter category name",
            Text = existingName ?? string.Empty,
        };
        Grid.SetRow(_nameTextBox, 2);
        Grid.SetColumn(_nameTextBox, 0);
        mainGrid.Children.Add(_nameTextBox);

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 20, 0, 0)
        };

        var saveButton = new Wpf.Ui.Controls.Button
        {
            Content = "Save",
            Margin = new Thickness(0, 0, 10, 0),
            MinWidth = 100,
            Padding = new Thickness(16, 8, 16, 8)
        };
        saveButton.Click += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
            {
                var msg = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Error",
                    Content = "Please enter a category name.",
                    Owner = this
                };
                msg.ShowDialogAsync();
                return;
            }
            Success = true;
            Close();
        };

        var cancelButton = new Wpf.Ui.Controls.Button
        {
            Content = "Cancel",
            Appearance = Wpf.Ui.Controls.ControlAppearance.Secondary,
            MinWidth = 100,
            Padding = new Thickness(16, 8, 16, 8)
        };
        cancelButton.Click += (s, e) => { Success = false; Close(); };

        buttonPanel.Children.Add(saveButton);
        buttonPanel.Children.Add(cancelButton);
        Grid.SetRow(buttonPanel, 3);
        mainGrid.Children.Add(buttonPanel);

        Content = mainGrid;

        _nameTextBox.Loaded += (s, e) => _nameTextBox.Focus();
    }

    public new bool? ShowDialog()
    {
        base.ShowDialog();
        return Success;
    }
}
