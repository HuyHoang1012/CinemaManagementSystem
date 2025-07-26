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
        private int currentUserId;

        public ManageTicketsUserControl(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadTickets();
            LoadComboBoxes();
        }

        private void LoadTickets()
        {
            dataGridTickets.ItemsSource = con.Tickets.ToList();
        }

        private void LoadComboBoxes()
        {
            cbShowtimeId.ItemsSource = con.Showtimes.Select(s => s.ShowtimeId).ToList();
            cbCustomerId.ItemsSource = con.Customers.Select(c => c.CustomerId).ToList();
            cbShowtimeForSeats.ItemsSource = con.Showtimes.Select(s => s.ShowtimeId).ToList();
        }

        private void ResetFields()
        {
            txtTicketId.Text = "";
            cbShowtimeId.SelectedIndex = -1;
            cbCustomerId.SelectedIndex = -1;
            txtSeatNumber.Text = "";
            txtPrice.Text = "";
            cbIsPaid.Text = "";
            txtSearch.Text = "";
            LoadTickets();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtTicketId.Text.Trim(), out int ticketId))
                {
                    MessageBox.Show("Ticket ID không hợp lệ.");
                    return;
                }

                if (con.Tickets.Any(t => t.TicketId == ticketId))
                {
                    MessageBox.Show($"Ticket ID {ticketId} đã tồn tại.");
                    return;
                }

                int? showtimeId = cbShowtimeId.SelectedItem as int?;
                string seatNumber = txtSeatNumber.Text.Trim();

                if (con.Tickets.Any(t => t.ShowtimeId == showtimeId && t.SeatNumber == seatNumber))
                {
                    MessageBox.Show($"Ghế {seatNumber} đã được đặt trong Showtime ID {showtimeId}.");
                    return;
                }

                Ticket ticket = new Ticket
                {
                    TicketId = ticketId,
                    ShowtimeId = showtimeId,
                    CustomerId = cbCustomerId.SelectedItem as int?,
                    SeatNumber = seatNumber,
                    Price = int.TryParse(txtPrice.Text.Trim(), out int price) ? price : null,
                    BookingDate = DateOnly.FromDateTime(DateTime.Now),
                    IsPaid = bool.TryParse(cbIsPaid.Text.Trim(), out bool isPaid) ? isPaid : null,
                    ProcessedBy = currentUserId
                };

                con.Tickets.Add(ticket);
                con.SaveChanges();
                MessageBox.Show("Thêm vé thành công.");
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm vé: " + ex.Message);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTicketId.Text.Trim(), out int ticketId))
            {
                var ticket = con.Tickets.FirstOrDefault(t => t.TicketId == ticketId);
                if (ticket != null)
                {
                    int? newShowtimeId = cbShowtimeId.SelectedItem as int?;
                    string newSeatNumber = txtSeatNumber.Text.Trim();

                    if (con.Tickets.Any(t => t.TicketId != ticketId &&
                                             t.ShowtimeId == newShowtimeId &&
                                             t.SeatNumber == newSeatNumber))
                    {
                        MessageBox.Show($"Ghế {newSeatNumber} đã được đặt trong Showtime ID {newShowtimeId}.");
                        return;
                    }

                    ticket.ShowtimeId = newShowtimeId;
                    ticket.CustomerId = cbCustomerId.SelectedItem as int?;
                    ticket.SeatNumber = newSeatNumber;
                    ticket.Price = int.TryParse(txtPrice.Text.Trim(), out int price) ? price : null;
                    ticket.BookingDate = DateOnly.FromDateTime(DateTime.Now);
                    ticket.IsPaid = bool.TryParse(cbIsPaid.Text.Trim(), out bool isPaid) ? isPaid : null;
                    ticket.ProcessedBy = currentUserId;

                    con.SaveChanges();
                    MessageBox.Show("Cập nhật vé thành công.");
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
            if (int.TryParse(txtTicketId.Text.Trim(), out int ticketId))
            {
                var ticket = con.Tickets.FirstOrDefault(t => t.TicketId == ticketId);
                if (ticket != null)
                {
                    con.Tickets.Remove(ticket);
                    con.SaveChanges();
                    MessageBox.Show("Xóa vé thành công.");
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
            var results = con.Tickets.Where(t =>
                t.TicketId.ToString().Contains(keyword) ||
                (t.SeatNumber != null && t.SeatNumber.ToLower().Contains(keyword)) ||
                (t.Price.HasValue && t.Price.Value.ToString().Contains(keyword)) ||
                (t.BookingDate.HasValue && t.BookingDate.Value.ToString().Contains(keyword)) ||
                (t.CustomerId.HasValue && t.CustomerId.Value.ToString().Contains(keyword)) ||
                (t.ShowtimeId.HasValue && t.ShowtimeId.Value.ToString().Contains(keyword))
            ).ToList();

            dataGridTickets.ItemsSource = results;
        }

        private void DataGridTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridTickets.SelectedItem is Ticket ticket)
            {
                txtTicketId.Text = ticket.TicketId.ToString();
                cbShowtimeId.SelectedItem = ticket.ShowtimeId;
                cbCustomerId.SelectedItem = ticket.CustomerId;
                txtSeatNumber.Text = ticket.SeatNumber ?? "";
                txtPrice.Text = ticket.Price?.ToString() ?? "";
                cbIsPaid.Text = ticket.IsPaid?.ToString() ?? "";
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
                        ticket.ProcessedBy = currentUserId;
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

        private void ViewAvailableSeats_Click(object sender, RoutedEventArgs e)
        {
            if (cbShowtimeForSeats.SelectedItem is int showtimeId)
            {
                List<string> allSeats = GenerateSeatList();
                var bookedSeats = con.Tickets
                    .Where(t => t.ShowtimeId == showtimeId)
                    .Select(t => t.SeatNumber)
                    .ToList();

                var availableSeats = allSeats.Except(bookedSeats).ToList();

                MessageBox.Show(
                    availableSeats.Count > 0
                        ? "Ghế còn trống:\n" + string.Join(", ", availableSeats)
                        : "Tất cả các ghế đã được đặt trong lịch chiếu này.",
                    "Kết quả"
                );
            }
            else
            {
                MessageBox.Show("Vui lòng chọn Showtime ID để xem ghế trống.");
            }
        }

        private void CalculateRevenue_Click(object sender, RoutedEventArgs e)
        {
            var totalRevenue = con.Tickets
                .Where(t => t.IsPaid == true && t.Price.HasValue)
                .Sum(t => t.Price.Value);

            MessageBox.Show($"Doanh thu (đã thanh toán): {totalRevenue:N0} VND", "Thống kê");
        }

        private void CalculateExpectedRevenue_Click(object sender, RoutedEventArgs e)
        {
            var expectedRevenue = con.Tickets
                .Where(t => t.Price.HasValue)
                .Sum(t => t.Price.Value);

            MessageBox.Show($"Doanh thu dự kiến: {expectedRevenue:N0} VND", "Thống kê");
        }

        private List<string> GenerateSeatList()
        {
            List<string> seats = new List<string>();
            string[] rows = { "A", "B", "C", "D" };
            for (int i = 1; i <= 8; i++)
            {
                foreach (var row in rows)
                {
                    seats.Add(row + i);
                }
            }
            return seats;
        }
    }
}