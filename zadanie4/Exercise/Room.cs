//Mateusz Osipa
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise
{
    public class Room : IRoom
    {
        public RoomStandard Apartment { get;private set; }

        public Room(double v, RoomStandard apartment)
        {
            this.SquareMeters= v;
            this.Apartment = apartment;
        }

        public double SquareMeters { get;private set; }
    }
}
