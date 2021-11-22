using PayCounter.DB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PayCounter
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public Window PreviousWindow { get; set; }
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
            PreviousWindow.Show();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                User newuser = new User
                {
                    FIO = FIO.Text,
                    Login = Login.Text,
                    Password = Security.CreateMD5(Password.Password),
                    PINCode = Convert.ToUInt32(PINCode.Text)
                };
                db.Add(newuser);
                db.SaveChanges();
            }
            MessageBox.Show("Новый пользователь успешно зарегистрирован");
            Close();
            PreviousWindow.Show();
        }
    }
}
