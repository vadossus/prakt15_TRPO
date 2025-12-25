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
using MaterialDesignThemes.Wpf;

namespace prakt15_TRPO.Views
{
    public partial class ProductEditWindow : Window
    {
        public List<Category> Categories { get; set; }
        public List<Brand> Brands { get; set; }

        public List<Tag> tags { get; set; }
        public Product CurrentProduct { get; }

        public ProductEditWindow(Product product)
        {
            InitializeComponent();
            CurrentProduct = product;
            DataContext = CurrentProduct;
            var context = DatabaseService.Instance.Context;
            Categories = context.Categories.OrderBy(c => c.Name).ToList();
            Brands = context.Brands.OrderBy(b => b.Name).ToList();
            tags = context.Tags.OrderBy(t => t.Name).ToList();

            this.DataContext = CurrentProduct;

            if (CurrentProduct.Tags == null)
                CurrentProduct.Tags = new List<Tag>();

            this.Loaded += (s, e) => MarkSelectedTags();
        }

        private void MarkSelectedTags()
        {
            if (CurrentProduct.Tags == null || !CurrentProduct.Tags.Any()) return;

            TagsListBox.SelectedItems.Clear();

            foreach (var tag in tags)
            {
                if (CurrentProduct.Tags.Any(t => t.Id == tag.Id))
                {
                    TagsListBox.SelectedItems.Add(tag);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(NameBox) ||
                Validation.GetHasError(RatingBox) ||
                Validation.GetHasError(PriceBox) || 
                Validation.GetHasError(DescBox) ||
                Validation.GetHasError(StockBox))
            {
                MessageBox.Show("Исправьте ошибки валидации, прежде чем продолжить.",
                                "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentProduct.CategoryId <= 0 || CurrentProduct.BrandId <= 0)
            {
                MessageBox.Show("Выберите категорию или бренд");
                return;
            }

            if (TagsListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Необходимо выбрать хотя бы один тег для товара!",
                                "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CurrentProduct.Tags.Clear();
            foreach (Tag selectedTag in TagsListBox.SelectedItems)
            {
                CurrentProduct.Tags.Add(selectedTag);
            }

            DialogResult = true;
        }
    }
}
