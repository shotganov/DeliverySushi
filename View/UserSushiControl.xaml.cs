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
using DeliverySushi.ViewModel;


namespace DeliverySushi.View
{
    /// <summary>
    /// Логика взаимодействия для UserMainControl.xaml
    /// </summary>
    public partial class UserSushiControl : UserControl
    {
        public UserSushiControl()
        {
            InitializeComponent();
            var mainViewModel = (MainViewModel)Application.Current.MainWindow.DataContext;
            DataContext = mainViewModel.SushiViewModel;
        }
    }
}
