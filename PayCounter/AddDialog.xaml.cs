using PayCounter.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddDialog.xaml
    /// </summary>
    public partial class AddDialog : Window
    {
        public MainWindow mainWindow { get; set; }
        public AddDialog(MainWindow openedfrom)
        {
            Owner = openedfrom;
            mainWindow = openedfrom;
            InitializeComponent();
            CategoryComboBox.ItemsSource = mainWindow.Categories;
            CategoryComboBox.SelectedIndex = 0;
        }

        private void CharPrevent(object sender, TextCompositionEventArgs e)
        {
            bool handle = IsInt(e.Text);
            e.Handled = !handle;
            if (handle == false)
            {
                MessageBox.Show("Вы ввели не число");
            }
        }
        private static bool IsInt(string text)
        {
            try
            {
                Convert.ToInt32(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            int QuantityBoxValue = Convert.ToInt32(QuantityBox.Text);
            int NewQuantityBoxValue = QuantityBoxValue + 1;
            QuantityBox.Text = NewQuantityBoxValue.ToString();
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            int QuantityBoxValue = Convert.ToInt32(QuantityBox.Text);
            if (QuantityBoxValue > 1)
            {
                int NewQuantityBoxValue = QuantityBoxValue - 1;
                QuantityBox.Text = NewQuantityBoxValue.ToString();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryComboBox.SelectedIndex == 0 || string.IsNullOrEmpty(DescriptionBox.Text) || string.IsNullOrEmpty(PriceBox.Text))
            {
                MessageBox.Show("Не все поля заполнены");
                return;
            }
            using (ApplicationContext db = new ApplicationContext())
            {
                var cat = db.Categories.ToList().Find(x => x.Name == ((Category)CategoryComboBox.SelectedItem).Name);
                Payment payment1 = new Payment
                {
                    Category = cat,
                    Date = DateTime.Today,
                    Description = DescriptionBox.Text,
                    Quantity = Convert.ToUInt32(QuantityBox.Text),
                    Price = Convert.ToDecimal(PriceBox.Text),
                    User = ((MainWindow)Owner).CurrentUser,
                };
                db.Payments.Add(payment1);
                db.Users.Attach(mainWindow.CurrentUser);
                db.Categories.Attach(cat);
                db.SaveChanges();
                mainWindow.Payments = db.Payments.Where(payment => payment.User == mainWindow.CurrentUser).ToList();
                mainWindow.ListView.ItemsSource = mainWindow.Payments;
            }
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
