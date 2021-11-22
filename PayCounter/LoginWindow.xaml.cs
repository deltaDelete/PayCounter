using PayCounter.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PayCounter
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public List<User> UserList { get; set; }
        public LoginWindow()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                UserList = db.Users.ToList();
            }
            InitializeComponent();
            Login.ItemsSource = UserList;
            Login.DisplayMemberPath = "Login";
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("CLICK!");
            Close();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("CLICK!");
            var foundUser = UserList.Find(user => user.Login == Login.Text && user.Password == Security.CreateMD5(Password.Password).ToLower());
            if (foundUser == null)
            {
                MessageBox.Show("Неправильный логин или пароль");
                return;
            }
            MainWindow mainWindow = new MainWindow(foundUser);
            mainWindow.Show();
            // Вход сделан, теперь передавать foundUser в окно MainWindow и там вычислять связи
            Close();
        }

        private void LoginBtn_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("RIGHTCLICK!");
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.PreviousWindow = this;
            registerWindow.Show();
            Hide();
        }
    }
}
