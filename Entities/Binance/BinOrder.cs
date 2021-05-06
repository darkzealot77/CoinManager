using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoinManager.Entities.Binance
{
    public partial class BinOrder
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("origQty")]
        public decimal OrigQty { get; set; }

        [JsonProperty("executedQty")]
        public decimal ExecutedQty { get; set; }

        [JsonProperty("cummulativeQuoteQty")]
        public decimal CummulativeQuoteQty { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("timeInForce")]
        public string TimeInForce { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("stopPrice")]
        public decimal StopPrice { get; set; }

        [JsonProperty("icebergQty")]
        public decimal IcebergQty { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("updateTime")]
        public long UpdateTime { get; set; }

        [JsonProperty("isWorking")]
        public bool IsWorking { get; set; }

        [JsonProperty("origQuoteOrderQty")]
        public decimal OrigQuoteOrderQty { get; set; }
    }
}
