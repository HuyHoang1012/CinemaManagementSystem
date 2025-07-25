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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CinemaManagementSystem.Models;

namespace CinemaManagementSystem.UserControls
{
    /// <summary>
    /// Interaction logic for ManageUsersUserControl.xaml
    /// </summary>
    public partial class ManageUsersUserControl : UserControl
    {
        private CinemaManagementSystemDbContext con = new CinemaManagementSystemDbContext();

        public ManageUsersUserControl()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            dataGridUsers.ItemsSource = con.Users.ToList();
        }

        private void ResetFields()
        {
            txtUserID.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
            cbRole.SelectedIndex = -1;
            txtSearch.Text = "";
            LoadUsers();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtUserID.Text.Trim(), out int userId))
                {
                    MessageBox.Show("User ID không hợp lệ. Vui lòng nhập số nguyên.");
                    return;
                }
                if (string.IsNullOrEmpty(txtUsername.Text.Trim()) || string.IsNullOrEmpty(txtPassword.Text.Trim()) || cbRole.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng điền đầy đủ Username, Password và chọn Role.");
                    return;
                }

                if (con.Users.Any(u => u.UserId == userId))
                {
                    MessageBox.Show($"User ID {userId} đã tồn tại, vui lòng chọn ID khác.");
                    return;
                }

                string username = txtUsername.Text.Trim();

                if (con.Users.Any(u => u.Username != null && u.Username.ToLower() == username.ToLower()))
                {
                    MessageBox.Show($"Username '{username}' đã tồn tại, vui lòng chọn Username khác.");
                    return;
                }

                string selectedRole = ((ComboBoxItem)cbRole.SelectedItem).Content.ToString();

                User user = new User
                {
                    UserId = userId,
                    Username = username,
                    Password = txtPassword.Text.Trim(),
                    Role = selectedRole
                };

                con.Users.Add(user);
                con.SaveChanges();
                MessageBox.Show("Thêm User thành công!");
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm User: " + ex.Message);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtUserID.Text.Trim(), out int userId))
                {
                    MessageBox.Show("User ID không hợp lệ.");
                    return;
                }

                var user = con.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    if (string.IsNullOrEmpty(txtUsername.Text.Trim()) || string.IsNullOrEmpty(txtPassword.Text.Trim()) || cbRole.SelectedItem == null)
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ Username, Password và chọn Role.");
                        return;
                    }

                    string username = txtUsername.Text.Trim();

                    if (con.Users.Any(u => u.Username != null && u.Username.ToLower() == username.ToLower() && u.UserId != userId))
                    {
                        MessageBox.Show($"Username '{username}' đã tồn tại, vui lòng chọn Username khác.");
                        return;
                    }

                    string selectedRole = ((ComboBoxItem)cbRole.SelectedItem).Content.ToString();

                    user.Username = username;
                    user.Password = txtPassword.Text.Trim();
                    user.Role = selectedRole;

                    con.SaveChanges();
                    MessageBox.Show("Cập nhật User thành công!");
                    ResetFields();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy User để cập nhật.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật User: " + ex.Message);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtUserID.Text.Trim(), out int userId))
                {
                    MessageBox.Show("User ID không hợp lệ.");
                    return;
                }

                var user = con.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    if (MessageBox.Show($"Bạn có chắc chắn muốn xóa User {user.Username}?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        con.Users.Remove(user);
                        con.SaveChanges();
                        MessageBox.Show("Xóa User thành công!");
                        ResetFields();
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy User để xóa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa User: " + ex.Message);
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim().ToLower();
                var results = con.Users.Where(u =>
                    u.UserId.ToString().Contains(keyword) ||
                    (!string.IsNullOrEmpty(u.Username) && u.Username.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.Role) && u.Role.ToLower().Contains(keyword))
                ).ToList();

                dataGridUsers.ItemsSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message);
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetFields();
        }

        private void DataGridUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridUsers.SelectedItem is User user)
            {
                txtUserID.Text = user.UserId.ToString();
                txtUsername.Text = user.Username ?? "";
                txtPassword.Text = user.Password ?? "";
                if (!string.IsNullOrEmpty(user.Role))
                {
                    foreach (ComboBoxItem item in cbRole.Items)
                    {
                        if (item.Content.ToString() == user.Role)
                        {
                            cbRole.SelectedItem = item;
                            break;
                        }
                    }
                }
                else
                {
                    cbRole.SelectedIndex = -1;
                }
            }
        }
    }
}