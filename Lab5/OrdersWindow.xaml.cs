using System.Linq;
using System.Windows;
using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5
{
    public partial class OrdersWindow : Window
    {
        private string _userRole;

        public OrdersWindow(string role)
        {
            InitializeComponent();
            _userRole = role;
            UserInfoText.Text = $"Роль: {role}";

            ApplyRole();
            LoadOrders();
        }

        private void ApplyRole()
        {
            if (_userRole != "Администратор")
            {
                AdminPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadOrders()
        {
            using (var context = new Lab5BikeShopContext())
            {
                var orders = context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Status)
                    .Include(o => o.PickupPoint)
                    .ToList();

                OrdersGrid.ItemsSource = orders;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
