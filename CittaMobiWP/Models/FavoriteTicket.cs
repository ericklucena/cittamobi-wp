using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CittaMobiWP.Models
{
    class FavoriteTicket
    {
        public string Id { get; set; }
        public string Nickname { get; set; }
        public double Balance { get; set; }

        public string StringBalance
        {
            get
            {
                return "R$ " + Balance;
            }
        }

        public FavoriteTicket (string id, string nickname)
        {
            Id = id;
            if (nickname == null || nickname.Length == 0)
            {
                Nickname = id;
            }
            else
            {
                Nickname = nickname;
            }
        }
    }
}
