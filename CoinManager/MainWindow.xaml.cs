using APICall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace CoinBase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BinanceAPI _binanceAPI;

        public MainWindow(BinanceAPI binanceAPI)
        {
            InitializeComponent();

            _binanceAPI = binanceAPI;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var code = _binanceAPI.GetAccountInformation().GetAwaiter().GetResult();

            if (code == HttpStatusCode.TooManyRequests)
                DisableAllRequest();
        }

        private void DisableAllRequest()
        {
            ButtonCall.IsEnabled = false;
        }
    }
}
