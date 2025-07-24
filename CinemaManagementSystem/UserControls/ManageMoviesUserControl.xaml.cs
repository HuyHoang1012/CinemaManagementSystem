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
    /// Interaction logic for ManageMoviesUserControl.xaml
    /// </summary>
    public partial class ManageMoviesUserControl : UserControl
    {
        private CinemaManagementSystemDbContext con = new CinemaManagementSystemDbContext();
        public ManageMoviesUserControl()
        {
            InitializeComponent();
            LoadMovies();
        }

        private void LoadMovies()
        {
            dataGridMovies.ItemsSource = con.Movies.ToList();
        }

        private void ResetFields()
        {
            txtMovieID.Text = "";
            txtMovieName.Text = "";
            txtGenre.Text = "";
            txtDuration.Text = "";
            txtAgeRating.Text = "";
            txtSearch.Text = "";
            LoadMovies();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Movie movie = new Movie
                {
                    MovieName = txtMovieName.Text.Trim(),
                    Genre = txtGenre.Text.Trim(),
                    Duration = int.TryParse(txtDuration.Text.Trim(), out int dur) ? dur : null,
                    AgeRating = txtAgeRating.Text.Trim()
                };

                con.Movies.Add(movie);
                con.SaveChanges();
                MessageBox.Show("Thêm phim thành công!");
                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm phim: " + ex.Message);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtMovieID.Text.Trim(), out int id))
            {
                var movie = con.Movies.FirstOrDefault(m => m.MovieId == id);
                if (movie != null)
                {
                    movie.MovieName = txtMovieName.Text.Trim();
                    movie.Genre = txtGenre.Text.Trim();
                    movie.Duration = int.TryParse(txtDuration.Text.Trim(), out int dur) ? dur : null;
                    movie.AgeRating = txtAgeRating.Text.Trim();

                    con.SaveChanges();
                    MessageBox.Show("Cập nhật phim thành công!");
                    ResetFields();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy phim để cập nhật.");
                }
            }
            else
            {
                MessageBox.Show("ID không hợp lệ.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtMovieID.Text.Trim(), out int id))
            {
                var movie = con.Movies.FirstOrDefault(m => m.MovieId == id);
                if (movie != null)
                {
                    con.Movies.Remove(movie);
                    con.SaveChanges();
                    MessageBox.Show("Xóa phim thành công!");
                    ResetFields();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy phim để xóa.");
                }
            }
            else
            {
                MessageBox.Show("ID không hợp lệ.");
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetFields();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var results = con.Movies
                .Where(m =>
                    m.MovieName.ToLower().Contains(keyword) ||
                    m.Genre.ToLower().Contains(keyword) ||
                    m.AgeRating.ToLower().Contains(keyword) ||
                    m.MovieId.ToString().Contains(keyword))
                .ToList();

            dataGridMovies.ItemsSource = results;
        }

        private void DataGridMovies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridMovies.SelectedItem is Movie movie)
            {
                txtMovieID.Text = movie.MovieId.ToString();
                txtMovieName.Text = movie.MovieName;
                txtGenre.Text = movie.Genre;
                txtDuration.Text = movie.Duration?.ToString() ?? "";
                txtAgeRating.Text = movie.AgeRating;
            }
        }
    }
}
