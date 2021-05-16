using CoinManager.Entities.Binance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinManager.Entities
{
    public class OrderGridRow
    {
        public string Symbol { get; set; }

        //public long OrderId { get; set; }

        public string Date { get; set; }

        public string Action { get; set; }

        private decimal Price { get; set; }

        //public decimal OrigQty { get; set; }
        //public decimal OrigQuoteOrderQty { get; set; }

        public string Status { get; set; }

        private decimal _nombre { get; set; }
        public string Nombre { get; set; }

        public string Valeur { get; set; }

        //public string Type { get; set; }

        public string Prix { get; set; }

        public string CoursDown { get; set; }
        public string CoursUP { get; set; }
        public string Difference { get; set; }


        //public string OrigQuoteOrderQty { get; set; }

        public OrderGridRow (BinOrder order, decimal coursActu)
        {
            Symbol = order.Symbol;
            //OrderId = order.OrderId;
            Price = order.Price;
            //OrigQty = order.OrigQty;
            _nombre = order.ExecutedQty;
            Nombre = _nombre.ToString("0.00");
            Valeur = order.CummulativeQuoteQty.ToString("0.00");
            Status = order.Status;
            //Type = order.Type;
            Action = order.Side;
            Date = new DateTime(1970, 01,01).AddMilliseconds(order.Time).ToString("dd/MM/yyyy HH:mm:ss");
            //OrigQuoteOrderQty = order.OrigQuoteOrderQty;

            if (Price == 0m)
                Price = (order.CummulativeQuoteQty / order.ExecutedQty);

            Prix = Price.ToString("0.0000");

            if (Action == "BUY")
            {
                if (Price > coursActu)
                    CoursDown = coursActu.ToString("0.0000");
                else
                    CoursUP = coursActu.ToString("0.0000");

                Difference = ((coursActu * _nombre) - (Price * _nombre)).ToString("0.0000");
            }
        }
    }
}
