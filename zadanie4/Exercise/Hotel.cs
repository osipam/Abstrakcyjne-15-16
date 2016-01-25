//Mateusz Osipa
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Exercise
{
    public class Hotel<R, C> : IHotel where R : IRoom where C : IComparer<R>, new()
    {
        private readonly IDictionary<R, List<Tuple<DateTime, DateTime>>> rooms;
        private C comparer;

        public Hotel(uint v, params R[] r)
        {
            if(r == null)
                throw new ArgumentNullException();
            Stars = v;
            comparer = new C();
            rooms = new SortedDictionary<R, List<Tuple<DateTime, DateTime>>>(comparer);
            foreach (var room in r.AsEnumerable())
            {
                rooms.Add(room, new List<Tuple<DateTime, DateTime>>());
            }
        }

        public uint Stars { get; }

        public IEnumerator<IRoom> GetEnumerator()
        {
            foreach (var i in rooms.Keys)
            {
                yield return i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsFree(R p0, DateTime from, DateTime to)
        {
            if (!rooms.Keys.ToList().Contains(p0) || from.CompareTo(to) > 0 || from.CompareTo(DateTime.Now) < 0)
                throw new ArgumentOutOfRangeException("");
            List<Tuple<DateTime, DateTime>> reservation;
            if (rooms.TryGetValue(p0, out reservation))
            {
                return reservation.All(res => CheckIfIsFree(res, from, to));
            }
            throw new ArgumentOutOfRangeException("");
        }

        public void Book(R p0, DateTime from, DateTime to)
        {
            if (!rooms.Keys.ToList().Contains(p0) || from.CompareTo(to) > 0 || from.CompareTo(DateTime.Now) < 0)
                throw new ArgumentOutOfRangeException("");
            List<Tuple<DateTime, DateTime>> reservation;
            if (rooms.TryGetValue(p0, out reservation) && reservation.All(res => CheckIfIsFree(res, from, to)))
            {
                reservation.Add(new Tuple<DateTime, DateTime>(from, to));
            }
            else
                throw new ArgumentOutOfRangeException("It's not possible to find this room in hotel");
        }

        private bool CheckIfIsFree(Tuple<DateTime, DateTime> reservation, DateTime from, DateTime to)
        {
            if (reservation.Item1 <= from && reservation.Item2 >= to)
                return false;
            if (reservation.Item1 <= from && reservation.Item2 < to && reservation.Item2 > from)
                return false;
            if (reservation.Item1 > from && reservation.Item2 >= to && reservation.Item1 < to)
                return false;
            if (reservation.Item1 > from && reservation.Item2 < to)
                return false;

            return true;
        }
    }
}