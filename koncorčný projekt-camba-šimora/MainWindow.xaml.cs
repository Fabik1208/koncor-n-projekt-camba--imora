using System;
using System.Windows;

namespace koncorčný_projekt_camba_šimora
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowLogin(); // default
        }

        private void ShowLogin()
        {
            LoginGrid.Visibility = Visibility.Visible;
            RegisterGrid.Visibility = Visibility.Collapsed;
            BtnLoginTab.IsEnabled = false;
            BtnRegisterTab.IsEnabled = true;
        }

        private void ShowRegister()
        {
            LoginGrid.Visibility = Visibility.Collapsed;
            RegisterGrid.Visibility = Visibility.Visible;
            BtnLoginTab.IsEnabled = true;
            BtnRegisterTab.IsEnabled = false;
        }

        private void SwitchToLogin_Click(object sender, RoutedEventArgs e)
        {
            ShowLogin();
        }

        private void SwitchToRegister_Click(object sender, RoutedEventArgs e)
        {
            ShowRegister();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var email = LoginEmail.Text?.Trim();
            var pw = LoginPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pw))
            {
                MessageBox.Show("Please fill in both email and password.", "Missing information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: Replace with real authentication
            MessageBox.Show($"Welcome back, {email}!", "Signed in", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var name = RegName.Text?.Trim();
            var email = RegEmail.Text?.Trim();
            var pw = RegPassword.Password;
            var confirm = RegConfirmPassword.Password;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pw))
            {
                MessageBox.Show("Please complete all fields.", "Missing information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (pw != confirm)
            {
                MessageBox.Show("Passwords do not match.", "Validation error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // TODO: Replace with real registration logic (save user, validate email, etc.)
            MessageBox.Show("Account created successfully. You can now sign in.", "Registered", MessageBoxButton.OK, MessageBoxImage.Information);
            ShowLogin();
        }
    }
}
