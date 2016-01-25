//Mateusz Osipa
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise
{
    public abstract class RoomComparer : IComparer<Room>
    {
        public abstract int Compare(Room x, Room y);
    }
}
