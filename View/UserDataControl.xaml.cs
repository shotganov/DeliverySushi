using DeliverySushi.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace DeliverySushi.View
{
    public partial class UserDataControl : UserControl
    {
        public UserDataControl()
        {
            InitializeComponent();
            var mainViewModel = (MainViewModel)Application.Current.MainWindow.DataContext;
            DataContext = mainViewModel.UserViewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && DataContext is UserViewModel viewModel)
            {
                viewModel.TempPassword = passwordBox.Password; // Обновляем TempPassword
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Tag is string password)
            {
                PasswordBox.Password = password; // Устанавливаем значение пароля из ViewModel
            }

            if (DataContext is UserViewModel viewModel)
            {
                viewModel.LoadUser(viewModel.User.id);
            }
        }
    }
}