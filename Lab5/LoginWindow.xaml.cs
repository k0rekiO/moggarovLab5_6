using System.Linq;
using System.Windows;
using Lab5.Models;

namespace Lab5
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new Lab5BikeShopContext())
            {
                var user = context.Users
                    .FirstOrDefault(u => u.Login == LoginBox.Text && u.Password == PasswordBox.Password);

                if (user != null)
                {
                    string role = context.Roles
                        .FirstOrDefault(r => r.Id == user.RoleId)?.Name ?? "Авторизированный клиент";

                    var mainWindow = new MainWindow(role);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow("Гость");
            mainWindow.Show();
            this.Close();
        }
    }
}