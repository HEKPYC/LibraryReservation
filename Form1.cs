using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryReservation
{
    public partial class Form1 : Form
    {
        LibrarySystem library = new LibrarySystem();

        private TextBox txtUser;
        private TextBox txtTitle;
        private TextBox txtAuthor;
        private TextBox txtYear;
        private TextBox txtPublisher;
        private TextBox txtPages;
        private TextBox txtQuantity;
        private ListBox lstQueue;
        private ComboBox cmbBooks;
        private Button btnRegisterUser;
        private Button btnAddBook;
        private Button btnReserveBook;
        private Button btnReturnBook;
        private Button btnPayPenalty;
        private ListBox lstOutput;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnRegisterUser_Click(object sender, EventArgs e)
        {
            string userName = txtUser.Text.Trim();
            if (string.IsNullOrEmpty(userName))
            {
                ShowMessage("Введіть ім’я користувача");
                return;
            }
            library.RegisterUser(userName);
            txtUser.Clear();
            ShowMessage($"Користувача '{userName}' зареєстровано");
        }

        private void btnAddBook_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string author = txtAuthor.Text.Trim();
            string yearStr = txtYear.Text.Trim();
            string publisher = txtPublisher.Text.Trim();
            string pagesStr = txtPages.Text.Trim();
            string quantityStr = txtQuantity.Text.Trim();
            string penaltyStr = txtPenalty.Text.Trim().Replace('.', ',');

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(author) || string.IsNullOrEmpty(yearStr) ||
                string.IsNullOrEmpty(publisher) || string.IsNullOrEmpty(pagesStr) || string.IsNullOrEmpty(quantityStr) || 
                string.IsNullOrEmpty(penaltyStr))
            {
                ShowMessage("Заповніть всі поля книги");
                return;
            }

            if (!int.TryParse(yearStr, out int year))
            {
                ShowMessage("Некоректний рік видання");
                return;
            }
            if (!int.TryParse(pagesStr, out int pages))
            {
                ShowMessage("Некоректна кількість сторінок");
                return;
            }
            if (!int.TryParse(quantityStr, out int quantity))
            {
                ShowMessage("Некоректна кількість екземплярів");
                return;
            }
            if (!double.TryParse(penaltyStr, out double penalty))
            {
                ShowMessage("Некоректна пеня");
                return;
            }

            if (library.Books.ContainsKey(title) && library.Books[title].Author == author && library.Books[title].Year == year && library.Books[title].Publisher == publisher
                 && library.Books[title].Pages == pages && library.Books[title].DailyPenaltyRate == penalty)
            {
                library.Books[title].Quantity += quantity;
                txtTitle.Clear();
                txtAuthor.Clear();
                txtYear.Clear();
                txtPublisher.Clear();
                txtPages.Clear();
                txtQuantity.Clear();
                txtPenalty.Clear();
                ShowMessage($"Кількість книги '{title}' оновлено");
            }
            else
            {
                var book = new Book()
                {
                    Title = title,
                    Author = author,
                    Year = year,
                    Publisher = publisher,
                    Pages = pages,
                    Quantity = quantity,
                    DailyPenaltyRate = penalty
                };
                library.AddBook(book);
                txtTitle.Clear();
                txtAuthor.Clear();
                txtYear.Clear();
                txtPublisher.Clear();
                txtPages.Clear();
                txtQuantity.Clear();
                txtPenalty.Clear();
                ShowMessage($"Додано книгу '{title}'");
            }

            UpdateBooksComboBox();
        }

        private void btnReserveBook_Click(object sender, EventArgs e)
        {
            string userName = txtUser.Text.Trim();
            string selected = cmbBooks.SelectedItem as string;
            int index = selected.LastIndexOf('(');
            string title = (index > 0) ? selected.Substring(0, index).Trim() : selected;
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(title))
            {
                ShowMessage("Вкажіть користувача та книгу для резервування");
                return;
            }
            txtUser.Clear();
            if (cmbBooks.SelectedIndex != -1)
            {
                cmbBooks.SelectedIndex = -1;
            }
            string result = library.ReserveBook(userName, title);
            UpdateBooksComboBox();
            ShowMessage(result);
            UpdateQueueList(title);
        }

        private void btnReturnBook_Click(object sender, EventArgs e)
        {
            string userName = txtUser.Text.Trim();
            string selected = cmbBooks.SelectedItem as string;
            int index = selected.LastIndexOf('(');                
            string title = (index > 0) ? selected.Substring(0, index).Trim() : selected;
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(title))
            {
                ShowMessage("Вкажіть користувача та книгу для повернення");
                return;
            }
            txtUser.Clear();
            if (cmbBooks.SelectedIndex != -1)
            {
                cmbBooks.SelectedIndex = -1;
            }
            string result = library.ReturnBook(userName, title);
            UpdateBooksComboBox();
            ShowMessage(result);
            UpdateQueueList(title);
        }

        private void btnPayPenalty_Click(object sender, EventArgs e)
        {
            string userName = txtUser.Text.Trim();
            if (string.IsNullOrEmpty(userName))
            {
                ShowMessage("Введіть ім’я користувача");
                return;
            }

            if (!library.Users.ContainsKey(userName))
            {
                ShowMessage($"Користувач {userName} не знайдений.");
                return;
            }
            txtUser.Clear();
            var user = library.Users[userName];
            double penalty = user.Penalty;

            if (penalty == 0)
            {
                ShowMessage($"Користувач '{userName}' не має пені.");
                return;
            }

            library.PayPenalty(userName);

            ShowMessage($"Пеня користувача '{userName}' сплачена в розмірі {(double)penalty} грн.");
        }

        private void cmbBooks_SelectedIndexChanged(object sender, EventArgs e)
        {
            string title = cmbBooks.SelectedItem as string;
            if (!string.IsNullOrEmpty(title))
                UpdateQueueList(title);
        }

        private void UpdateBooksComboBox()
        {
            cmbBooks.Items.Clear();
            foreach (var book in library.Books.Values)
            {
                string Book = book.Title + "(" + Math.Max(0, book.AvailableCopies - book.WaitQueue.Count) + ")";
                cmbBooks.Items.Add(Book);
            }
        }

        private void UpdateQueueList(string title)
        {
            lstQueue.Items.Clear();
            if (library.Books.ContainsKey(title))
            {
                var book = library.Books[title];
                foreach (var user in book.WaitQueue)
                    lstQueue.Items.Add(user);
            }
        }

        private void ShowMessage(string message)
        {
            lstOutput.Items.Add($"{DateTime.Now.ToShortTimeString()}: {message}");
            lstOutput.TopIndex = lstOutput.Items.Count - 1; // прокрутка вниз
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
