using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CinemaManagementSystem.Models;

public partial class CinemaManagementSystemDbContext : DbContext
{
    public CinemaManagementSystemDbContext()
    {
    }

    public CinemaManagementSystemDbContext(DbContextOptions<CinemaManagementSystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Showtime> Showtimes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("MyCnn"));
        }
    }
#warning 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B89B9DA965");

            entity.Property(e => e.CustomerId)
                .ValueGeneratedNever()
                .HasColumnName("CustomerID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(15);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movies__4BD2943A6EDA00C4");

            entity.Property(e => e.MovieId)
                .ValueGeneratedNever()
                .HasColumnName("MovieID");
            entity.Property(e => e.AgeRating).HasMaxLength(10);
            entity.Property(e => e.Genre).HasMaxLength(50);
            entity.Property(e => e.MovieName).HasMaxLength(100);
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.HasKey(e => e.ShowtimeId).HasName("PK__Showtime__32D31FC01A81911F");

            entity.Property(e => e.ShowtimeId)
                .ValueGeneratedNever()
                .HasColumnName("ShowtimeID");
            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.RoomName).HasMaxLength(50);
            entity.Property(e => e.ShowTime1).HasColumnName("ShowTime");

            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__Showtimes__Movie__4BAC3F29");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Tickets__712CC627AEE80A0D");

            entity.Property(e => e.TicketId)
                .ValueGeneratedNever()
                .HasColumnName("TicketID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.SeatNumber).HasMaxLength(10);
            entity.Property(e => e.ShowtimeId).HasColumnName("ShowtimeID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Tickets__Custome__534D60F1");

            entity.HasOne(d => d.ProcessedByNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ProcessedBy)
                .HasConstraintName("FK__Tickets__Process__5441852A");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ShowtimeId)
                .HasConstraintName("FK__Tickets__Showtim__52593CB8");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACE9AB4630");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
