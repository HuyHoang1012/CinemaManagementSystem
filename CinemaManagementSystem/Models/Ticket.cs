using System;
using System.Collections.Generic;

namespace CinemaManagementSystem.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int ShowtimeId { get; set; }

    public int CustomerId { get; set; }

    public string? SeatNumber { get; set; }

    public decimal Price { get; set; }

    public DateTime? BookingDate { get; set; }

    public bool? IsPaid { get; set; }

    public int ProcessedBy { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual User ProcessedByNavigation { get; set; } = null!;

    public virtual Showtime Showtime { get; set; } = null!;
}
