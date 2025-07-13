using System;
using System.Collections.Generic;

namespace CinemaManagementSystem.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int? ShowtimeId { get; set; }

    public int? CustomerId { get; set; }

    public string? SeatNumber { get; set; }

    public int? Price { get; set; }

    public DateOnly? BookingDate { get; set; }

    public bool? IsPaid { get; set; }

    public int? ProcessedBy { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual User? ProcessedByNavigation { get; set; }

    public virtual Showtime? Showtime { get; set; }
}
