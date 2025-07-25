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
    /// Interaction logic for ManageTicketsUserControl.xaml
    /// </summary>
    public partial class ManageTicketsUserControl : UserControl
    {
        private CinemaManagementSystemDbContext con = new CinemaManagementSystemDbContext();
        private int currentUserId; // Lưu UserID người đăng nhập

        public ManageTicketsUserControl(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadTickets();
        }

        private void LoadTickets()
        {
            dataGridTickets.ItemsSource = con.Tickets.ToList();
        }

        private void ResetFields()
        {
            txtTicketId.Text = "";
            txtShowtimeId.Text = "";
            txtCustomerId.Text = "";
            txtSeatNumber.Text = "";
            txtPrice.Text = "";
            txtBookingDate.Text = "";
            txtIsPaid.Text = "";
            txtSearch.Text = "";
            LoadTickets();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtTicketId.Text.Trim(), out int ticketId))
                {
                    MessageBox.Show("Ticket ID không hợp lệ. Vui lòng nhập số nguyên.");
                    return;
                }
                if (ticketId < 1)
                {
                    MessageBox.Show("Ticket ID phải >= 1.");
                    return;
                }

                if (con.Tickets.Any(t => t.TicketId == ticketId))
                {
                    MessageBox.Show($"Ticket ID {ticketId} đã tồn tại, vui lòng chọn ID khác.");
                    return;
                }

                Ticket ticket = new Ticket
                {
                    TicketId = ticketId,
                    ShowtimeId = int.TryParse(txtShowtimeId.Text.Trim(), out int showtimeId) ? showtimeId : null,
                    CustomerId = int.TryParse(txtCustomerId.Text.Trim(), out int customerId) ? customerId : null,
                    SeatNumber = txtSeatNumber.Text.Trim(),
                    Price = int.TryParse(txtPrice.Text.Trim(), out int price) ? price : null,
                    BookingDate = DateOnly.TryParse(txtBookingDate.Text.Trim(), out DateOnly bookingDate) ? bookingDate : null,
                    IsPaid = bool.TryParse(txtIsPaid.Text.Trim(), out bool isPaid) ? isPaid : null,
                    ProcessedBy = currentUserId
                };

                con.Tickets.Add(ticket);
                con.SaveChanges();
                MessageBox.Show("Thêm vé thành công!");
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm vé: " + ex.Message);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTicketId.Text.Trim(), out int id))
            {
                var ticket = con.Tickets.FirstOrDefault(t => t.TicketId == id);
                if (ticket != null)
                {
                    ticket.ShowtimeId = int.TryParse(txtShowtimeId.Text.Trim(), out int showtimeId) ? showtimeId : null;
                    ticket.CustomerId = int.TryParse(txtCustomerId.Text.Trim(), out int customerId) ? customerId : null;
                    ticket.SeatNumber = txtSeatNumber.Text.Trim();
                    ticket.Price = int.TryParse(txtPrice.Text.Trim(), out int price) ? price : null;
                    ticket.BookingDate = DateOnly.TryParse(txtBookingDate.Text.Trim(), out DateOnly bookingDate) ? bookingDate : null;
                    ticket.IsPaid = bool.TryParse(txtIsPaid.Text.Trim(), out bool isPaid) ? isPaid : null;
                    ticket.ProcessedBy = currentUserId; // Lưu UserID người chỉnh sửa

                    con.SaveChanges();
                    MessageBox.Show("Cập nhật vé thành công!");
                    ResetFields();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy vé để cập nhật.");
                }
            }
            else
            {
                MessageBox.Show("Ticket ID không hợp lệ.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTicketId.Text.Trim(), out int id))
            {
                var ticket = con.Tickets.FirstOrDefault(t => t.TicketId == id);
                if (ticket != null)
                {
                    con.Tickets.Remove(ticket);
                    con.SaveChanges();
                    MessageBox.Show("Xóa vé thành công!");
                    ResetFields();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy vé để xóa.");
                }
            }
            else
            {
                MessageBox.Show("Ticket ID không hợp lệ.");
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetFields();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var results = con.Tickets
                .Where(t =>
                    t.TicketId.ToString().Contains(keyword) ||
                    (t.SeatNumber != null && t.SeatNumber.ToLower().Contains(keyword)) ||
                    (t.Price.HasValue && t.Price.Value.ToString().Contains(keyword)) ||
                    (t.BookingDate.HasValue && t.BookingDate.Value.ToString().Contains(keyword)) ||
                    (t.CustomerId.HasValue && t.CustomerId.Value.ToString().Contains(keyword)) ||
                    (t.ShowtimeId.HasValue && t.ShowtimeId.Value.ToString().Contains(keyword))
                )
                .ToList();

            dataGridTickets.ItemsSource = results;
        }

        private void DataGridTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridTickets.SelectedItem is Ticket ticket)
            {
                txtTicketId.Text = ticket.TicketId.ToString();
                txtShowtimeId.Text = ticket.ShowtimeId?.ToString() ?? "";
                txtCustomerId.Text = ticket.CustomerId?.ToString() ?? "";
                txtSeatNumber.Text = ticket.SeatNumber ?? "";
                txtPrice.Text = ticket.Price?.ToString() ?? "";
                txtBookingDate.Text = ticket.BookingDate?.ToString("yyyy-MM-dd") ?? "";
                txtIsPaid.Text = ticket.IsPaid?.ToString() ?? "";
            }
        }

        private void DataGridTickets_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column is DataGridCheckBoxColumn && e.Column.Header.ToString() == "Is Paid")
            {
                if (e.Row.Item is Ticket ticket)
                {
                    var element = e.EditingElement as CheckBox;
                    bool? newValue = element?.IsChecked;

                    if (MessageBox.Show($"Bạn có chắc muốn đổi IsPaid thành {newValue}?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ticket.IsPaid = newValue;
                        ticket.ProcessedBy = currentUserId; // Ghi UserID khi chỉnh
                        con.SaveChanges();
                        MessageBox.Show("Cập nhật IsPaid thành công.");
                        LoadTickets();
                    }
                    else
                    {
                        Dispatcher.InvokeAsync(() => LoadTickets());
                    }
                }
            }
        }
    }
}