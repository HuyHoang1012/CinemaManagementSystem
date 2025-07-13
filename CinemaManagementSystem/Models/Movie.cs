using System;
using System.Collections.Generic;

namespace CinemaManagementSystem.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string? MovieName { get; set; }

    public string? Genre { get; set; }

    public int? Duration { get; set; }

    public string? AgeRating { get; set; }

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
