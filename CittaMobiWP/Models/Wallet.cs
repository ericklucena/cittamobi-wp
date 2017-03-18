using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CittaMobiWP.Models
{
    class Wallet
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public double Saldo { get; set; }
        public double SaldoRecarga { get; set; }
        public bool Cansellrecharge { get; set; }
    }
}
