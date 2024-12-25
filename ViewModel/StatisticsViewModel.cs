using DeliverySushi.Model;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DeliverySushi.ViewModel
{
    public class StatisticsViewModel : INotifyPropertyChanged
    {
        private DateTime? _selectedDate;
        private SeriesCollection _seriesCollection;
        
        public ICommand ShowStatisticsCommand { get; set; }
        public ICommand LoadNewOrderCommand { get; set; }
        public ICommand LoadHistoryOrdersCommand { get; set; }
        public ICommand AddCourierCommand { get; set; }
        public ICommand AddNewProductCommand { get; set; }
        public ICommand ConfirmOrderCommand { get; set; }
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                UpdateChart();
            }
        }

        private ObservableCollection<DateTime> _availableDates;
        public ObservableCollection<DateTime> AvailableDates
        {
            get => _availableDates;
            set
            {
                _availableDates = value;
                OnPropertyChanged(nameof(AvailableDates));
            }
        }

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                _seriesCollection = value;
                OnPropertyChanged(nameof(SeriesCollection));
            }
        }

        public StatisticsViewModel()
        {
            // Инициализация доступных дат
            LoadAvailableDates();

            LoadNewOrderCommand = new RelayCommand(LoadConfrirmListOrders);
            ConfirmOrderCommand = new RelayCommand(ConfirmOrder);
            LoadHistoryOrdersCommand = new RelayCommand(LoadHistoryOrders);
        }

        private bool _ConfirmButtonVisible;
        public bool ConfirmButtonVisible
        {
            get => _ConfirmButtonVisible;
            set
            {
                _ConfirmButtonVisible = value;
                OnPropertyChanged(nameof(ConfirmButtonVisible));
            }
        }


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


        private async void ConfirmOrder(object parameter)
        {
            if (parameter is Order order)
            {
                using (var context = new sushiContext())
                {
                    var confirmOrder = await context.Order
                        .Where(o => o.status == "В обработке" && o.FK_order_id == order.FK_order_id)
                        .Include(o => o.Order_Item) // Явное включение связанных данных
                        .FirstAsync();

                    confirmOrder.status = "Принят";

                    await context.SaveChangesAsync();

                    LoadConfrirmListOrders(Orders);
                }
            }
        }

        private async void LoadAvailableDates()
        {
            using (var context = new sushiContext())
            {
                // Загружаем даты заказов из базы данных
                var dates = context.Order
                    .Where(o => o.order_date.HasValue)
                    .Select(o => o.order_date.Value)
                    .ToList();

                // Преобразуем даты в компоненты Date
                var availableDate = dates
                    .Select(d => d.Date)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                AvailableDates = new ObservableCollection<DateTime>(availableDate);

                if (AvailableDates.Any())
                {
                    SelectedDate = AvailableDates.First();
                }

            }
        }

        private async void LoadConfrirmListOrders(object parameter)
        {
            using (var context = new sushiContext())
            {
                var availableOrders = await context.Order
                    .Where(o => o.status == "В обработке")
                    .Include(o => o.Order_Item) // Явное включение связанных данных
                    .ToListAsync();

                await loadOrder_items(context, availableOrders);

                ConfirmButtonVisible = true;
                availableOrders.Reverse();
                Orders = new ObservableCollection<Order>(availableOrders);
            }

        }

        private async void LoadHistoryOrders(object parameter)
        {
            using (var context = new sushiContext())
            {
                var HistoryOrders = await context.Order
                    .Where(o => o.status != "В обработке" && o.status != "Не принят")
                    .Include(o => o.Order_Item) // Явное включение связанных данных
                    .ToListAsync();

                await loadOrder_items(context, HistoryOrders);

                ConfirmButtonVisible = false;
                HistoryOrders.Reverse();
                Orders = new ObservableCollection<Order>(HistoryOrders);
            }
        }
        
     

        private async void UpdateChart()
        {
            if (SelectedDate == null)
                return;

            using (var context = new sushiContext())
            {
                // Загружаем заказы для выбранной даты
                var orders = context.Order
                .Where(o => o.order_date.HasValue) // Фильтруем по наличию даты
                .Include(o => o.Order_Item) // Загружаем связанные Order_Item
                .ToList(); // Выполняем запрос к базе данных

                // Фильтруем заказы по выбранной дате в памяти
                var filteredOrders = orders
                    .Where(o => o.order_date.Value.Date == SelectedDate.Value.Date)
                    .ToList();


                // Создаем словарь для хранения данных для диаграммы
                var productData = new Dictionary<string, int>();

                foreach (var order in filteredOrders)
                {
                    foreach (var item in order.Order_Item)
                    {
                        // Определяем тип продукта и связываем данные
                        string productName = "Неизвестный продукт";

                        if (item.product_type == "sushi")
                        {
                            var sushi = context.Sushi.Find(item.FK_product_id);
                            if (sushi != null)
                            {
                                productName = "Роллы " + sushi.name;
                            }
                        }
                        else if (item.product_type == "set")
                        {
                            var set = context.Set.Find(item.FK_product_id);
                            if (set != null)
                            {
                                productName = "Сет " + set.name;
                            }
                        }
                        else if (item.product_type == "addon")
                        {
                            var addon = context.Addon.Find(item.FK_product_id);
                            if (addon != null)
                            {
                                productName = "Дополнения " + addon.name;
                            }
                        }

                        // Добавляем данные в словарь
                        if (productData.ContainsKey(productName))
                        {
                            productData[productName] += item.quantity ?? 0; // Суммируем количество
                        }
                        else
                        {
                            productData[productName] = item.quantity ?? 0;
                        }
                    }
                }

                // Создаем SeriesCollection для диаграммы
                SeriesCollection = new SeriesCollection();

                foreach (var product in productData)
                {
                    SeriesCollection.Add(new PieSeries
                    {
                        Title = product.Key, // Название продукта
                        Values = new ChartValues<int> { product.Value }, // Количество
                        DataLabels = true
                    });
                }             

                OnPropertyChanged(nameof(SeriesCollection));
              
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
