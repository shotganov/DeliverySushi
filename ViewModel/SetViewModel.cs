using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Diagnostics;
using DeliverySushi.Model;
using System.Windows;
using System.Windows.Input;
using DeliverySushi;

namespace DeliverySushi.ViewModel
{
    public class SetViewModel : INotifyPropertyChanged
    {
        public event EventHandler CartUpdated; // Событие для обновления корзины

        private ObservableCollection<Set> sets;
        private int userId;

        public ObservableCollection<Set> Sets
        {
            get => sets;
            set
            {
                sets = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddToCartCommand { get; }
        public SetViewModel()
        {
            AddToCartCommand = new RelayCommand(param =>
            {
                if (param is Set selectedSet)
                {
                    AddToCart(selectedSet);
                }
            });

          
            LoadSets();
        }

        public async void AddToCart(Set selectedSet)
        {
            if (userId <= 0)
            {
                MessageBox.Show("Ошибка: пользователь не авторизован.");
                return;
            }

            if (selectedSet == null)
            {
                MessageBox.Show("Ошибка: сет не выбран.");
                return;
            }

            using (var context = new sushiContext())
            {
                var cartItem = await context.Cart_Set.FirstOrDefaultAsync(c =>
                    c.FK_set_id == selectedSet.id && c.FK_customer_id == userId);

                if (cartItem != null)
                {
                    cartItem.quantity += 1;
                }
                else
                {
                    var newCartItem = new Cart_Set
                    {
                        FK_set_id = selectedSet.id,
                        FK_customer_id = userId,
                        quantity = 1
                    };

                    context.Cart_Set.Add(newCartItem);
                }

                await context.SaveChangesAsync();
                CartUpdated?.Invoke(this, EventArgs.Empty);

                MessageBox.Show($"Сет {selectedSet.name} добавлен в корзину.");
            }
        }
      
        public void SetId(int id)
        {
            userId = id;
        }

        private async void LoadSets()
        {
            using (var context = new sushiContext())
            {
                var setList = await context.Set.ToListAsync();

                foreach (var set in setList)
                {
                    set.ImageSource = ImageConverter.LoadImage(set.image);
                }

                Sets = new ObservableCollection<Set>(setList);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
