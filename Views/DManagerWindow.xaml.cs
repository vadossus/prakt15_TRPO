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
                var db = DatabaseService.Instance.Context;

                var emptyCategories = db.Categories.Local.Where(x => string.IsNullOrWhiteSpace(x.Name)).ToList();
                foreach (var item in emptyCategories) db.Categories.Remove(item);

                var emptyBrands = db.Brands.Local.Where(x => string.IsNullOrWhiteSpace(x.Name)).ToList();
                foreach (var item in emptyBrands) db.Brands.Remove(item);

                var emptyTags = db.Tags.Local.Where(x => string.IsNullOrWhiteSpace(x.Name)).ToList();
                foreach (var item in emptyTags) db.Tags.Remove(item);

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}
