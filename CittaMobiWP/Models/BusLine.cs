using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CittaMobiWP.Models
{
    enum Direction{
        Ida,
        Volta
    }

    class BusLine
    {
        public int Id {get; set;}
        public string Name { get; set; }
        public Direction Direction {get; set;}

        public BusLine(int id, string name, Direction direction)
        {
            this.Id = id;
            this.Name = name;
            this.Direction = direction;
        }
    }
}
