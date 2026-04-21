using System.Windows;
using koncorcny_projekt_camba_simora;
using koncorčný_projekt_camba_šimora.Services;

namespace koncorčný_projekt_camba_šimora
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UserStore _userStore = new();

        public MainWindow()
        {
            InitializeComponent();
            ShowLogin(); // default

            // load remembered email
            var remembered = _userStore.GetRememberedEmail();
            if (!string.IsNullOrEmpty(remembered))
            {
                LoginEmail.Text = remembered;
                RememberMe.IsChecked = true;
            }
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

            // Ensure email contains '@'
            if (!email.Contains('@'))
            {
                MessageBox.Show("Please enter a valid email address containing '@'.", "Invalid email", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_userStore.Authenticate(email, pw, out var userName))
            {
                MessageBox.Show("Invalid email or password.", "Authentication failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // remember preference
            if (RememberMe.IsChecked == true)
                _userStore.RememberEmail(email);
            else
                _userStore.RememberEmail(null);

            // Open dashboard window and hide main while dashboard is open
            DashboardWindow dashboardWindow = new(string.IsNullOrEmpty(userName) ? email : userName)
            {
                Owner = this
            };
            DashboardWindow dashboard = dashboardWindow;

            Hide();
            dashboard.ShowDialog();
            Show();
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

            // Ensure email contains '@'
            if (!email.Contains('@'))
            {
                MessageBox.Show("Please enter a valid email address containing '@'.", "Invalid email", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (pw != confirm)
            {
                MessageBox.Show("Passwords do not match.", "Validation error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_userStore.CreateUser(name, email, pw, out var error))
            {
                MessageBox.Show(error, "Registration error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Account created successfully. You can now sign in.", "Registered", MessageBoxButton.OK, MessageBoxImage.Information);
            // pre-fill login email
            LoginEmail.Text = email;
            ShowLogin();
        }
    }
}
