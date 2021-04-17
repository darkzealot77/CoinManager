using CoinManager.Entities.Binance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinManager.Entities
{
    public class DicoOrders : Dictionary<string, Dictionary<long, BinOrder>>
    {

    }
}
