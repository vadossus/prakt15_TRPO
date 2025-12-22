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
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using prakt15_TRPO.Models;
using prakt15_TRPO.Service;
using System.IO;

namespace prakt15_TRPO.Views
{
    public partial class ProductEditWindow : Window
    {
        public List<Category> Categories { get; set; }
        public List<Brand> Brands { get; set; }
        public Product CurrentProduct { get; }

        public ProductEditWindow(Product product)
        {
            InitializeComponent();
            CurrentProduct = product;
            DataContext = CurrentProduct;
            var context = DatabaseService.Instance.Context;
            Categories = context.Categories.OrderBy(c => c.Name).ToList();
            Brands = context.Brands.OrderBy(b => b.Name).ToList();

            this.DataContext = CurrentProduct;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentProduct.Name) || CurrentProduct.Price < 0)
            {
                MessageBox.Show("Пожалуйста, проверьте правильность заполнения полей.");
                return;
            }
            DialogResult = true;
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png",
                Title = "Выберите картинку товара"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string imagesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                    if (!Directory.Exists(imagesDir)) Directory.CreateDirectory(imagesDir);

                    string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(dialog.FileName);
                    string destPath = System.IO.Path.Combine(imagesDir, fileName);

                    File.Copy(dialog.FileName, destPath, true);

                    CurrentProduct.ImagePath = System.IO.Path.Combine("Images", fileName);
                    ImgPreview.Source = new BitmapImage(new Uri(destPath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке: " + ex.Message);
                }
            }
        }
    }
}
