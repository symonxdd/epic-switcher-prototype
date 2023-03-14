using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EpicGamesAccountSwitcher
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            BasicFeaturesSetup.Setup(this, appTitleBar);
            BusinessLogic.Initialize(this, accountsComboBox, accountIdTextBlock, noAccountsWarningTextBlock, accountsCountTextBlock, switchAccountButton);
            BusinessLogic.UpdateUI();
        }

        private async void SwitchAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await BusinessLogic.ShowContentDialog("Switch?", "Are you sure you want to switch accounts?");
            if (result == ContentDialogResult.Primary)
            {
                BusinessLogic.SwitchAccount();
                accountIdTextBlock.Text = BusinessLogic.CurrentAccountId;
            }
        }

        private async void NewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await BusinessLogic.ShowContentDialog("Add?", "Are you sure you want to log into another account?");
            if (result == ContentDialogResult.Primary)
            {
                BusinessLogic.AddAccount();
            }
        }

        private void AccountsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BusinessLogic.SelectedAccountId = (string)accountsComboBox.SelectedItem;
        }
    }
}
