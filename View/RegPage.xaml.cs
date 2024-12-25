using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DeliverySushi.ViewModel;
using System.Threading.Tasks;

namespace DeliverySushi.View
{
    public partial class RegPage : Page
    {
        private readonly UserViewModel userViewModel;

        public RegPage()
        {
            InitializeComponent();
            userViewModel = new UserViewModel();
        }

        private async void Button_Reg_Click(object sender, RoutedEventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string password = passBox.Password.Trim();
            string confirmPassword = passBox_2.Password.Trim();
            string email = textBoxEmail.Text.Trim().ToLower();
            string phoneNum = textBoxNumber.Text.Trim();

            if (login.Length < 5)
            {
                ShowError(textBoxLogin, "Логин должен содержать не менее 5 символов.");
                return;
            }

            if (password.Length < 5)
            {
                ShowError(passBox, "Пароль должен содержать не менее 5 символов.");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError(passBox_2, "Пароли не совпадают.");
                return;
            }

            if (email.Length < 5 || !email.Contains("@") || !email.Contains("."))
            {
                ShowError(textBoxEmail, "Введите корректный email.");
                return;
            }

            if (phoneNum.Length != 11 || !long.TryParse(phoneNum, out long phone))
            {
                ShowError(textBoxNumber, "Введите корректный номер телефона.");
                return;
            }

            ResetFieldStyle(textBoxLogin);
            ResetFieldStyle(passBox);
            ResetFieldStyle(passBox_2);
            ResetFieldStyle(textBoxEmail);
            ResetFieldStyle(textBoxNumber);


            int registered = await userViewModel.RegisterAsync(login, password, email, phone);
            var mainViewModel = (MainViewModel)Application.Current.MainWindow.DataContext;
           

            mainViewModel.SushiViewModel.SetId(registered);
            mainViewModel.SetViewModel.SetId(registered);
            mainViewModel.AddonViewModel.SetId(registered);
            mainViewModel.CartViewModel.SetId(registered);
            mainViewModel.UserViewModel.SetId(registered);

            var customer = mainViewModel.UserViewModel.GetCustomerByIdAsync(registered);

            // Устанавливаем ID пользователя


            // Переход на пользовательскую страницу

            NavigationService.Navigate(new UserPage());
            if (registered > 0)
            {
                NavigationService.Navigate(new UserPage());
            }
            else
            {
                MessageBox.Show("Логин или email уже используется.");
            }
        }

        private void Button_Window_Auth_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void ShowError(Control control, string message)
        {
            control.ToolTip = message;
            control.Background = Brushes.DarkRed;
        }

        private void ResetFieldStyle(Control control)
        {
            control.ToolTip = null;
            control.Background = Brushes.Transparent;
        }
    }
}
