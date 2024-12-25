//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DeliverySushi.Model;

//namespace DeliverySushi.ViewModel
//{
//    public class AuthWindowViewModel : INotifyPropertyChanged
//    {
//        public event PropertyChangedEventHandler PropertyChanged = delegate { };

//        //Fields
//        private IMainWindowsCodeBehind _MainCodeBehind;

//        //ctor
//        public AuthWindowViewModel(IMainWindowsCodeBehind codeBehind)
//        {
//            if (codeBehind == null) throw new ArgumentNullException(nameof(codeBehind));

//            _MainCodeBehind = codeBehind;
//        }

//        //Properties

//        /// <summary>
//        /// Введенная строка в TextBox
//        /// </summary>
//        private string _InputText;
//        public string InputText
//        {
//            get { return _InputText; }
//            set
//            {
//                _InputText = value;
//                PropertyChanged(this, new PropertyChangedEventArgs(nameof(InputText)));
//            }
//        }


//        //Commands

//        /// <summary>
//        /// Сообщение пользователю
//        /// </summary>
//        //private RelayCommand _ShowMessageCommand;
//        //public RelayCommand ShowMessageCommand
//        //{
//        //    get
//        //    {
//        //        return _ShowMessageCommand = _ShowMessageCommand ??
//        //          new RelayCommand(OnShowMessage, CanShowMessage);
//        //    }
//        //}
//        private bool CanShowMessage()
//        {
//            return true;
//        }
       
//    }
//}
