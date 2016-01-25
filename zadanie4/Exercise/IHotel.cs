//Mateusz Osipa
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exercise
{
    public interface IHotel :IEnumerable<IRoom>
    {
        uint Stars { get;  }
    }
}
