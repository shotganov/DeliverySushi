using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DeliverySushi.Model; // Убедитесь, что этот namespace содержит ваши модели

namespace DeliverySushi.ViewModel
{
    public class CourierViewModel : INotifyPropertyChanged
    {
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

        private int courierId; // Идентификатор курьера

        private bool _StatusDeliveryButtonVisible;
        public bool StatusDeliveryButtonVisible
        {
            get => _StatusDeliveryButtonVisible;
            set
            {
                _StatusDeliveryButtonVisible = value;
                OnPropertyChanged(nameof(StatusDeliveryButtonVisible));
            }
        }

        private bool _ConfirmDeliveryButtonVisible;
        public bool ConfirmDeliveryButtonVisible
        {
            get => _ConfirmDeliveryButtonVisible;
            set
            {
                _ConfirmDeliveryButtonVisible = value;
                OnPropertyChanged(nameof(ConfirmDeliveryButtonVisible));
            }
        }

        private string _buttonContent;
        public string ButtonContent
        {
            get => _buttonContent;
            set
            {
                _buttonContent = value;
                OnPropertyChanged(nameof(ButtonContent));
            }
        }

        public ObservableCollection<string> Statuses { get; set; }

        public ICommand CancelOrderCommand { get; set; }
        public ICommand AcceptOrderCommand { get; set; }
        public ICommand LoadWaitOrderCommand { get; set; }
        public ICommand LoadInDeliveryOrderCommand { get; set; }
        public ICommand ConfirmDeliverOrderCommand { get; set; }
        public ICommand LoadDeliveredOrdersCommand { get; set; }
        public ICommand LoadCanceledOrdersCommand { get; set; }

    

        public CourierViewModel(int courierId)
        {
            this.courierId = courierId;

            // Статусы заказа
            Statuses = new ObservableCollection<string>
            {
                "Принят",
                "В пути",
                "Доставлен"
            };

            
            AcceptOrderCommand = new RelayCommand(AcceptOrder);
            CancelOrderCommand = new RelayCommand(CancelOrder);
            LoadWaitOrderCommand = new RelayCommand(LoadWaitOrders);
            LoadInDeliveryOrderCommand = new RelayCommand(LoadInDeliveryOrders);
            ConfirmDeliverOrderCommand = new RelayCommand(ConfirmDeliverOrder);
            LoadDeliveredOrdersCommand = new RelayCommand(LoadDeliveredOrders);
            LoadCanceledOrdersCommand = new RelayCommand(LoadCanceledOrders);
           
            LoadWaitOrders(Orders);
        }

        private async void LoadWaitOrders(object parameter)
        {
            using (var context = new sushiContext())
            {
                var availableOrders = await context.Database
                 .SqlQuery<Order>("SELECT * FROM [Order] WHERE FK_worker_id IS NULL AND status = 'Принят'")
                 .ToListAsync();

                //if (availableOrders.Count == 0)
                //{
                //    MessageBox.Show("Для вас нет доступных заказов.");
                //    return;
                //}

                await loadOrder_items(context, availableOrders);

                Orders = new ObservableCollection<Order>(availableOrders);

                ConfirmDeliveryButtonVisible = false;
                StatusDeliveryButtonVisible = true; 
                ButtonContent = "Принять заказ";
            }
        }


        private async void LoadInDeliveryOrders(object parameter)
        {
            using (var context = new sushiContext())
            {
                var availableOrders = await context.Order
                    .Where(o => o.status == "В пути" && o.FK_worker_id == courierId)
                    .Include(o => o.Order_Item) // Явное включение связанных данных
                    .ToListAsync();

                await loadOrder_items(context, availableOrders);

                availableOrders.Reverse();
                Orders = new ObservableCollection<Order>(availableOrders);

                ConfirmDeliveryButtonVisible = true;
                StatusDeliveryButtonVisible = true;
                ButtonContent = "Отменить заказ";
            }

        }


        private async void LoadDeliveredOrders(object parameter)
        {
            using(var context = new sushiContext())
            {
                var DeliveredOrders = await context.Order.Where(o => o.FK_worker_id == courierId && o.status == "Доставлен").Include(o => o.Order_Item).ToListAsync();

                await loadOrder_items(context, DeliveredOrders);

                DeliveredOrders.Reverse();
                Orders = new ObservableCollection<Order>(DeliveredOrders);

                ConfirmDeliveryButtonVisible = false;
                StatusDeliveryButtonVisible = false;
            }
        }

        private async void LoadCanceledOrders(object parameter)
        {
            using (var context = new sushiContext())
            {
                var CanceledOrders = await context.Order.Where(o => o.FK_worker_id == courierId && o.status == "Отменен").Include(o => o.Order_Item).ToListAsync();

                await loadOrder_items(context, CanceledOrders);

                CanceledOrders.Reverse();
                Orders = new ObservableCollection<Order>(CanceledOrders);

                ConfirmDeliveryButtonVisible = false;
                StatusDeliveryButtonVisible = false;
            }
        }
        private async void AcceptOrder(object parameter)
        {
            if (parameter is Order order)
            {
                using (var context = new sushiContext())
                {
                    var dbOrder = await context.Order.FirstOrDefaultAsync(c => c.FK_order_id == order.FK_order_id);
                    MessageBox.Show("Вы приняли заказ!");
                    if (dbOrder != null)
                    {
                        // Обновляем FK_courier_id
                        dbOrder.FK_worker_id = courierId;
                        dbOrder.status = "В пути";

                        await context.SaveChangesAsync();

                        LoadWaitOrders(Orders);
                    }
                }
            }
        }

        private async void CancelOrder(object parameter)
        {
            if (parameter is Order order)
            {
                using (var context = new sushiContext())
                {
                    // Находим заказ в базе данных
                    var dbOrder = await context.Order.FirstOrDefaultAsync(c => c.FK_order_id == order.FK_order_id);
                    if (dbOrder != null)
                    {
                        dbOrder.status = "Отменен";

                        await context.SaveChangesAsync();

                        LoadInDeliveryOrders(Orders);
                    }
                }
            }
        }

        private async void ConfirmDeliverOrder(object parameter)
        {
            if (parameter is Order order)
            {
                using (var context = new sushiContext())
                {
                    // Находим заказ в базе данных
                    var dbOrder = await context.Order.FirstOrDefaultAsync(c => c.FK_order_id == order.FK_order_id);
                    if (dbOrder != null)
                    {
                       
                        dbOrder.status = "Доставлен";

                        // Сохраняем изменения
                        await context.SaveChangesAsync();

                        LoadInDeliveryOrders(Orders);
                    }
                }
            }
        }

        private async Task loadOrder_items(sushiContext context, List<Order> orders)
        {
            foreach (var order in orders)
            {
                // Загружаем элементы заказа
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
                            item.name = "Дополнения " + addon.name;
                            item.ImageSource = ImageConverter.LoadImage(addon.image);
                        }
                    }

                    
                }

                order.Order_Item = orderItems;

            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}