using Core.Application;
using Core.Application.Contract;
using Core.Domain;
using Core.Persistence.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repository;

namespace Bootstrapper;

public static class Bootstrapper
{
    public static void RegisterApplication(this IServiceCollection services)
    {
        services.AddScoped<ISlotService, SlotService>();
    }

    public static void RegisterPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddDbContext<AppointmentContext>(options =>
            options.UseSqlServer(configuration.GetSection("ConnectionString").Value));
    }

    public static async Task InitializeDb(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new DbContextOptionsBuilder<AppointmentContext>()
            .UseSqlServer(configuration.GetSection("ConnectionString").Value).Options;
        await using var context = new AppointmentContext(options);

        if (await context.Database.EnsureCreatedAsync())
            await RunDataFeed(configuration);
    }

    private static async Task RunDataFeed(IConfiguration configuration)
    {
        var options = new DbContextOptionsBuilder<AppointmentContext>()
            .UseSqlServer(configuration.GetSection("ConnectionString").Value).Options;
        var context = new AppointmentContext(options);

        var startScheduleDateTime = DateTime.Now;
        var currentDay = startScheduleDateTime.DayOfWeek;

        if (currentDay is not DayOfWeek.Monday)
            for (var day = 0; day < 7; day++)
            {
                var date = startScheduleDateTime.AddDays(day);
                var dayOfWeek = date.DayOfWeek;
                if (dayOfWeek is DayOfWeek.Monday)
                    startScheduleDateTime = date;
            }

        var workPeriods = new List<WorkPeriod>
        {
            new()
            {
                StartHour = 9,
                EndHour = 17,
                LunchStartHour = 12,
                LunchEndHour = 13
            },
            new()
            {
                StartHour = 8,
                EndHour = 16,
                LunchStartHour = 11,
                LunchEndHour = 12
            },
            new()
            {
                StartHour = 10,
                EndHour = 18,
                LunchStartHour = 13,
                LunchEndHour = 14
            }
        };

        var slots = new List<Slot>
        {
            new()
            {
                Start = new DateTime(startScheduleDateTime.Year, startScheduleDateTime.Month, startScheduleDateTime.Day,
                    9, 0, 0),
                End = new DateTime(startScheduleDateTime.Year, startScheduleDateTime.Month, startScheduleDateTime.Day,
                    9, 30, 0),
                Day = DayOfWeek.Monday,
                ScheduleStartDate = DateOnly.FromDateTime(startScheduleDateTime.Date)
            },
            new()
            {
                Start = new DateTime(startScheduleDateTime.Year, startScheduleDateTime.Month,
                    startScheduleDateTime.Day + 1, 11, 30, 0),
                End = new DateTime(startScheduleDateTime.Year, startScheduleDateTime.Month,
                    startScheduleDateTime.Day + 1, 12, 00, 0),
                Day = DayOfWeek.Tuesday,
                ScheduleStartDate = DateOnly.FromDateTime(startScheduleDateTime.Date)
            }
        };

        var facility =
            new Facility
            {
                Name = "Central Hospital",
                Address = "123 Main St, Springfield"
            };

        var patients = new List<Patient>
        {
            new()
            {
                Name = "John",
                SecondName = "Doe",
                Email = "johndoe@example.com",
                Phone = "123-456-7890"
            },
            new()
            {
                Name = "Jane",
                SecondName = "Smith",
                Email = "janesmith@example.com",
                Phone = "098-765-4321"
            }
        };

        var daySchedules = new List<DaySchedule>
        {
            new()
            {
                Day = DayOfWeek.Monday,
                WorkPeriod = workPeriods[0]
            },
            new()
            {
                Day = DayOfWeek.Tuesday,
                WorkPeriod = workPeriods[1]
            },
            new()
            {
                Day = DayOfWeek.Wednesday,
                WorkPeriod = workPeriods[0]
            },
            new()
            {
                Day = DayOfWeek.Thursday,
                WorkPeriod = workPeriods[0]
            },
            new()
            {
                Day = DayOfWeek.Friday,
                WorkPeriod = workPeriods[1]
            }
        };

        var schedule =
            new Schedule
            {
                StartDate = DateOnly.FromDateTime(startScheduleDateTime.Date),
                Facility = facility,
                SlotDurationMinutes = 30,
                DaySchedules = daySchedules,
                BusySlots = slots
            };

        var appointments = new List<Appointment>
        {
            new()
            {
                Slot = slots[0],
                Comments = "Initial consultation",
                Patient = patients[0]
            },
            new()
            {
                Slot = slots[1],
                Comments = "Follow-up visit",
                Patient = patients[1]
            }
        };

        var user = new User
        {
            Name = "techuser",
            Password = "secretpassWord"
        };

        context.Add(user);
        context.Appointment.AddRange(appointments);
        context.Schedule.Add(schedule);

        await context.SaveChangesAsync();
        await context.DisposeAsync();
    }
}