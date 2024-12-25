using DeliverySushi.Model;
using DeliverySushi;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System;
using System.Data.Entity;

namespace DeliverySushi.ViewModel
{
    public class SushiViewModel : INotifyPropertyChanged
{
    public event EventHandler CartUpdated; // Событие для обновления корзины

    private ObservableCollection<Sushi> sushis;
    private int userId;

    public ObservableCollection<Sushi> Sushis
    {
        get => sushis;
        set
        {
            sushis = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddToCartCommand { get; }

        public SushiViewModel()
        {
            AddToCartCommand = new RelayCommand(param =>
            {
                if (param is Sushi selectedSushi)
                {
                    AddToCart(selectedSushi);
                }
            });

            LoadSushi();

        }

        private async void AddToCart(Sushi selectedSushi)
        {
            if (userId <= 0)
            {
                MessageBox.Show("Ошибка: пользователь не авторизован.");
                return;
            }

            if (selectedSushi == null)
            {
                MessageBox.Show("Ошибка: суши не выбраны.");
                return;
            }

            using (var context = new sushiContext())
            {
                var cartItem = await context.Cart_Sushi.FirstOrDefaultAsync(c =>
                    c.FK_sushi_id == selectedSushi.id && c.FK_customer_id == userId);

                if (cartItem != null)
                {
                    cartItem.quantity += 1;
                }
                else
                {
                    var newCartItem = new Cart_Sushi
                    {
                        FK_sushi_id = selectedSushi.id,
                        FK_customer_id = userId,
                        quantity = 1
                    };

                    context.Cart_Sushi.Add(newCartItem);
                }

                await context.SaveChangesAsync();
                CartUpdated?.Invoke(this, EventArgs.Empty);

                MessageBox.Show($"Суши {selectedSushi.name} добавлены в корзину.");
            }
        }

      
        public void SetId(int id)
        {
            userId = id;
        }

    private async void LoadSushi()
    {
        using (var context = new sushiContext())
        {
            var sushiList = await context.Sushi.ToListAsync();

            foreach (var sushi in sushiList)
            {
                sushi.ImageSource = ImageConverter.LoadImage(sushi.image);
            }

            Sushis = new ObservableCollection<Sushi>(sushiList);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
}