using CoinManager.Entities.Binance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinManager.Entities
{
    public class OrderGridRow
    {
        public string Symbol { get; set; }

        public long OrderId { get; set; }

        public string Price { get; set; }

        public string OrigQty { get; set; }

        public string ExecutedQty { get; set; }

        public string CummulativeQuoteQty { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }

        public string Side { get; set; }

        public string TimeStr { get; set; }

        private DateTime _time;
        private DateTime Time 
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                TimeStr = _time.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        public string OrigQuoteOrderQty { get; set; }

        public OrderGridRow (BinOrder order)
        {
            Symbol = order.Symbol;
            OrderId = order.OrderId;
            Price = order.Price;
            OrigQty = order.OrigQty;
            ExecutedQty = order.ExecutedQty;
            CummulativeQuoteQty = order.CummulativeQuoteQty;
            Status = order.Status;
            Type = order.Type;
            Side = order.Side;
            Time = new DateTime(1970, 01,01).AddMilliseconds(order.Time);
            OrigQuoteOrderQty = order.OrigQuoteOrderQty;
        }
    }
}
