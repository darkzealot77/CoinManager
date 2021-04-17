using CoinManager.Entities;
using CoinManager.Entities.Binance;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APICall
{
    public class BinanceAPI
    {
        HttpClient HttpClient { get; set; }

        IConfiguration Configuration { get; set; }

        private string _apiKey;
        private string _secret;
        private string _url;

        public BinanceAPI(HttpClient httpClient, IConfiguration configuration)
        {
            HttpClient = httpClient;
            Configuration = configuration;

            _url = GetConfiguration("Binance:Url");
            _apiKey = GetConfiguration("Binance:ApiKey");
            _secret = GetConfiguration("Binance:Secret");

            httpClient.BaseAddress = new Uri(_url);
        }

        public async Task<HttpStatusCode> GetHistoricalTrades()
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/v3/historicalTrades?symbol=ADAUSDT"))
                {
                    AddApiKey(request);

                    using (HttpResponseMessage httpResponse = HttpClient.Send(request))
                    {
                        string statusCode = httpResponse.StatusCode.ToString();
                        //Log.Debug("# HTTP RESPONSE : " + statusCode);
                        //if (statusCode != "OK")
                        //    Log.Error(httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                        var strInfo = await httpResponse.Content.ReadAsStringAsync();

                        return httpResponse.StatusCode;
                    }
                }
            }
            catch (Exception ex)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<HttpStatusCode> GetAccountInformation()
        {
            string parameters = await AddServerTime(false);
            string allParameters = GetParametersWithHash(parameters);

            using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/v3/account?" + allParameters))
            {
                AddApiKey(request);

                using (HttpResponseMessage httpResponse = HttpClient.Send(request))
                {
                    string statusCode = httpResponse.StatusCode.ToString();

                    var strInfo = await httpResponse.Content.ReadAsStringAsync();

                    return httpResponse.StatusCode;
                }
            }

        }

        public async Task<ReturnObject<List<BinOrder>>> GetAllTrades(string symbol)
        {
            string parameters = "symbol=" + symbol + await AddServerTime();
            string allParameters = GetParametersWithHash(parameters);

            return await GetTrades(allParameters);
        }

        public async Task<ReturnObject<List<BinOrder>>> GetLastTrades(string symbol, long lastOrderId)
        {
            string parameters = "symbol=" + symbol + "&orderId=" + lastOrderId + await AddServerTime();
            string allParameters = GetParametersWithHash(parameters);

            return await GetTrades(allParameters);
        }

        public async Task<ReturnObject<List<BinOrder>>> GetTrades(string allParameters)
        { 
            using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/v3/allOrders?" + allParameters))
            {
                AddApiKey(request);

                using (HttpResponseMessage httpResponse = HttpClient.Send(request))
                {
                    if (httpResponse.StatusCode.ToString() == "418" || httpResponse.StatusCode.ToString() == "429")
                    {
                        string strSecond = ((List<string>)httpResponse.Headers.GetValues("Retry-After")).FirstOrDefault();
                        int retryAfterS = int.Parse(strSecond);

                        return new ReturnObject<List<BinOrder>>(httpResponse.StatusCode, null, retryAfterS);
                    }
                    else if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        var strInfo = await httpResponse.Content.ReadAsStringAsync();
                        return new ReturnObject<List<BinOrder>>(httpResponse.StatusCode, null, 10) { Error = strInfo };
                    }
                    else
                    {
                        var strInfo = await httpResponse.Content.ReadAsStringAsync();
                        List<BinOrder> allOrders = JsonConvert.DeserializeObject<List<BinOrder>>(strInfo);
                        
                        return new ReturnObject<List<BinOrder>>(httpResponse.StatusCode, allOrders);
                    }
                }
            }
        }

        public async Task<ReturnObject<BinMarketPrice>> GetMarketPrice(string symbol)
        {
            string parameters = "symbol=" + symbol;

            using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/v3/avgPrice?" + parameters))
            {
                using (HttpResponseMessage httpResponse = HttpClient.Send(request))
                {
                    if (httpResponse.StatusCode.ToString() == "418" || httpResponse.StatusCode.ToString() == "429")
                    {
                        string strSecond = ((List<string>)httpResponse.Headers.GetValues("Retry-After")).FirstOrDefault();
                        int retryAfterS = int.Parse(strSecond);

                        return new ReturnObject<BinMarketPrice>(httpResponse.StatusCode, null, retryAfterS);
                    }
                    else if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        var strInfo = await httpResponse.Content.ReadAsStringAsync();
                        return new ReturnObject<BinMarketPrice>(httpResponse.StatusCode, null, 10) { Error = strInfo };
                    }
                    else
                    {
                        var strInfo = await httpResponse.Content.ReadAsStringAsync();
                        BinMarketPrice price = JsonConvert.DeserializeObject<BinMarketPrice>(strInfo);

                        return new ReturnObject<BinMarketPrice>(httpResponse.StatusCode, price);
                    }
                }
            }

        }

        #region Méthodes privées
        #region Gestion du Temps
        private async Task<string> AddServerTime(bool add = true)
        {
            return (add ? "&" : string.Empty) + "recvWindow=5000&timestamp=" + await GetServerTime();
        }

        private long GetLocalTime()
        {
            return (long)Math.Truncate(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
        }

        private async Task<string> GetServerTime ()
        {
            var binanceTime = await HttpClient.GetAsync("/api/v1/time");
            if (binanceTime.IsSuccessStatusCode)
            {
                var serverTime = await binanceTime.Content.ReadAsStringAsync();
                BinServerTime binanceServerTime = JsonConvert.DeserializeObject<BinServerTime>(serverTime);

                return binanceServerTime.serverTime.ToString();
            }

            return GetLocalTime().ToString();
        }
        #endregion

        private string GetParametersWithHash(string parameters)
        {
            string hash = GetHash(parameters);

            return parameters + "&signature=" + hash;
        }

        private String GetHash(String text)
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(_secret);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private void AddApiKey(HttpRequestMessage message)
        {
            message.Headers.Add("X-MBX-APIKEY", _apiKey);
        }

        private string GetConfiguration(string key)
        {
            string val = Configuration[key];

            if (String.IsNullOrEmpty(val))
                throw new Exception($"Pas de {key} trouvé dans le fichier de configuration");

            return val;
        }
        #endregion
    }
}
