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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private StatisticsViewModel statisticsViewModel;
        private StatisticsControl statisticsControl;
        public AdminPage()
        {
            InitializeComponent();
            var mainViewModel = (MainViewModel)Application.Current.MainWindow.DataContext;
            statisticsViewModel = mainViewModel.StatisticsViewModel;
            DataContext = statisticsViewModel;

            statisticsControl = new StatisticsControl
            {
                DataContext = statisticsViewModel
            };

            ContentArea.Content = statisticsControl;
        }

        private void ButtonShowStatistics_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = statisticsControl;
        }

        private void ButtonLoadNewOrder_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new AdminCard();
        }
        private void ButtonLoadHistoryOrders_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new AdminCard();
        }
        private void ButtonAddCourier_Click(object sender, RoutedEventArgs e)
        {
           
        }
        private void ButtonAddNewProduct_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
