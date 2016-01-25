//Mateusz Osipa
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise
{
    public interface IRoom
    {
        double SquareMeters { get;}
        RoomStandard Apartment { get; }
    }
}
