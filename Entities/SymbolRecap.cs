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

        public decimal Valeur { get; set; }

        public decimal Nombre { get; set; }
        public decimal Moyenne
        {
            get
            {
                if (Nombre != 0)
                    return Valeur / Nombre;
                else
                    return 0;
            }

            set
            {
                Valeur = value * Nombre;
            }
        }

        public decimal Cours { get; set; }
        public decimal Difference { get; set; }
    }
}
