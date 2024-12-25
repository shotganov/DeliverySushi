using DeliverySushi.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DeliverySushi.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public SushiViewModel SushiViewModel { get;  set; }
        public SetViewModel SetViewModel { get; set; }

        public UserViewModel UserViewModel { get; set; }

        public CartViewModel CartViewModel { get; set; }

        public AddonViewModel AddonViewModel { get; set; }

        public WorkerViewModel WorkerViewModel { get; set; }

        public OrderViewModel OrderViewModel { get; set; }

        public CourierViewModel CourierViewModel { get; set; }

        public StatisticsViewModel StatisticsViewModel { get; set; }
        public MainViewModel()
        {
            
        }

        // Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
