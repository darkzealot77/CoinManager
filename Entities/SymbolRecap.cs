using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinManager.Entities
{
    public class SymbolRecap
    {
        public string Symbol { get; set; }

        public double Valeur { get; set; }

        public double Nombre { get; set; }
        public double Moyenne
        {
            get
            {
                if (Nombre != 0)
                    return Valeur / Nombre;
                else
                    return 0;
            }
        }

        public double Cours { get; set; }
        public double Difference { get; set; }
    }
}
