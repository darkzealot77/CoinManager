using APICall;
using CoinManager.Component;
using CoinManager.Entities;
using Microsoft.Extensions.Configuration;
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
        IConfiguration Configuration;
        List<string> SymbolList { get; set; } = new List<string>();

        public MainWindow(BinanceAPI binanceAPI, IConfiguration configuration)
        {
            InitializeComponent();

            _binanceAPI = binanceAPI;
            Configuration = configuration;

            List<KeyValuePair<string, string>> sectionValues = configuration.GetSection("SymbolList")
                    .AsEnumerable()
                    .Where(p => p.Value != null)
                    .ToList();

            foreach (var pair in sectionValues)
            {
                SymbolList.Add(pair.Value);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, AllOrders> dicoOrders = new Dictionary<string, AllOrders>();

            foreach (string symbol in SymbolList)
            {
                var returnObject = await _binanceAPI.GetAllTrades(symbol);

                if (returnObject.RetryAfter > 0)
                {
                    DisableAllRequest();
                    break;
                }

                if (returnObject.Code == HttpStatusCode.OK)
                { 
                    dicoOrders.Add(symbol, returnObject.Value);
                }
                //else
                //{
                //    returnObject.Error;
                //}
            }

            Calculate(dicoOrders);

            Show(dicoOrders);
        }

        private void Show(Dictionary<string, AllOrders> dicoOrders)
        {
            stackMain.Children.Clear();

            foreach (var pair in dicoOrders)
            {
                ValeurCrypto valeurCrypto = new ValeurCrypto(pair.Key, pair.Value);
                stackMain.Children.Add(valeurCrypto);
            }
        }

        private void Calculate(Dictionary<string, AllOrders> dicoOrders)
        {
            foreach (var pair in dicoOrders)
            {
                AllOrders allOrders = pair.Value;

                foreach (var order in allOrders.AllordersList)
                {
                    if (order.Side == "BUY" && order.Status == "FILLED")
                    {
                        allOrders.Nombre += double.Parse(order.ExecutedQty.Replace(".", ","));
                        allOrders.Valeur += double.Parse(order.CummulativeQuoteQty.Replace(".", ","));
                    }
                }
            }
        }

        private void DisableAllRequest()
        {
            ButtonCall.IsEnabled = false;
        }
    }
}
