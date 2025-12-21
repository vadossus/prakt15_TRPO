using Microsoft.EntityFrameworkCore;
using prakt15_TRPO.Data;
using prakt15_TRPO.Models;
using prakt15_TRPO.Service;
using prakt15_TRPO.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace prakt15_TRPO
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly bool _isManager;
        public bool IsManager => _isManager;
        private EStoreContext _context;

        private ObservableCollection<Product> _products = new();
        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(nameof(Products)); }
        }

        private ICollectionView _productsView;
        public ICollectionView ProductsView
        {
            get => _productsView;
            set { _productsView = value; OnPropertyChanged(nameof(ProductsView)); }
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ProductsView?.Refresh();
                UpdateCounters();
            }
        }

        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<Brand> Brands { get; set; } = new();

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                ProductsView?.Refresh();
                UpdateCounters();
            }
        }

        private Brand _selectedBrand;
        public Brand SelectedBrand
        {
            get => _selectedBrand;
            set
            {
                _selectedBrand = value;
                OnPropertyChanged(nameof(SelectedBrand));
                ProductsView?.Refresh();
                UpdateCounters();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow(bool isManager)
        {
            _isManager = isManager;
            _context = DatabaseService.Instance.Context;

            InitializeComponent();

            UserRoleText.Text = _isManager ? "Режим: Менеджер" : "Режим: Посетитель";
            AddProductBtn.Visibility = _isManager ? Visibility.Visible : Visibility.Collapsed;
            ManageCategoriesBtn.Visibility = _isManager ? Visibility.Visible : Visibility.Collapsed;

            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            var categories = _context.Categories.ToList();
            Categories.Clear();
            foreach (var c in categories) Categories.Add(c);
            var brands = _context.Brands.ToList();
            Brands.Clear();
            foreach (var b in brands) Brands.Add(b);

            LoadProducts();

            ProductsView = CollectionViewSource.GetDefaultView(Products);
            ProductsView.Filter = FilterProduct;

            UpdateCounters();
        }

        private void LoadProducts()
        {
            try
            {
                var products = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Tags)
                    .ToList();

                Products.Clear();
                foreach (var product in products) Products.Add(product);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private bool FilterProduct(object obj)
        {
            if (obj is not Product product) return false;

            if (!string.IsNullOrEmpty(SearchText) &&
                !product.Name.ToLower().Contains(SearchText.ToLower()))
                return false;

            if (SelectedCategory != null && product.CategoryId != SelectedCategory.Id)
                return false;

            if (SelectedBrand != null && product.BrandId != SelectedBrand.Id)
                return false;

            if (decimal.TryParse(PriceFromTextBox.Text, out decimal min) && product.Price < min) return false;
            if (decimal.TryParse(PriceToTextBox.Text, out decimal max) && product.Price > max) return false;

            if (LowStockCheckBox.IsChecked == true && product.Stock > 10) return false;

            return true;
        }

        private void UpdateCounters()
        {
            var total = _context.Products.Count();
            var filtered = ProductsView?.Cast<Product>().Count() ?? 0;

            TotalCountText.Text = $"Всего товаров: {total}";
            FilteredCountText.Text = $"Показано: {filtered}";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => SearchText = SearchTextBox.Text;

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void PriceFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProductsView?.Refresh();
            UpdateCounters();
        }

        private void LowStockCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            ProductsView?.Refresh();
            UpdateCounters();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsView == null || SortComboBox.SelectedItem == null) return;
            var selected = (ComboBoxItem)SortComboBox.SelectedItem;
            ProductsView.SortDescriptions.Clear();

            string tag = selected.Tag.ToString();
            if (tag.Contains("Desc"))
                ProductsView.SortDescriptions.Add(new SortDescription(tag.Replace("Desc", ""), ListSortDirection.Descending));
            else
                ProductsView.SortDescriptions.Add(new SortDescription(tag, ListSortDirection.Ascending));
        }

        private void ResetFiltersBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            SelectedCategory = null; 
            SelectedBrand = null;    
            PriceFromTextBox.Text = "";
            PriceToTextBox.Text = "";
            LowStockCheckBox.IsChecked = false;
            SortComboBox.SelectedIndex = -1;

            ProductsView?.Refresh();
            UpdateCounters();
        }

        private void EditProductBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int productId)
            {
                var productToEdit = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .FirstOrDefault(p => p.Id == productId);

                if (productToEdit != null)
                {
                    var editWin = new ProductEditWindow(productToEdit) { Owner = this };
                    if (editWin.ShowDialog() == true)
                    {
                        try
                        {
                            _context.SaveChanges();

                            ProductsView.Refresh();
                            UpdateCounters();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка");
                        }
                    }
                    else
                    {
                        _context.Entry(productToEdit).Reload();
                        ProductsView.Refresh();
                    }
                }
            }
        }

        private void DeleteProductBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int productId)
            {
                if (MessageBox.Show("Удалить товар?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var product = _context.Products.Find(productId);
                    if (product != null)
                    {
                        _context.Products.Remove(product);
                        _context.SaveChanges();
                        var item = Products.FirstOrDefault(p => p.Id == productId);
                        if (item != null) Products.Remove(item);
                        UpdateCounters();
                    }
                }
            }
        }

        private void AddProductBtn_Click(object sender, RoutedEventArgs e)
        {
            var newProduct = new Product { Name = "", CreatedAt = DateTime.Now };
            var editWin = new ProductEditWindow(newProduct) { Owner = this };

            if (editWin.ShowDialog() == true)
            {
                _context.Products.Add(newProduct);
                _context.SaveChanges();
                Products.Add(newProduct);
                ProductsView.Refresh();
            }
        }


        private void ManageCategoriesBtn_Click(object sender, RoutedEventArgs e)
        {
            var dictWin = new DManagerWindow() { Owner = this };
            if (dictWin.ShowDialog() == true)
            {
                LoadData(); 
            }
        }
        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !char.IsDigit(e.Text, 0);

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}