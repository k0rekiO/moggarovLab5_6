using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5
{
    public partial class MainWindow : Window
    {
        private string _userRole;
        private ObservableCollection<Product> _products;
        private CollectionViewSource _source;
        public ICollectionView Products { get; set; }

        public MainWindow(string role)
        {
            InitializeComponent();
            _userRole = role;
            UserInfoText.Text = $"Роль: {role}";

            DataContext = this;
            _products = new ObservableCollection<Product>();
            _source = new CollectionViewSource { Source = _products };
            Products = _source.View;

            ApplyRole();
            LoadData();
        }

        private void ApplyRole()
        {
            if (_userRole == "Гость" || _userRole == "Авторизированный клиент")
            {
                SearchGroup.Visibility = Visibility.Collapsed;
                AdminPanel.Visibility = Visibility.Collapsed;
            }
            else if (_userRole == "Менеджер")
            {
                AddButton.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Collapsed;
                DeleteButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadData()
        {
            using (var context = new Lab5BikeShopContext())
            {
                _products.Clear();
                var list = context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Supplier)
                    .ToList();

                foreach (var p in list)
                {
                    _products.Add(p);
                }
            }
            Products.Filter = FilterProducts;
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            var ordersWindow = new OrdersWindow(_userRole);
            ordersWindow.Owner = this;
            ordersWindow.ShowDialog();
        }

        private bool FilterProducts(object item)
        {
            if (item is Product product)
            {
                if (string.IsNullOrWhiteSpace(SearchBox.Text)) return true;
                string search = SearchBox.Text.ToLower();

                return (product.Name?.ToLower().Contains(search) ?? false) ||
                       (product.Description?.ToLower().Contains(search) ?? false) ||
                       (product.Article?.ToLower().Contains(search) ?? false);
            }
            return false;
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Products.Refresh();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new ProductEditWindow();
            editWindow.Owner = this;
            if (editWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is Product selectedProduct)
            {
                var editWindow = new ProductEditWindow(selectedProduct);
                editWindow.Owner = this;
                if (editWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is Product selectedProduct)
            {
                using (var context = new Lab5BikeShopContext())
                {
                    bool isInOrder = context.OrderArticles.Any(oa => oa.ProductId == selectedProduct.Id);
                    if (isInOrder)
                    {
                        MessageBox.Show("Невозможно удалить товар, так как он присутствует в заказе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var result = MessageBox.Show($"Удалить товар '{selectedProduct.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            var productToDelete = context.Products.Find(selectedProduct.Id);
                            if (productToDelete != null)
                            {
                                // 1. Удаляем картинку из папки
                                if (!string.IsNullOrEmpty(productToDelete.ImagePath))
                                {
                                    string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", productToDelete.ImagePath);
                                    if (System.IO.File.Exists(filePath))
                                    {
                                        try
                                        {
                                            System.IO.File.Delete(filePath);
                                        }
                                        catch (Exception imgEx)
                                        {
                                            MessageBox.Show("Не удалось удалить файл картинки: " + imgEx.Message);
                                        }
                                    }
                                }

                                // 2. Удаляем товар из базы
                                context.Products.Remove(productToDelete);
                                context.SaveChanges();
                            }
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка удаления: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления.");
            }
        
    }
    }
}