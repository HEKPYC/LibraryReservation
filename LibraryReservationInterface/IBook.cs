using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryReservation
{
    public interface IBook
    {
        string Title { get; set; }
        string Author { get; set; }
        int Year { get; set; }
        string Publisher { get; set; }
        int Pages { get; set; }
        int Quantity { get; set; }
        double DailyPenaltyRate { get; set; }

        int AvailableCopies { get; }
        List<string> ReservedBy { get; }
        Queue<string> WaitQueue { get; }
    }

}
