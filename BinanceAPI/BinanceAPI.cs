using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
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

        //public async Task<HttpStatusCode> GetAllTrades()
        //{
        //    using (var request = new HttpRequestMessage(HttpMethod.Get, _url))
        //    {
        //        AddApiKey(request);

        //        request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        //        using (HttpResponseMessage httpResponse = await HttpClient.SendAsync(request))
        //        {
        //            string statusCode = httpResponse.StatusCode.ToString();
        //            //Log.Debug("# HTTP RESPONSE : " + statusCode);
        //            //if (statusCode != "OK")
        //            //    Log.Error(httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult());

        //            return httpResponse.StatusCode;
        //        }
        //    }

        //}

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
