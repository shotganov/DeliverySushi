using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows.Input;
using DeliverySushi.Model;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using DeliverySushi.View;

namespace DeliverySushi.ViewModel
{
    public class CartViewModel : INotifyPropertyChanged
    {
        public event EventHandler OrderUpdated; // Событие для обновления корзины

        private int userId;

        private ObservableCollection<object> cartItems; // Для хранения и суши, сетов и дополений
        public ObservableCollection<object> CartItems
        {
            get => cartItems;
            set
            {
                cartItems = value;
                OnPropertyChanged();
            }
        }

        private int sum;
        public int Sum
        {
            get => sum;
            set
            {
                sum = value;
                OnPropertyChanged();
            }
        }

        public async Task<int> GetSum()
        {
            return sum;
        }

        public ICommand MakeOrderCommand { get; private set; }
        public ICommand IncreaseQuantityCommand { get; private set; }
        public ICommand DecreaseQuantityCommand { get; private set; }

        public CartViewModel()
        {
            MakeOrderCommand = new RelayCommand(MakeOrderForm);
            IncreaseQuantityCommand = new RelayCommand(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(DecreaseQuantity);
        }

        private void MakeOrderForm(object parameter)
        {
            if (CartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }

            // Открываем форму заказа через Dispatcher
            Application.Current.Dispatcher.Invoke(() =>
            {
                var orderForm = new OrderForm(Sum);
                bool? result = orderForm.ShowDialog();

                if (result == true)
                {
                    string recipientName = orderForm.RecipientName;
                    string deliveryAddress = orderForm.DeliveryAddress;
                    string phoneNumber = orderForm.PhoneNumber;
                    if (CheckInputData(recipientName, deliveryAddress, phoneNumber))
                    {
                        MakeOrder(recipientName, deliveryAddress, phoneNumber);
                        RefreshCartItems();
                    }

                }
            });
        }

        private bool CheckInputData(string recipientName, string deliveryAddress, string phoneNumber )
        {
            if (string.IsNullOrWhiteSpace(recipientName) || string.IsNullOrWhiteSpace(deliveryAddress) || phoneNumber.Length != 11)
            {
                MessageBox.Show("Данные не введены или введены некорректно");
                return false;
                
            }
            else
            {
                return true;
            }
        }
        public async void SetId(int id)
        {
            userId = id;
            await LoadCartItems();
        }

        public async void RefreshCartItems()
        {
            await LoadCartItems();
        }
           public async void MakeOrder(string name, string adress, string phone)
            {
                using (var context = new sushiContext())
                {
                    // Удаляем элементы из корзины
                    var cartSushiItems = await context.Cart_Sushi
                        .Where(cart => cart.FK_customer_id == userId)
                        .ToListAsync();

                    var cartSetItems = await context.Cart_Set
                        .Where(cart => cart.FK_customer_id == userId)
                        .ToListAsync();

                    var cartAddonItems = await context.Cart_Addon
                        .Where(cart => cart.FK_customer_id == userId)
                        .ToListAsync();

                    context.Cart_Sushi.RemoveRange(cartSushiItems);
                    context.Cart_Set.RemoveRange(cartSetItems);
                    context.Cart_Addon.RemoveRange(cartAddonItems);

                var numOrder = await context.Order
                    .OrderByDescending(o => o.FK_order_id)
                    .Select(o => o.FK_order_id)
                    .FirstOrDefaultAsync();

                    // Создаем новый заказ
                    var newOrder = new Order
                    {
                        FK_customer_id = userId,
                        order_date = DateTime.Now,
                        status = "В обработке",
                        total_price = Sum,
                        FK_order_id = numOrder + 1,
                        adress = adress,
                        name = name,
                        phone = long.Parse(phone)
                    };

                    context.Order.Add(newOrder);
                    await context.SaveChangesAsync();

                    // Создаем Order_Item для каждого элемента в корзине
                    foreach (var item in CartItems)
                    {
                        if (item is Sushi sushi)
                        {
                            var orderItem = new Order_Item
                            {
                                FK_order_id = newOrder.FK_order_id,
                                FK_product_id = sushi.id,
                                product_type = "sushi",
                                quantity = sushi.quantity,
                                price = sushi.price
                            };
                            context.Order_Item.Add(orderItem);
                        }
                        else if (item is Set set)
                        {
                            var orderItem = new Order_Item
                            {
                                FK_order_id = newOrder.FK_order_id,
                                FK_product_id = set.id,
                                product_type = "set",
                                quantity = set.quantity,
                                price = set.price
                            };
                            context.Order_Item.Add(orderItem);
                        }
                        else if (item is Addon addon)
                        {
                            var orderItem = new Order_Item
                            {
                                FK_order_id = newOrder.FK_order_id,
                                FK_product_id = addon.id,
                                product_type = "addon",
                                quantity = addon.quantity,
                                price = addon.price
                            };
                            context.Order_Item.Add(orderItem);
                        }
                    }

                    await context.SaveChangesAsync();
                    OrderUpdated?.Invoke(this, EventArgs.Empty);
                    // Очищаем корзину в интерфейсе
                    CartItems = new ObservableCollection<object>();
                    Sum = 0;
                }
        }
        private async Task LoadCartItems()
        {
            if (userId <= 0)
            {
                CartItems = new ObservableCollection<object>();
                Sum = 0; // Устанавливаем сумму в 0
                return;
            }

            using (var context = new sushiContext())
            {
                var combinedItems = new ObservableCollection<object>();
                Sum = 0; // Сбрасываем сумму

                // Загружаем суши из Cart_Sushi
                List<Cart_Sushi> sushiCartItems = await context.Cart_Sushi
                    .Where(cart => cart.FK_customer_id == userId)
                    .Include(cart => cart.Sushi)
                    .ToListAsync();

                foreach (var cartItem in sushiCartItems)
                {
                    if (cartItem.Sushi != null)
                    {
                        cartItem.Sushi.ImageSource = ImageConverter.LoadImage(cartItem.Sushi.image);
                        cartItem.Sushi.quantity = cartItem.quantity;
                        cartItem.Sushi.price = cartItem.Sushi.price * cartItem.quantity;
                        combinedItems.Add(cartItem.Sushi);
                        Sum += cartItem.Sushi.price; // Увеличиваем сумму
                    }
                }

                // Загружаем сеты из Cart_Set
                List<Cart_Set> setCartItems = await context.Cart_Set
                    .Where(cart => cart.FK_customer_id == userId)
                    .Include(cart => cart.Set)
                    .ToListAsync();

                foreach (var cartItem in setCartItems)
                {
                    if (cartItem.Set != null)
                    {
                        cartItem.Set.ImageSource = ImageConverter.LoadImage(cartItem.Set.image);
                        cartItem.Set.quantity = cartItem.quantity;
                        cartItem.Set.price = cartItem.Set.price * cartItem.quantity;
                        combinedItems.Add(cartItem.Set);
                        Sum += cartItem.Set.price; // Увеличиваем сумму
                    }
                }

                // Загружаем дополнения из Cart_Addon
                List<Cart_Addon> addonCartItems = await context.Cart_Addon
                    .Where(cart => cart.FK_customer_id == userId)
                    .Include(cart => cart.Addon)
                    .ToListAsync();

                foreach (var cartItem in addonCartItems)
                {
                    if (cartItem.Addon != null)
                    {
                        cartItem.Addon.ImageSource = ImageConverter.LoadImage(cartItem.Addon.image);
                        cartItem.Addon.quantity = cartItem.quantity;
                        cartItem.Addon.price = cartItem.Addon.price * cartItem.quantity;
                        combinedItems.Add(cartItem.Addon);
                        Sum += cartItem.Addon.price; // Увеличиваем сумму
                    }
                }

                CartItems = combinedItems;
            }
        }

        private void IncreaseQuantity(object parameter)
        {
            if (parameter is Sushi sushi)
            {
                UpdateQuantity(sushi, true);
            }
            else if (parameter is Set set)
            {
                UpdateQuantity(set, true);
            }
            else if (parameter is Addon addon)
            {
                UpdateQuantity(addon, true);
            }
        }

        private void DecreaseQuantity(object parameter)
        {
            if (parameter is Sushi sushi)
            {
                UpdateQuantity(sushi, false);
            }
            else if (parameter is Set set)
            {
                UpdateQuantity(set, false);
            }
            else if (parameter is Addon addon)
            {
                UpdateQuantity(addon, false);
            }
        }

        private async void UpdateQuantity(object item, bool increase)
        {
            using (var context = new sushiContext())
            {
                if (item is Sushi sushi)
                {
                    var cartItem = await context.Cart_Sushi.FirstOrDefaultAsync(c =>
                        c.FK_customer_id == userId && c.FK_sushi_id == sushi.id);

                    if (cartItem != null)
                    {
                        if (increase)
                        {
                            cartItem.quantity += 1;
                        }
                        else if (cartItem.quantity > 1)
                        {
                            cartItem.quantity -= 1;
                        }
                        else
                        {
                            context.Cart_Sushi.Remove(cartItem);
                        }
                    }
                }
                else if (item is Set set)
                {
                    var cartItem = await context.Cart_Set.FirstOrDefaultAsync(c =>
                        c.FK_customer_id == userId && c.FK_set_id == set.id);

                    if (cartItem != null)
                    {
                        if (increase)
                        {
                            cartItem.quantity += 1;
                        }
                        else if (cartItem.quantity > 1)
                        {
                            cartItem.quantity -= 1;
                        }
                        else
                        {
                            context.Cart_Set.Remove(cartItem);
                        }
                    }
                }
                else if (item is Addon addon)
                {
                    var cartItem = await context.Cart_Addon.FirstOrDefaultAsync(c =>
                        c.FK_customer_id == userId && c.FK_addon_id == addon.id);

                    if (cartItem != null)
                    {
                        if (increase)
                        {
                            cartItem.quantity += 1;
                        }
                        else if (cartItem.quantity > 1)
                        {
                            cartItem.quantity -= 1;
                        }
                        else
                        {
                            context.Cart_Addon.Remove(cartItem);
                        }
                    }
                }

                await context.SaveChangesAsync();
                RefreshCartItems();
            }
        }

     
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}