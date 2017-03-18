using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CittaMobiWP.Models
{
    class EletronicTicket
    {
        public int CidadeId { get; set; }
        public string Numero { get; set; }
        public long UltimaAtualizacao { get; set; }
        public double Saldo { get; set; }
        public double SaldoRecarga { get; set; }
        public List<Wallet> Carteiras { get; set; }
        public List<object> DadosNFC { get; set; }
        public object NumeroInterno { get; set; }
        public object Errormessage { get; set; }
    }
}
