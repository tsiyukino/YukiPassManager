using System.Windows;
using Wpf.Ui.Controls;
using YukiManager.Models;

namespace YukiManager.Dialogs
{
    public partial class ViewEntryDialog : FluentWindow
    {
        private readonly VaultEntry _entry;

        public ViewEntryDialog(VaultEntry decryptedEntry)
        {
            InitializeComponent();
            _entry = decryptedEntry;
            
            LoadEntryData();
        }

        private void LoadEntryData()
        {
            EntryTitle.Text = _entry.Title;
            TypeText.Text = _entry.Type.ToString();
            
            // Show fields based on type
            switch (_entry.Type)
            {
                case EntryType.Password:
                    UsernameText.Text = _entry.Username;
                    PasswordText.Text = _entry.Password;
                    UrlText.Text = _entry.Url;
                    
                    UsernameRow.Visibility = Visibility.Visible;
                    PasswordRow.Visibility = Visibility.Visible;
                    UrlRow.Visibility = Visibility.Visible;
                    break;
                    
                case EntryType.SecureNote:
                    NotesText.Text = _entry.Notes;
                    NotesRow.Visibility = Visibility.Visible;
                    break;
                    
                case EntryType.CreditCard:
                    CardNumberText.Text = _entry.CardNumber;
                    CardHolderText.Text = _entry.CardHolder;
                    ExpiryText.Text = _entry.ExpiryDate;
                    CVVText.Text = _entry.CVV;
                    
                    CardNumberRow.Visibility = Visibility.Visible;
                    CardHolderRow.Visibility = Visibility.Visible;
                    ExpiryRow.Visibility = Visibility.Visible;
                    CVVRow.Visibility = Visibility.Visible;
                    break;
            }
            
            // Always show notes if present
            if (!string.IsNullOrWhiteSpace(_entry.Notes) && _entry.Type != EntryType.SecureNote)
            {
                NotesText.Text = _entry.Notes;
                NotesRow.Visibility = Visibility.Visible;
            }
            
            CreatedText.Text = _entry.CreatedAt.ToString("g");
            ModifiedText.Text = _entry.ModifiedAt.ToString("g");
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is string value)
            {
                Clipboard.SetText(value);
                
                // Show brief notification
                button.Content = "Copied!";
                var originalContent = "Copy";
                
                var timer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = System.TimeSpan.FromSeconds(2)
                };
                
                timer.Tick += (s, args) =>
                {
                    button.Content = originalContent;
                    timer.Stop();
                };
                
                timer.Start();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
