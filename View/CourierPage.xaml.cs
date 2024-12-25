using DeliverySushi.Model;
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
    /// Логика взаимодействия для CourierPage.xaml
    /// </summary>
    public partial class CourierPage : Page
    {
        private CourierViewModel courierViewModel;
        public CourierPage()
        {
            InitializeComponent();
            var mainViewModel = (MainViewModel)Application.Current.MainWindow.DataContext;
            courierViewModel = mainViewModel.CourierViewModel;
            courierViewModel.ConfirmDeliveryButtonVisible = false;
            DataContext = courierViewModel;

        }

        private void ButtonWaitOrder_Click(object sender, RoutedEventArgs e)
        {

        }


        private void ButtonInDelivery_Click(object sender, RoutedEventArgs e)
        {


        }
        private void ButtonDelivered_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ButtonCanceled_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}