using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DeliverySushi.Model;
using System.Collections.Generic;
using System.Windows;
using System;

namespace DeliverySushi.ViewModel
{
    public class OrderViewModel : INotifyPropertyChanged
    {
       

        private int userId;
        private ObservableCollection<Order> orders;

        public ObservableCollection<Order> Orders
        {
            get => orders;
            set
            {
                orders = value;
                OnPropertyChanged();
            }
        }

        public OrderViewModel()
        {
            Orders = new ObservableCollection<Order>();
        }

        public async void SetId(int id)
        {
            userId = id;
            await LoadOrders();
        }

        public async void RefreshOrderItems()
        {
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            if (userId <= 0)
            {
                Orders.Clear();
                return;
            }

            try
            {
                using (var context = new sushiContext())
                {
                    // Загружаем заказы пользователя
                    var userOrders = await context.Order
                        .Where(o => o.FK_customer_id == userId)
                        .Include(o => o.Order_Item) // Загружаем связанные Order_Item
                        .ToListAsync();

                    foreach (var order in userOrders)
                    {
                        // Загружаем элементы заказа отдельно
                        var orderItems = await context.Order_Item
                            .Where(oi => oi.FK_order_id == order.FK_order_id)
                            .ToListAsync();

                        foreach (var item in orderItems)
                        {
                            // Определяем тип продукта и связываем данные
                            if (item.product_type == "sushi")
                            {
                                var sushi = await context.Sushi.FindAsync(item.FK_product_id);
                                if (sushi != null)
                                {
                                    item.name = "Роллы " + sushi.name;
                                    item.ImageSource = ImageConverter.LoadImage(sushi.image);
                                }
                            }
                            else if (item.product_type == "set")
                            {
                                var set = await context.Set.FindAsync(item.FK_product_id);
                                if (set != null)
                                {
                                    item.name = "Сет " + set.name;
                                    item.ImageSource = ImageConverter.LoadImage(set.image);
                                }
                            }
                            else if (item.product_type == "addon")
                            {
                                var addon = await context.Addon.FindAsync(item.FK_product_id);
                                if (addon != null)
                                {
                                    item.name = addon.name;
                                    item.ImageSource = ImageConverter.LoadImage(addon.image);
                                }
                            }
                        }

                        // Обновляем элементы заказа в заказе
                        order.Order_Item = orderItems;
                    }

                    userOrders.Reverse();
                    // Обновляем коллекцию Orders с полностью загруженными данными
                    Orders = new ObservableCollection<Order>(userOrders);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}