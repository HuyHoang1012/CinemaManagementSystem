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
using CinemaManagementSystem.Models;
using static System.Collections.Specialized.BitVector32;

namespace CinemaManagementSystem
{
    public static class Session
    {
        public static User CurrentUser { get; set; }
    }
    public partial class LoginWindow : Window
    {
        private CinemaManagementSystemDbContext con;
        public LoginWindow()
        {
            InitializeComponent();
            con = new CinemaManagementSystemDbContext();
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
                    Session.CurrentUser = user;
                    if (user.Role == "Admin")
                    {
                        MessageBox.Show("Chào mừng Admin!", "Đăng nhập thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();

                    }
                    else if (user.Role == "Nhân viên")
                    {
                        MessageBox.Show("Chào mừng User!", "Đăng nhập thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
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
