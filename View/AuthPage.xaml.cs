using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DeliverySushi.ViewModel;
using System.Threading.Tasks;

namespace DeliverySushi.View
{
    public partial class AuthPage : Page
    {
        private readonly UserViewModel userViewModel;
        private readonly WorkerViewModel workerViewModel;

        public AuthPage()
        {
            InitializeComponent();
            userViewModel = new UserViewModel();
            workerViewModel = new WorkerViewModel();
            var mainViewModel = (MainViewModel)Application.Current.MainWindow.DataContext;
        }

        private async void Button_Auth_Click(object sender, RoutedEventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string password = passBox.Password.Trim();

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

            ResetFieldStyle(textBoxLogin);
            ResetFieldStyle(passBox);

            object authCustomer = null;
            if (btnWorker.Content != "Назад")
            {
                authCustomer = await userViewModel.AuthenticateAsync(login, password);
            }
            else
            {
                authCustomer = await workerViewModel.AuthenticateAsync(login, password);
            }

            if (authCustomer != null)
            {
                var mainViewModel = (MainViewModel)Application.Current.MainWindow.DataContext;

                // Устанавливаем ID пользователя
                if (authCustomer is Model.User user)
                {
                    mainViewModel.UserViewModel = new UserViewModel();
                    // Устанавливаем ID пользователя

                    mainViewModel.SushiViewModel.SetId(user.id);
                    mainViewModel.SetViewModel.SetId(user.id);
                    mainViewModel.AddonViewModel.SetId(user.id);
                    mainViewModel.CartViewModel.SetId(user.id);
                    mainViewModel.UserViewModel.SetId(user.id);
                    mainViewModel.OrderViewModel.SetId(user.id);

                    NavigationService.Navigate(new UserPage());
                }
                else if (authCustomer is Model.Worker worker)
                {
                    mainViewModel.WorkerViewModel = new WorkerViewModel();
                    mainViewModel.StatisticsViewModel = new StatisticsViewModel();

                    mainViewModel.OrderViewModel.SetId(worker.id);
                    mainViewModel.WorkerViewModel.SetId(worker.id);

                    if (worker.position == "курьер")
                    {
                        mainViewModel.CourierViewModel = new CourierViewModel(worker.id);
                        NavigationService.Navigate(new CourierPage());
                    }
                    else
                    {
                        NavigationService.Navigate(new AdminPage());
                    }
                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.");
            }
        }

        private void Button_Reg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
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

        private void Button_Worker_Click(object sender, RoutedEventArgs e)
        {
            if (btnWorker.Content != "Назад")
            {
                // Применяем стиль для кнопок "Войти" и "Войти в кабинет"
                btnAuth.Style = (Style)FindResource("RoundedButtonStyle");
                btnAuthUser.Style = (Style)FindResource("RoundedButtonStyle");

                // Изменяем цвет текста кнопки "Для сотрудников"
                btnWorker.Foreground = new SolidColorBrush(Color.FromRgb(75, 75, 158)); // Голубовато-синий
                btnReg.Visibility = Visibility.Hidden;

                btnWorker.Content = "Назад";
            }
            else
            {
                // Возвращаем стиль и цвет кнопок
                btnWorker.Content = "Для сотрудников";
                btnWorker.Foreground = new SolidColorBrush(Color.FromRgb(75, 75, 158)); // Голубовато-синий
                btnAuth.Style = (Style)FindResource("RoundedButtonStyle");
                btnAuthUser.Style = (Style)FindResource("RoundedButtonStyle");
                btnReg.Visibility = Visibility.Visible;
            }
        }
    }
}