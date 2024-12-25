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
    /// Логика взаимодействия для UserSetControl.xaml
    /// </summary>
    public partial class UserSetControl : UserControl
    {
        public UserSetControl()
        {
            InitializeComponent();
            var mainViewModel = (MainViewModel)Application.Current.MainWindow.DataContext;
            DataContext = mainViewModel.SetViewModel;
        }
    }
}
