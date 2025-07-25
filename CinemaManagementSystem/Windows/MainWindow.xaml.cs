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
using CinemaManagementSystem.UserControls;

namespace CinemaManagementSystem.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (Session.CurrentUser.Role != "Admin")
            {
                btnManageUsers.Visibility = Visibility.Collapsed;
            }

            MainContent.Content = new DashboardUserControl();
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DashboardUserControl();
        }
        private void ManageMovies_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ManageMoviesUserControl();
        }

        private void ManageShowtimes_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ManageShowtimesUserControl();
        }

        private void ManageTickets_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ManageTicketsUserControl(Session.CurrentUser.UserId);
        }

        private void ManageCustomers_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ManageCustomersUserControl();
        }

        private void ManageUsers_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ManageUsersUserControl();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Session.CurrentUser = null;

                LoginWindow login = new LoginWindow();
                login.Show();

                this.Close();
            }
        }

    }
}
