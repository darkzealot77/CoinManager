using CoinManager.Entities;
using CoinManager.Entities.Binance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CoinManager.Services
{
    public class FileOrdersService
    {
        public string Path { get; set; } = "StockFile";

        public FileOrdersService()
        {

        }

        public async Task Save (DicoOrders stockValue)
        {
            string strStock = JsonConvert.SerializeObject(stockValue);

            await File.WriteAllTextAsync(Path, strStock);
        }

        public async Task<DicoOrders> Load()
        {
            if (!File.Exists(Path))
                using (File.Create(Path)) { }

            string strStock = await File.ReadAllTextAsync(Path).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<DicoOrders>(strStock);
        }
    }
}
