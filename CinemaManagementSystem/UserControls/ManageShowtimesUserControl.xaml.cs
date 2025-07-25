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
    /// Interaction logic for ManageShowtimesUserControl.xaml
    /// </summary>
    public partial class ManageShowtimesUserControl : UserControl
    {
        private CinemaManagementSystemDbContext con = new CinemaManagementSystemDbContext();

        public ManageShowtimesUserControl()
        {
            InitializeComponent();
            LoadShowtimes();
        }

        private void LoadShowtimes()
        {
            dataGridShowtimes.ItemsSource = con.Showtimes.ToList();
        }

        private void ResetFields()
        {
            txtShowtimeID.Text = "";
            txtMovieID.Text = "";
            txtShowDate.Text = "";
            txtShowTime.Text = "";
            txtRoomName.Text = "";
            txtSearch.Text = "";
            LoadShowtimes();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtShowtimeID.Text.Trim(), out int showtimeId) || showtimeId < 1)
                {
                    MessageBox.Show("Showtime ID không hợp lệ hoặc nhỏ hơn 1.");
                    return;
                }

                if (!int.TryParse(txtMovieID.Text.Trim(), out int movieId) || movieId < 1)
                {
                    MessageBox.Show("Movie ID không hợp lệ hoặc nhỏ hơn 1.");
                    return;
                }

                if (!DateOnly.TryParse(txtShowDate.Text.Trim(), out DateOnly showDate))
                {
                    MessageBox.Show("Ngày chiếu không hợp lệ (định dạng yyyy-MM-dd).");
                    return;
                }

                if (!TimeOnly.TryParse(txtShowTime.Text.Trim(), out TimeOnly showTime))
                {
                    MessageBox.Show("Giờ chiếu không hợp lệ (định dạng HH:mm).");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtRoomName.Text.Trim()))
                {
                    MessageBox.Show("Room Name không được bỏ trống.");
                    return;
                }

                // Check trùng ID
                var existing = con.Showtimes.FirstOrDefault(s => s.ShowtimeId == showtimeId);
                if (existing != null)
                {
                    MessageBox.Show($"Showtime ID {showtimeId} đã tồn tại.");
                    return;
                }

                var showtime = new Showtime
                {
                    ShowtimeId = showtimeId,
                    MovieId = movieId,
                    ShowDate = showDate,
                    ShowTime1 = showTime,
                    RoomName = txtRoomName.Text.Trim()
                };

                con.Showtimes.Add(showtime);
                con.SaveChanges();
                MessageBox.Show("Thêm lịch chiếu thành công!");
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm lịch chiếu: " + ex.Message);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtShowtimeID.Text.Trim(), out int showtimeId))
            {
                var showtime = con.Showtimes.FirstOrDefault(s => s.ShowtimeId == showtimeId);
                if (showtime != null)
                {
                    if (!int.TryParse(txtMovieID.Text.Trim(), out int movieId) || movieId < 1)
                    {
                        MessageBox.Show("Movie ID không hợp lệ hoặc nhỏ hơn 1.");
                        return;
                    }

                    if (!DateOnly.TryParse(txtShowDate.Text.Trim(), out DateOnly showDate))
                    {
                        MessageBox.Show("Ngày chiếu không hợp lệ (yyyy-MM-dd).");
                        return;
                    }

                    if (!TimeOnly.TryParse(txtShowTime.Text.Trim(), out TimeOnly showTime))
                    {
                        MessageBox.Show("Giờ chiếu không hợp lệ (HH:mm).");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtRoomName.Text.Trim()))
                    {
                        MessageBox.Show("Room Name không được bỏ trống.");
                        return;
                    }

                    showtime.MovieId = movieId;
                    showtime.ShowDate = showDate;
                    showtime.ShowTime1 = showTime;
                    showtime.RoomName = txtRoomName.Text.Trim();

                    con.SaveChanges();
                    MessageBox.Show("Cập nhật lịch chiếu thành công!");
                    ResetFields();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy lịch chiếu để cập nhật.");
                }
            }
            else
            {
                MessageBox.Show("Showtime ID không hợp lệ.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtShowtimeID.Text.Trim(), out int showtimeId))
            {
                var showtime = con.Showtimes.FirstOrDefault(s => s.ShowtimeId == showtimeId);
                if (showtime != null)
                {
                    con.Showtimes.Remove(showtime);
                    con.SaveChanges();
                    MessageBox.Show("Xóa lịch chiếu thành công!");
                    ResetFields();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy lịch chiếu để xóa.");
                }
            }
            else
            {
                MessageBox.Show("Showtime ID không hợp lệ.");
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetFields();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var results = con.Showtimes
                .Where(s =>
                    s.ShowtimeId.ToString().Contains(keyword) ||
                    (s.MovieId.HasValue && s.MovieId.ToString().Contains(keyword)) ||
                    (s.ShowDate.HasValue && s.ShowDate.ToString().Contains(keyword)) ||
                    (s.ShowTime1.HasValue && s.ShowTime1.ToString().Contains(keyword)) ||
                    (s.RoomName != null && s.RoomName.ToLower().Contains(keyword)))
                .ToList();

            dataGridShowtimes.ItemsSource = results;
        }

        private void DataGridShowtimes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridShowtimes.SelectedItem is Showtime showtime)
            {
                txtShowtimeID.Text = showtime.ShowtimeId.ToString();
                txtMovieID.Text = showtime.MovieId?.ToString() ?? "";
                txtShowDate.Text = showtime.ShowDate?.ToString("yyyy-MM-dd") ?? "";
                txtShowTime.Text = showtime.ShowTime1?.ToString("HH:mm") ?? "";
                txtRoomName.Text = showtime.RoomName ?? "";
            }
        }
    }
}
