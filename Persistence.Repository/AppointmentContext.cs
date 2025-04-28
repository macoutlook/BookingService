using Core.Domain;
using Microsoft.EntityFrameworkCore;
using Patient = Core.Domain.Patient;

namespace Persistence.AppointmentRepository;

/// <summary>
///     Database context from which db migration was generated.
///     To create database from migration, run below command in Package Manager Console:
///     > Update-Database
/// </summary>
public class AppointmentContext(DbContextOptions<AppointmentContext> options) : DbContext(options)
{
    public DbSet<Appointment> Appointment { get; set; }
    public DbSet<Schedule> Schedule { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Schedule>()
            .HasKey(s => s.Id);
        modelBuilder.Entity<Schedule>()
            .HasIndex(s => s.StartDate)
            .HasDatabaseName("IX_StartDate")
            .IsUnique()
            .IsClustered(false);
        
        modelBuilder.Entity<Appointment>()
            .HasKey(s => s.Id);
        
        modelBuilder.Entity<DaySchedule>()
            .HasKey(s => s.Id);
        
        modelBuilder.Entity<Facility>()
            .HasKey(s => s.Id);
        
        modelBuilder.Entity<Patient>()
            .HasKey(s => s.Id);
        
        modelBuilder.Entity<Slot>()
            .HasKey(s => s.Id);
        
        modelBuilder.Entity<WorkPeriod>()
            .HasKey(s => s.Id);
    }
}