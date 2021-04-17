using APICall;
using CoinManager.Component;
using CoinManager.Entities;
using CoinManager.Entities.Binance;
using CoinManager.Services;
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
        FileOrdersService _fileStockService;
        IConfiguration Configuration;
        List<string> SymbolList { get; set; } = new List<string>();

        public DicoOrders AllOrders { get; set; }
        public Dictionary<string, SymbolRecap> AllRecap { get; set; } = new Dictionary<string, SymbolRecap>();

        public MainWindow(BinanceAPI binanceAPI, FileOrdersService fileStockService, IConfiguration configuration)
        {
            InitializeComponent();

            _binanceAPI = binanceAPI;
            Configuration = configuration;
            _fileStockService = fileStockService;

            List<KeyValuePair<string, string>> sectionValues = configuration.GetSection("SymbolList")
                    .AsEnumerable()
                    .Where(p => p.Value != null)
                    .ToList();

            foreach (var pair in sectionValues)
            {
                SymbolList.Add(pair.Value);
                AllRecap.Add(pair.Value, new SymbolRecap());
            }

            AllOrders = _fileStockService.Load().GetAwaiter().GetResult();
            if (AllOrders == null)
                AllOrders = new DicoOrders();
        }

        private async void ButtonOrders_Click(object sender, RoutedEventArgs e)
        {
            await GetTrades();

            await SaveTrades();

            Calculate();

            await GetMarketPrice();

            ShowRecap();
        }

        private async Task GetTrades()
        {
            foreach (string symbol in SymbolList)
            {
                ReturnObject<List<BinOrder>> returnObject;

                // Récupération tous les ordres ou les derniers seulement
                if (AllOrders != null)
                {
                    if (AllOrders.ContainsKey(symbol))
                        returnObject = await _binanceAPI.GetLastTrades(symbol, AllOrders[symbol].Select(x => x.Key).Max());
                    else
                        returnObject = await _binanceAPI.GetAllTrades(symbol);
                }
                else
                    returnObject = await _binanceAPI.GetAllTrades(symbol);

                if (returnObject.RetryAfter > 0)
                {
                    DisableAllRequest();
                    break;
                }

                if (returnObject.Code == HttpStatusCode.OK)
                {
                    if (!AllOrders.ContainsKey(symbol))
                        AllOrders.Add(symbol, new Dictionary<long, BinOrder>());

                    foreach (var order in returnObject.Value)
                    {
                        if (!AllOrders[order.Symbol].ContainsKey(order.OrderId))
                            AllOrders[order.Symbol].Add(order.OrderId, order);
                    }
                }
            }
        }

        private async Task SaveTrades()
        {
            await _fileStockService.Save(AllOrders);
        }

        private async void ButtonPrices_Click(object sender, RoutedEventArgs e)
        {
            await GetMarketPrice();

            Calculate();

            ShowRecap();
        }

        private async Task GetMarketPrice()
        {
            foreach (var pair in AllOrders)
            {
                var returnObject = await _binanceAPI.GetMarketPrice(pair.Key);

                if (returnObject.RetryAfter > 0)
                {
                    DisableAllRequest();
                    break;
                }

                if (returnObject.Code == HttpStatusCode.OK)
                {
                    SymbolRecap symbolRecap = AllRecap[pair.Key];

                    symbolRecap.Cours = returnObject.Value.price;
                    symbolRecap.Difference = (symbolRecap.Cours * symbolRecap.Nombre) - (symbolRecap.Moyenne * symbolRecap.Nombre);
                }
            }
        }

        private void ShowRecap()
        {
            stackMain.Children.Clear();
            stackMain.Children.Add(new TitreValeurCrypto());

            foreach (var pair in AllRecap)
            {
                ValeurCrypto valeurCrypto = new ValeurCrypto(pair.Key, pair.Value);
                stackMain.Children.Add(valeurCrypto);
            }
        }

        private void Calculate()
        {
            foreach (var pair in AllOrders)
            {
                SymbolRecap symbolRecap = AllRecap[pair.Key];
                symbolRecap.Nombre = 0;
                symbolRecap.Valeur = 0;

                foreach (var pairOrder in pair.Value)
                {
                    BinOrder binOrder = pairOrder.Value;

                    if (binOrder.Side == "BUY" && binOrder.Status == "FILLED")
                    {
                        symbolRecap.Nombre += double.Parse(binOrder.ExecutedQty.Replace(".", ","));
                        symbolRecap.Valeur += double.Parse(binOrder.CummulativeQuoteQty.Replace(".", ","));
                    }
                    else if (binOrder.Side == "SELL" && binOrder.Status == "FILLED")
                    {
                        symbolRecap.Nombre -= double.Parse(binOrder.ExecutedQty.Replace(".", ","));
                        symbolRecap.Valeur -= double.Parse(binOrder.CummulativeQuoteQty.Replace(".", ","));
                    }
                }
            }
        }

        private void DisableAllRequest()
        {
            ButtonOrders.IsEnabled = false;
            ButtonPrices.IsEnabled = false;
        }
    }
}
