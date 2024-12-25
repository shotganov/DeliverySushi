using DeliverySushi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeliverySushi.View
{
    /// <summary>
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
  
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
         
            //var mainViewModel = (MainViewModel)Application.Current.MainWindow.DataContext;
            //DataContext = mainViewModel;
            ContentArea.Content = new UserSushiControl();
        }

        private void SushiButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContentArea.Content = new UserSushiControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading main page: " + ex.Message);
            }
        }
        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContentArea.Content = new UserSetControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading main page: " + ex.Message);
            }
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContentArea.Content = new UserCartControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading main page: " + ex.Message);
            }
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContentArea.Content = new UserDataControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading main page: " + ex.Message);
            }
        }

        private void AddonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContentArea.Content = new UserAddonControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading main page: " + ex.Message);
            }
        }

        private void ButtonOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContentArea.Content = new UserOrderControl();
                //ContentArea.Content = new Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading main page: " + ex.Message);
            }
        }
    }
}
