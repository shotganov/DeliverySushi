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
using System.Windows.Media.Animation;
using DeliverySushi.ViewModel;
using DeliverySushi.View;

namespace DeliverySushi
{
    public partial class MainWindow : Window
    {

        public MainViewModel MainViewModel { get; } = new MainViewModel(); // Глобальная модель

        public MainWindow()
        {
            InitializeComponent();
            var sushiViewModel = new SushiViewModel();
            var cartViewModel = new CartViewModel();
            var setViewModel =  new SetViewModel();
            var addonViewModel = new AddonViewModel();
            var orderViewModel = new OrderViewModel();

            sushiViewModel.CartUpdated += (s, e) => cartViewModel.RefreshCartItems();
            setViewModel.CartUpdated += (s, e) => cartViewModel.RefreshCartItems();
            addonViewModel.CartUpdated += (s, e) => cartViewModel.RefreshCartItems();
            cartViewModel.OrderUpdated += (s, e) => orderViewModel.RefreshOrderItems();

            MainViewModel.SushiViewModel = sushiViewModel;
            MainViewModel.CartViewModel = cartViewModel;
            MainViewModel.SetViewModel = setViewModel;
            MainViewModel.AddonViewModel = addonViewModel;
            MainViewModel.OrderViewModel = orderViewModel;  

            DataContext = MainViewModel; // Устанавливаем глобальный контекст данных
            MainFrame.Navigate(new AuthPage());
        }

    }
}
