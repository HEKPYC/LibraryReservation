using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryReservation
{
    public class User:IUser
    {
        public string Name { get; set; }
        public Dictionary<string, DateTime> BorrowedBooks { get; } = new Dictionary<string, DateTime>();
        public double Penalty { get; set; }

        public bool HasPenalty => Penalty > 0;
    }
}
