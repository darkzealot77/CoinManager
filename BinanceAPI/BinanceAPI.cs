using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
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
        }

        public async Task<HttpStatusCode> GetHistoricalTrades()
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, _url + "/api/v3/historicalTrades?symbol=ADAUSDT"))
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
            string parameters = "timestamp=" + Math.Truncate(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            string hash = GetHash(parameters);

            using (var request = new HttpRequestMessage(HttpMethod.Get, _url + "/api/v3/account?" + parameters + "&signature=" + hash))
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

        public async Task<HttpStatusCode> GetAllTrades()
        {
            string parameters = "symbol=EURUSDT" + "&timestamp=" + Math.Truncate(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            string hash = GetHash(parameters);

            using (var request = new HttpRequestMessage(HttpMethod.Get, _url + "/api/v3/allOrders?" + parameters + "&signature=" + hash))
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

        public String GetHash(String text)
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

    }
}
