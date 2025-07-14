using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryReservation
{
    public class LibrarySystem:ILibrarySystem
    {
        public Dictionary<string, IBook> Books { get; } = new Dictionary<string, IBook>();
        public Dictionary<string, IUser> Users { get; } = new Dictionary<string, IUser>();

        public void AddBook(IBook book)
        {
            Books[book.Title] = book;
        }

        public void RegisterUser(string name)
        {
            if (!Users.ContainsKey(name))
                Users[name] = new User { Name = name };
        }

        public string ReserveBook(string username, string title)
        {
            if (!Users.ContainsKey(username)) return $"Користувач {username} не знайдений для резервації книги {title}.";
            var user = Users[username];
            var book = Books[title];

            if (user.HasPenalty)
                return $"Користувач {username} спочатку має сплатити пеню: {user.Penalty} грн";

            if (book.AvailableCopies > 0)
            {
                book.ReservedBy.Add(username);
                user.BorrowedBooks[title] = DateTime.Now;
                return $"Книгу {title} зарезервовано за користувачем {user.Name}.";
            }
            else
            {
                if (!book.WaitQueue.Contains(username))
                    book.WaitQueue.Enqueue(username);
                return $"Немає вільних книг {title}. Користувача {user.Name} додано в чергу.";
            }
        }

        public string ReturnBook(string username, string title)
        {
            if (!Users.ContainsKey(username)) return $"Користувач {username} не знайдений для повернення книги {title}.";

            var user = Users[username];
            var book = Books[title];

            if (!user.BorrowedBooks.ContainsKey(title)) return $"Книга {title} не зарезервована за користувачем {username}.";

            var date = user.BorrowedBooks[title];
            user.BorrowedBooks.Remove(title);
            book.ReservedBy.Remove(username);

            var overdue = (DateTime.Now - date).Days - 14;
            if (overdue > 0)
                user.Penalty += overdue * book.DailyPenaltyRate;

            if (book.WaitQueue.Count > 0)
            {
                string nextUser = book.WaitQueue.Dequeue();
                ReserveBook(nextUser, title);
            }

            return overdue > 0 ? $"Книгу {title} повернуто з пенею. Для користувача {user.Name} додано {overdue * (double)book.DailyPenaltyRate} грн." : $"Книгу {title} повернуто користувачем {user.Name} вчасно.";
        }

        public void PayPenalty(string username)
        {
            if (Users.ContainsKey(username))
                Users[username].Penalty = 0;
        }
    }

}
