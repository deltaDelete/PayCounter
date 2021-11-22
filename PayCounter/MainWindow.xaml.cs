using PayCounter.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Diagnostics;

namespace PayCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public User CurrentUser { get; set; }
        public List<Payment> Payments { get; set; }
        public List<Category> Categories { get; set; }
        public MainWindow(User usr)
        {
            CurrentUser = usr;
            InitializeComponent();
            using (ApplicationContext db = new ApplicationContext())
            {
                Payments = db.Payments.Where(payment => payment.User == CurrentUser).ToList();
                Categories = db.Categories.ToList();
                Categories.Insert(0, new Category { Id = 0, Name = "Не задано" });
            }
            ListView.ItemsSource = Payments;
            category.ItemsSource = Categories;
            category.SelectedIndex = 0;
            category.DisplayMemberPath = "Name";
        }

        private void AddPayment_Click(object sender, RoutedEventArgs e)
        {
            AddDialog addDialog = new AddDialog(this);
            addDialog.Show();
        }

        private void RemovePayment_Click(object sender, RoutedEventArgs e)
        {
            Payment selected = (Payment)ListView.SelectedItem;
            if (selected != null)
            {
                PlayNotifySound();
                var result = MessageBox.Show($"Вы уверены, что хотите удалить \"{selected.Description}\"?", "Удалить?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        db.Payments.Remove(selected);
                        db.Users.Attach(CurrentUser);
                        db.SaveChanges();
                        Payments = db.Payments.Where(payment => payment.User == CurrentUser).ToList();
                    }
                    ListView.ItemsSource = Payments;
                }
            }
            else
            {
                MessageBox.Show("Не выбран элемент для удаления");
            }
        }
        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime DateTimeFrom = !DatePickerFrom.SelectedDate.HasValue ? DateTime.MinValue : (DateTime)DatePickerFrom.SelectedDate;
            DateTime DateTimeTo = !DatePickerTo.SelectedDate.HasValue ? DateTime.MaxValue : (DateTime)DatePickerTo.SelectedDate;
            System.Diagnostics.Debug.WriteLine($"FROM {DatePickerFrom} TO {DateTimeTo}");
            if (category.SelectedIndex > 0)
            {
                ListView.ItemsSource = Payments.Where(payment => payment.Category == category.SelectedItem && payment.Date.CompareTo(DateTimeFrom) > 0 && payment.Date.CompareTo(DateTimeTo) < 0);
            }
            else if (category.SelectedIndex == 0)
            {
                ListView.ItemsSource = Payments.Where(payment => payment.Date.CompareTo(DateTimeFrom) > 0 && payment.Date.CompareTo(DateTimeTo) < 0);
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ListView.ItemsSource = Payments;
        }
        public static void PlayNotifySound()
        {
            bool found = false;
            try
            {
                using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"AppEvents\Schemes\Apps\.Default\Notification.Default\.Current"))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue(null); // pass null to get (Default)
                        if (o != null)
                        {
                            System.Media.SoundPlayer theSound = new System.Media.SoundPlayer((String)o);
                            theSound.Play();
                            found = true;
                        }
                    }
                }
            }
            catch
            { }
            if (!found)
                System.Media.SystemSounds.Beep.Play();
        }
        private void ReportBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "report";
            dialog.DefaultExt = ".docx";
            dialog.Filter = "Документ Microsoft Word|*.docx";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                try
                {
                    Report.MakeReport(dialog.FileName, Payments, CurrentUser.FIO);
                }
                catch
                {
                    MessageBox.Show("Файл заблокирован другой программой, закройте её и повторите попытку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
