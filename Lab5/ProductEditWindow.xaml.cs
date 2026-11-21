using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Lab5.Models;

namespace Lab5
{
    public partial class ProductEditWindow : Window
    {
        private Lab5BikeShopContext _context;
        private Product _currentProduct;
        private bool _isEditMode;

        private string _selectedImagePath = "";

        public ProductEditWindow(Product product = null)
        {
            InitializeComponent();
            _context = new Lab5BikeShopContext();
            LoadComboBoxes();

            if (product != null)
            {
                _isEditMode = true;
                Title = "Редактирование товара";
                _currentProduct = product;
                FillFields();
            }
            else
            {
                _isEditMode = false;
                Title = "Добавление товара";
                IdBox.Visibility = Visibility.Collapsed;

                int maxId = _context.Products.Any() ? _context.Products.Max(p => p.Id) : 0;
                IdBox.Text = (maxId + 1).ToString();
            }
        }

        private void LoadComboBoxes()
        {
            CategoryCombo.ItemsSource = _context.Categories.ToList();
            ManufacturerCombo.ItemsSource = _context.Manufacturers.ToList();
            SupplierCombo.ItemsSource = _context.Suppliers.ToList();
        }

        private void FillFields()
        {
            IdBox.Text = _currentProduct.Id.ToString();
            ArticleBox.Text = _currentProduct.Article;
            NameBox.Text = _currentProduct.Name;
            DescBox.Text = _currentProduct.Description;
            UnitBox.Text = _currentProduct.Unit;
            PriceBox.Text = _currentProduct.Price.ToString();
            CountBox.Text = _currentProduct.Count.ToString();
            DiscountBox.Text = _currentProduct.Discount.ToString();

            CategoryCombo.SelectedItem = _currentProduct.Category;
            ManufacturerCombo.SelectedItem = _currentProduct.Manufacturer;
            SupplierCombo.SelectedItem = _currentProduct.Supplier;

            if (!string.IsNullOrEmpty(_currentProduct.ImagePath))
            {
                try
                {
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", _currentProduct.ImagePath);
                    if (File.Exists(fullPath))
                    {
                        BitmapImage bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.UriSource = new Uri(fullPath);
                        bmp.EndInit();
                        ProductImage.Source = bmp;
                    }
                }
                catch { /* Игнорируем ошибки загрузки картинки */ }
            }
        }

        private void ChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (dialog.ShowDialog() == true)
            {
                _selectedImagePath = dialog.FileName;

                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(_selectedImagePath);
                bmp.EndInit();
                ProductImage.Source = bmp;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ArticleBox.Text) || string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Артикул и Наименование обязательны для заполнения!");
                return;
            }
            if (!decimal.TryParse(PriceBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Цена должна быть неотрицательным числом!");
                return;
            }
            if (!int.TryParse(CountBox.Text, out int count) || count < 0)
            {
                MessageBox.Show("Количество не может быть отрицательным!");
                return;
            }
            if (!decimal.TryParse(DiscountBox.Text, out decimal discount) || discount < 0)
            {
                MessageBox.Show("Скидка не может быть отрицательной!");
                return;
            }

            try
            {
                if (!_isEditMode)
                {
                    _currentProduct = new Product
                    {
                        Article = ArticleBox.Text,
                        Name = NameBox.Text,
                        Description = DescBox.Text,
                        Unit = UnitBox.Text,
                        Price = price,
                        Count = count,
                        Discount = discount,
                        CategoryId = (CategoryCombo.SelectedItem as Category)?.Id,
                        ManufacturerId = (ManufacturerCombo.SelectedItem as Manufacturer)?.Id,
                        SupplierId = (SupplierCombo.SelectedItem as Supplier)?.Id
                    };
                    _context.Products.Add(_currentProduct);
                    _context.SaveChanges(); 

                    if (!string.IsNullOrEmpty(_selectedImagePath))
                    {
                        string newFileName = _currentProduct.Id + Path.GetExtension(_selectedImagePath);
                        SaveImageToFolder(_selectedImagePath, newFileName);
                        _currentProduct.ImagePath = newFileName;
                        _context.Products.Update(_currentProduct);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    _currentProduct.Article = ArticleBox.Text;
                    _currentProduct.Name = NameBox.Text;
                    _currentProduct.Description = DescBox.Text;
                    _currentProduct.Unit = UnitBox.Text;
                    _currentProduct.Price = price;
                    _currentProduct.Count = count;
                    _currentProduct.Discount = discount;
                    _currentProduct.CategoryId = (CategoryCombo.SelectedItem as Category)?.Id;
                    _currentProduct.ManufacturerId = (ManufacturerCombo.SelectedItem as Manufacturer)?.Id;
                    _currentProduct.SupplierId = (SupplierCombo.SelectedItem as Supplier)?.Id;

                    if (!string.IsNullOrEmpty(_selectedImagePath))
                    {
                        if (!string.IsNullOrEmpty(_currentProduct.ImagePath))
                        {
                            string oldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", _currentProduct.ImagePath);
                            if (File.Exists(oldPath))
                            {
                                try { File.Delete(oldPath); } catch { }
                            }
                        }

                        string newFileName = _currentProduct.Id + Path.GetExtension(_selectedImagePath);
                        SaveImageToFolder(_selectedImagePath, newFileName);
                        _currentProduct.ImagePath = newFileName;
                    }

                    _context.Products.Update(_currentProduct);
                    _context.SaveChanges();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void SaveImageToFolder(string sourcePath, string fileName)
        {
            string destFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            Directory.CreateDirectory(destFolder); 
            string destPath = Path.Combine(destFolder, fileName);
            File.Copy(sourcePath, destPath, true); 
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}