using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CinemaManagementSystem.Models;

namespace CinemaManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// //
    public partial class MainWindow : Window
    {
        private CinemaManagementSystemDbContext con;
        public MainWindow()
        {
            con = new CinemaManagementSystemDbContext();
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            String username = txtUsername.Text.Trim();
            String password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtMessage.Text = "Vui lòng nhập tên đăng nhập và mật khẩu.";
                return;
            }

            try
            {
                var user = con.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    if (user.Role == "Admin")
                    {
                        MessageBox.Show("Chào mừng Admin!", "Đăng nhập thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else if (user.Role == "Nhân viên")
                    {
                        MessageBox.Show("Chào mừng User!", "Đăng nhập thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        txtMessage.Text = "Vai trò người dùng không hợp lệ.";

                    }
                }
                else
                {
                    txtMessage.Text = "Tên đăng nhập hoặc mật khẩu không đúng.";
                }
            }
            catch (Exception ex)
            {
                txtMessage.Text = "Đã xảy ra lỗi: " + ex.Message;
            }
        }
    }
}