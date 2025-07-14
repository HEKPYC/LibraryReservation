using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryReservation
{
    public interface IUser
    {
        string Name { get; set; }
        Dictionary<string, DateTime> BorrowedBooks { get; }
        double Penalty { get; set; }
        bool HasPenalty { get; }
    }
}
