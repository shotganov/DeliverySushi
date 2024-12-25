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
    /// Логика взаимодействия для CourierOrderCard.xaml
    /// </summary>
    public partial class CourierOrderCard : UserControl
    {
        public CourierOrderCard()
        {
            InitializeComponent();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {

            if (sender is Button button && button.DataContext is Order order)
            {
                // Получаем DataContext страницы
                var viewModel = (MainViewModel)Application.Current.MainWindow.DataContext;

                // Проверяем текущий текст кнопки
                if (button.Content.ToString() == "Принять заказ")
                {
                    // Принимаем заказ
                    viewModel.CourierViewModel.AcceptOrderCommand.Execute(order);

                }
                else if (button.Content.ToString() == "Отменить заказ")
                {
                    // Отменяем заказ
                    viewModel.CourierViewModel.CancelOrderCommand.Execute(order);

                }
            }
        }

    }
}
