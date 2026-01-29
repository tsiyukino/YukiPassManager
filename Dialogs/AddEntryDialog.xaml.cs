using System;
using System.Windows;
using Wpf.Ui.Controls;
using YukiManager.Models;
using YukiManager.Services;

namespace YukiManager.Dialogs
{
    public partial class AddEntryDialog : FluentWindow
    {
        private readonly VaultService _vaultService;
        private readonly VaultEntry _existingEntry;
        
        public VaultEntry Entry { get; private set; }

        public AddEntryDialog(VaultService vaultService, VaultEntry existingEntry = null)
        {
            InitializeComponent();
            _vaultService = vaultService;
            _existingEntry = existingEntry;
            
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            if (_existingEntry != null)
            {
                Title = "Edit Entry";
                SaveButton.Content = "Save Changes";
                
                // Load existing data
                TitleInput.Text = _existingEntry.Title;
                TypeComboBox.SelectedIndex = (int)_existingEntry.Type;
                UsernameInput.Text = _existingEntry.Username;
                PasswordInput.Password = _existingEntry.Password;
                UrlInput.Text = _existingEntry.Url;
                NotesInput.Text = _existingEntry.Notes;
                
                // Credit card fields
                CardNumberInput.Text = _existingEntry.CardNumber;
                CardHolderInput.Text = _existingEntry.CardHolder;
                ExpiryInput.Text = _existingEntry.ExpiryDate;
                CVVInput.Password = _existingEntry.CVV;
            }
            else
            {
                Title = "Add Entry";
                SaveButton.Content = "Add Entry";
                TypeComboBox.SelectedIndex = 0;
            }
            
            UpdateFieldVisibility();
        }

        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateFieldVisibility();
        }

        private void UpdateFieldVisibility()
        {
            if (TypeComboBox == null) return;
            
            var selectedType = (EntryType)TypeComboBox.SelectedIndex;
            
            PasswordFields.Visibility = selectedType == EntryType.Password ? Visibility.Visible : Visibility.Collapsed;
            NoteFields.Visibility = selectedType == EntryType.SecureNote ? Visibility.Visible : Visibility.Collapsed;
            CardFields.Visibility = selectedType == EntryType.CreditCard ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            PasswordInput.Password = _vaultService.GeneratePassword();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(TitleInput.Text))
            {
                var msg = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Validation Error",
                    Content = "Please enter a title",
                    Owner = this
                };
                msg.ShowDialogAsync();
                return;
            }
            
            var selectedType = (EntryType)TypeComboBox.SelectedIndex;
            
            // Create or update entry
            var entry = _existingEntry ?? new VaultEntry();
            
            entry.Title = TitleInput.Text;
            entry.Type = selectedType;
            entry.Username = UsernameInput.Text;
            entry.Password = PasswordInput.Password;
            entry.Url = UrlInput.Text;
            entry.Notes = NotesInput.Text;
            entry.Content = NotesInput.Text; // For secure notes
            
            // Credit card fields
            entry.CardNumber = CardNumberInput.Text;
            entry.CardHolder = CardHolderInput.Text;
            entry.ExpiryDate = ExpiryInput.Text;
            entry.CVV = CVVInput.Password;
            
            if (_existingEntry == null)
            {
                entry.CreatedAt = DateTime.Now;
            }
            entry.ModifiedAt = DateTime.Now;
            
            Entry = entry;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
