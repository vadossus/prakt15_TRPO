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
            if (string.IsNullOrWhiteSpace(CurrentProduct.Name) || CurrentProduct.Price <= 0 || CurrentProduct.Rating <= 0 || CurrentProduct.Stock <= 0)
            {
                MessageBox.Show("Пожалуйста, проверьте правильность заполнения полей.");
                return;
            }

            if (CurrentProduct.CategoryId <= 0 || CurrentProduct.BrandId <= 0)
            {
                MessageBox.Show("Выберите категорию или бренд");
                return;
            }
            DialogResult = true;
        }
    }
}
