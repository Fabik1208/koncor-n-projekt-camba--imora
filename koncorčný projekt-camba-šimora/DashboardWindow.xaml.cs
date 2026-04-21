using System.Windows;

namespace koncorcny_projekt_camba_simora
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow(string userName)
        {
            InitializeComponent();
            WelcomeText.Text = $"Welcome, {userName}!";
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}