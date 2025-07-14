using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryReservation
{
    public interface ILibrarySystem
    {
        Dictionary<string, IBook> Books { get; }
        Dictionary<string, IUser> Users { get; }

        void AddBook(IBook book);
        void RegisterUser(string name);
        string ReserveBook(string username, string title);
        string ReturnBook(string username, string title);
        void PayPenalty(string username);
    }

}
