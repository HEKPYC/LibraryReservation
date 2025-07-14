using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryReservation
{
    public class Book:IBook
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Publisher { get; set; }
        public int Pages { get; set; }
        public int Quantity { get; set; }
        public double DailyPenaltyRate { get; set; }

        public List<string> ReservedBy { get; } = new List<string>();
        public Queue<string> WaitQueue { get; } = new Queue<string>();

        public int AvailableCopies => Quantity - ReservedBy.Count;
    }
}
