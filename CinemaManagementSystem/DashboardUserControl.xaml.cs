using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace CinemaManagementSystem
{
    /// <summary>
    /// Interaction logic for DashboardUserControl.xaml
    /// </summary>
    public partial class DashboardUserControl : UserControl
    {
        private CinemaManagementSystemDbContext con;
        public DashboardUserControl()
        {
            InitializeComponent();
            con = new CinemaManagementSystemDbContext();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                int movieCount = con.Movies.Count();
                txtMoviesCount.Text = movieCount.ToString();

                DateOnly today = DateOnly.FromDateTime(DateTime.Today);
                int showtimeToday = con.Showtimes.Count(s => s.ShowDate.HasValue && s.ShowDate.Value == today);
                txtShowtimesToday.Text = showtimeToday.ToString();

                int ticketsToday = con.Tickets.Count(t => t.BookingDate == today);
                txtTicketsToday.Text = ticketsToday.ToString();

                decimal totalRevenueToday = con.Tickets
                    .Where(t => t.BookingDate == today)
                    .Sum(t => (decimal?)t.Price) ?? 0;
                txtRevenueToday.Text = totalRevenueToday.ToString("C0", CultureInfo.GetCultureInfo("vi-VN"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
