using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data.Entity;
using System.Threading.Tasks;
using DeliverySushi.Model;
using System.Windows.Media.Imaging;
using System;
using System.Windows;
using System.Windows.Input;

namespace DeliverySushi.ViewModel
{
    public class AddonViewModel : INotifyPropertyChanged
    {
        public event EventHandler CartUpdated; // Событие для обновления корзины

        private int userId;
        private ObservableCollection<Addon> addons;

        public ICommand AddToCartCommand { get; }

        public ObservableCollection<Addon> Addons
        {
            get => addons;
            set
            {
                addons = value;
                OnPropertyChanged();
            }
        }

        public AddonViewModel()
        {
            AddToCartCommand = new RelayCommand(param =>
            {
                if (param is Addon selectedAddon)
                {
                    AddToCart(selectedAddon);
                }
            });

           
            LoadAddons();
        }
        public async void AddToCart(Addon selectedAddon)
        {
            if (userId <= 0)
            {
                MessageBox.Show("Ошибка: пользователь не авторизован.");
                return;
            }

            if (selectedAddon == null)
            {
                MessageBox.Show("Ошибка: дополнение не выбрано.");
                return;
            }

            using (var context = new sushiContext())
            {
                var cartItem = await context.Cart_Addon.FirstOrDefaultAsync(c =>
                    c.FK_addon_id == selectedAddon.id && c.FK_customer_id == userId);

                if (cartItem != null)
                {
                    cartItem.quantity += 1; // Увеличиваем количество
                }
                else
                {
                    var newCartItem = new Cart_Addon
                    {
                        FK_addon_id = selectedAddon.id,
                        FK_customer_id = userId,
                        quantity = 1
                    };

                    context.Cart_Addon.Add(newCartItem);
                }

                await context.SaveChangesAsync();
                CartUpdated?.Invoke(this, EventArgs.Empty);

                MessageBox.Show($"Дополнение {selectedAddon.name} добавлено в корзину.");
            }
        }

      

        public void SetId(int id)
        {
            userId = id;
        }
        private async void LoadAddons()
        {
            using (var context = new sushiContext())
            {
                var addonList = await context.Addon.ToListAsync();
           

                foreach (var addon in addonList)
                {
                    addon.ImageSource = ImageConverter.LoadImage(addon.image);
                }
                
                Addons = new ObservableCollection<Addon>(addonList);

               
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
