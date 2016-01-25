//Mateusz Osipa
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise
{
    public class StandardMultipliedSquareMetersComparer : RoomComparer
    {
        public override int Compare(Room x, Room y)
        {
            if (x != null && y != null)
            {
                return (x.SquareMeters*(byte) x.Apartment).CompareTo(y.SquareMeters*(byte) y.Apartment);
            }
            else
            {
                if(x == null && y ==null)
                {
                    return 0;
                }
                else if (x == null)
                {
                    return -1;
                }
                else return 1;
            }
            
        }
    }
}
