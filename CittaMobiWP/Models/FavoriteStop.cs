using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CittaMobiWP.Models
{
    class FavoriteStop
    {
        public int Id { get; set; }
        public string Mnemonic { get; set; }

        public FavoriteStop(int id, string mnemonic)
        {
            this.Id = id;
            this.Mnemonic = mnemonic;
        }
    }
}
