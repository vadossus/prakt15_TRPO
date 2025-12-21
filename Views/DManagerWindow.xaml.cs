using prakt15_TRPO.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using System.Windows.Shapes;

namespace prakt15_TRPO.Views
{
    /// <summary>
    /// Логика взаимодействия для DManagerWindow.xaml
    /// </summary>
    public partial class DManagerWindow : Window
    {
        public DManagerWindow()
        {
            InitializeComponent();
            var db = DatabaseService.Instance.Context;

            db.Categories.Load();
            db.Brands.Load();
            db.Tags.Load();

            CategoriesGrid.ItemsSource = db.Categories.Local.ToObservableCollection();
            BrandsGrid.ItemsSource = db.Brands.Local.ToObservableCollection();
            TagsGrid.ItemsSource = db.Tags.Local.ToObservableCollection();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DatabaseService.Instance.Context.SaveChanges();
                MessageBox.Show("Справочники обновлены успешно!");
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}
