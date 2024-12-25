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
using System.Windows.Shapes;

namespace DeliverySushi.View
{
    /// <summary>
    /// Логика взаимодействия для OrderForm.xaml
    /// </summary>
    public partial class OrderForm : Window
    {
        public string RecipientName { get; private set; }
        public string DeliveryAddress { get; private set; }
        public string PhoneNumber { get; private set; }
        public int OrderSum { get; private set; } // Сумма заказа

        public OrderForm(int orderSum)
        {
            InitializeComponent();
            OrderSum = orderSum; // Устанавливаем сумму заказа
            var mainViewModel = (MainViewModel)Application.Current.MainWindow.DataContext;
           
            DeliveryAddressTextBox.Text = mainViewModel.UserViewModel.Adress;
            PhoneNumberTextBox.Text = mainViewModel.UserViewModel.Phone.ToString();
            RecipientNameTextBox.Text = mainViewModel.UserViewModel.Login;
            DataContext = this; // Устанавливаем DataContext для привязки

            //this.Closing += OrderForm_Closing;
        }

        private void ConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            // Получаем данные из текстовых полей
            RecipientName = RecipientNameTextBox.Text;
            DeliveryAddress = DeliveryAddressTextBox.Text;
            PhoneNumber = PhoneNumberTextBox.Text;

            // Проверяем, что все поля заполнены
            if (string.IsNullOrWhiteSpace(RecipientName) ||
                string.IsNullOrWhiteSpace(DeliveryAddress) ||
                string.IsNullOrWhiteSpace(PhoneNumber))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            // Закрываем окно и возвращаем данные
            DialogResult = true;
            Close();
        }
        //private void OrderForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    // Здесь можно выполнить вашу функцию перед закрытием окна
        //    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

        //    if (result == MessageBoxResult.No)
        //    {
        //        // Если пользователь отказался, отменяем закрытие окна
        //        e.Cancel = true;
        //    }
        //    else
        //    {
        //        // Если пользователь подтвердил, закрываем окно
        //        DialogResult = false; // Устанавливаем DialogResult в false, если окно закрывается без подтверждения заказа
        //    }
        //}
    }
}
