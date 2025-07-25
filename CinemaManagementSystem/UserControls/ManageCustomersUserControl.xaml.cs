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
    /// Interaction logic for ManageCustomersUserControl.xaml
    /// </summary>
    public partial class ManageCustomersUserControl : UserControl
    {
        private CinemaManagementSystemDbContext con = new CinemaManagementSystemDbContext();

        public ManageCustomersUserControl()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            dataGridCustomers.ItemsSource = con.Customers.ToList();
        }

        private void ResetFields()
        {
            txtCustomerID.Text = "";
            txtFullName.Text = "";
            txtPhone.Text = "";
            txtSearch.Text = "";
            LoadCustomers();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtCustomerID.Text.Trim(), out int customerId))
                {
                    MessageBox.Show("Customer ID không hợp lệ.");
                    return;
                }

                if (con.Customers.Any(c => c.CustomerId == customerId))
                {
                    MessageBox.Show("Customer ID đã tồn tại.");
                    return;
                }

                Customer customer = new Customer
                {
                    CustomerId = customerId,
                    FullName = txtFullName.Text.Trim(),
                    Phone = txtPhone.Text.Trim()
                };

                con.Customers.Add(customer);
                con.SaveChanges();
                MessageBox.Show("Thêm khách hàng thành công.");
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm khách hàng: " + ex.Message);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCustomerID.Text.Trim(), out int customerId))
            {
                var customer = con.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                if (customer != null)
                {
                    customer.FullName = txtFullName.Text.Trim();
                    customer.Phone = txtPhone.Text.Trim();
                    con.SaveChanges();
                    MessageBox.Show("Cập nhật khách hàng thành công.");
                    ResetFields();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khách hàng để cập nhật.");
                }
            }
            else
            {
                MessageBox.Show("Customer ID không hợp lệ.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCustomerID.Text.Trim(), out int customerId))
            {
                var customer = con.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                if (customer != null)
                {
                    con.Customers.Remove(customer);
                    con.SaveChanges();
                    MessageBox.Show("Xóa khách hàng thành công.");
                    ResetFields();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khách hàng để xóa.");
                }
            }
            else
            {
                MessageBox.Show("Customer ID không hợp lệ.");
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetFields();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var results = con.Customers.Where(c =>
                c.CustomerId.ToString().Contains(keyword) ||
                (!string.IsNullOrEmpty(c.FullName) && c.FullName.ToLower().Contains(keyword)) ||
                (!string.IsNullOrEmpty(c.Phone) && c.Phone.Contains(keyword))
            ).ToList();

            dataGridCustomers.ItemsSource = results;
        }

        private void DataGridCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridCustomers.SelectedItem is Customer customer)
            {
                txtCustomerID.Text = customer.CustomerId.ToString();
                txtFullName.Text = customer.FullName ?? "";
                txtPhone.Text = customer.Phone ?? "";
            }
        }
    }
}

