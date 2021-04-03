using CoinManager.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoinManager.Component
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class ValeurCrypto : UserControl
    {
        public double Valeur { get; set; }

        public string ValeurStr
        {
            get
            {
                return String.Format("{0:N4}", Valeur);
            }
        }

        public double Nombre { get; set; }

        public string NombreStr
        {
            get
            {
                return String.Format("{0:N4}", Nombre);
            }
        }

        public double Moyenne { get; set; }

        public string MoyenneStr
        {
            get
            {
                return String.Format("{0:N4}", Moyenne);
            }
        }

        public string Symbol { get; set; }

        public double Cours { get; set; }

        public string CoursStr
        {
            get
            {
                return String.Format("{0:N4}", Cours);
            }
        }

        public ValeurCrypto()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public ValeurCrypto(string key, AllOrders value)
            :this()
        {
            Symbol = key;
            Valeur = value.Valeur;
            Nombre = value.Nombre;
            Moyenne = value.Moyenne;
        }
    }
}
