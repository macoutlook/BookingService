using Core.Domain;
using Microsoft.EntityFrameworkCore;
using Patient = Core.Domain.Patient;

namespace Persistence.Repository;

/// <summary>
///     Database context from which db migration was generated.
///     To create database from migration, run below command in Package Manager Console:
///     > Update-Database
/// </summary>
public class AppointmentContext(DbContextOptions<AppointmentContext> options) : DbContext(options)
{
    public DbSet<Appointment> Appointment { get; set; }
    public DbSet<Schedule> Schedule { get; set; }
    public DbSet<Patient> Patient { get; set; }
    public DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Schedule>()
            .HasKey(s => s.StartDate);
        modelBuilder.Entity<Schedule>()
            .HasIndex(s => s.StartDate)
            .HasDatabaseName("IX_StartDate")
            .IsUnique()
            .IsClustered(false);
        
        modelBuilder.Entity<Appointment>()
            .HasKey(a => a.Id);
        
        modelBuilder.Entity<DaySchedule>()
            .HasKey(d => d.Id);
        
        modelBuilder.Entity<Facility>()
            .HasKey(f => f.Id);
        
        modelBuilder.Entity<Patient>()
            .HasKey(p => p.Id);
        
        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.Email)
            .IsUnique();
        
        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.Phone)
            .IsUnique();
        
        modelBuilder.Entity<Slot>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Slot>()
            .HasIndex(s => new { s.Start, s.End, s.ScheduleStartDate })
            .IsUnique();
        
        modelBuilder.Entity<WorkPeriod>()
            .HasKey(w => w.Id);
        
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Name)
            .IsUnique();
    }
}