using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Application;
using Core.Domain;
using Core.Exceptions;
using Core.Persistence.Contract;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Application.UnitTest;

public class SlotServiceTests
{
    private readonly IAppointmentRepository _appointmentRepositoryMock;
    private readonly IPatientRepository _patientRepositoryMock;
    private readonly SlotService _slotService;

    public SlotServiceTests()
    {
        _appointmentRepositoryMock = Substitute.For<IAppointmentRepository>();
        _patientRepositoryMock = Substitute.For<IPatientRepository>();
        _slotService = new SlotService(_appointmentRepositoryMock, _patientRepositoryMock);
    }

    [Fact]
    public async Task TakeSlotAsync_ProperSlotGiven_ReturnsAppointmentId()
    {
        // Arrange
        var startDate = new DateOnly(2025, 5, 5);
        var facility = new Facility
        {
            Id = 1,
            Name = "Policlinico Modica",
            Address = "Via San Giorgio 1, 97015 Modica, IT"
        };

        var workPeriod = new WorkPeriod
        {
            Id = 1,
            StartHour = 8,
            EndHour = 16,
            LunchStartHour = 12,
            LunchEndHour = 13
        };

        var dayScheduleMonday = new DaySchedule
        {
            Id = 1,
            Day = DayOfWeek.Monday,
            WorkPeriod = workPeriod
        };

        var dayScheduleTuesday = new DaySchedule
        {
            Id = 2,
            Day = DayOfWeek.Tuesday,
            WorkPeriod = workPeriod
        };

        var busySlot = new Slot
        {
            Id = 1,
            Start = new DateTime(2025, 5, 5, 9, 0, 0),
            End = new DateTime(2025, 5, 5, 9, 30, 0),
            Day = DayOfWeek.Monday,
            ScheduleStartDate = startDate
        };

        var expectedSchedule = new Schedule
        {
            Facility = facility,
            SlotDurationMinutes = 30,
            DaySchedules = new List<DaySchedule> { dayScheduleMonday, dayScheduleTuesday },
            StartDate = startDate,
            BusySlots = new List<Slot> { busySlot }
        };

        var appointment = new Appointment
        {
            Id = 1, // Example ID
            Slot = new Slot
            {
                Id = 1, // Example Slot ID
                Start = new DateTime(2025, 5, 1, 9, 0, 0), // Example start time
                End = new DateTime(2025, 5, 1, 10, 0, 0), // Example end time
                Day = DayOfWeek.Thursday, // Example day
                ScheduleStartDate = new DateOnly(2025, 5, 1) // Example schedule start date
            },
            Comments = "Initial consultation", // Optional comments
            Patient = new Patient
            {
                Id = 1, // Example Patient ID
                Name = "John", // Example first name
                SecondName = "Doe", // Example second name
                Email = "john.doe@example.com", // Example email
                Phone = "+1234567890" // Example phone number
            }
        };
        _patientRepositoryMock.GetPatientIdByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(1UL);
        _appointmentRepositoryMock.GetScheduleAsync(Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(expectedSchedule);
        _appointmentRepositoryMock.AddAsync(Arg.Any<Appointment>(), Arg.Any<CancellationToken>())
            .Returns(1UL);

        // Act
        var result = await _slotService.TakeSlotAsync(appointment);

        // Assert
        result.Should().Be(1UL);
    }

    [Fact]
    public async Task TakeSlotAsync_PatientNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 1, // Example ID
            Slot = new Slot
            {
                Id = 1, // Example Slot ID
                Start = new DateTime(2025, 5, 1, 9, 0, 0), // Example start time
                End = new DateTime(2025, 5, 1, 10, 0, 0), // Example end time
                Day = DayOfWeek.Thursday, // Example day
                ScheduleStartDate = new DateOnly(2025, 5, 1) // Example schedule start date
            },
            Comments = "Initial consultation", // Optional comments
            Patient = new Patient
            {
                Id = 1, // Example Patient ID
                Name = "John", // Example first name
                SecondName = "Doe", // Example second name
                Email = "john.doe@example.com", // Example email
                Phone = "+1234567890" // Example phone number
            }
        };
        _patientRepositoryMock.GetPatientIdByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((ulong?)null);

        // Act & Assert
        await _slotService.Invoking(s => s.TakeSlotAsync(appointment))
            .Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage("Given patient cannot be found.");
    }

    [Fact]
    public async Task TakeSlotAsync_ScheduleNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 1, // Example ID
            Slot = new Slot
            {
                Id = 1, // Example Slot ID
                Start = new DateTime(2025, 5, 1, 9, 0, 0), // Example start time
                End = new DateTime(2025, 5, 1, 10, 0, 0), // Example end time
                Day = DayOfWeek.Thursday, // Example day
                ScheduleStartDate = new DateOnly(2025, 5, 1) // Example schedule start date
            },
            Comments = "Initial consultation", // Optional comments
            Patient = new Patient
            {
                Id = 1, // Example Patient ID
                Name = "John", // Example first name
                SecondName = "Doe", // Example second name
                Email = "john.doe@example.com", // Example email
                Phone = "+1234567890" // Example phone number
            }
        };
        _patientRepositoryMock.GetPatientIdByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(1UL);
        _appointmentRepositoryMock.GetScheduleAsync(Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns((Schedule?)null);

        // Act & Assert
        await _slotService.Invoking(s => s.TakeSlotAsync(appointment))
            .Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage("Schedule cannot be found.");
    }

    [Fact]
    public async Task GetAvailabilityAsync_ValidDateGiven_ReturnsSchedule()
    {
        // Arrange
        var startDate = new DateOnly(2025, 5, 5);
        var facility = new Facility
        {
            Id = 1,
            Name = "Policlinico Modica",
            Address = "Via San Giorgio 1, 97015 Modica, IT"
        };

        var workPeriod = new WorkPeriod
        {
            Id = 1,
            StartHour = 8,
            EndHour = 16,
            LunchStartHour = 12,
            LunchEndHour = 13
        };

        var dayScheduleMonday = new DaySchedule
        {
            Id = 1,
            Day = DayOfWeek.Monday,
            WorkPeriod = workPeriod
        };

        var dayScheduleTuesday = new DaySchedule
        {
            Id = 2,
            Day = DayOfWeek.Tuesday,
            WorkPeriod = workPeriod
        };

        var busySlot = new Slot
        {
            Id = 1,
            Start = new DateTime(2025, 5, 5, 9, 0, 0),
            End = new DateTime(2025, 5, 5, 9, 30, 0),
            Day = DayOfWeek.Monday,
            ScheduleStartDate = startDate
        };

        var expectedSchedule = new Schedule
        {
            Facility = facility,
            SlotDurationMinutes = 30,
            DaySchedules = new List<DaySchedule> { dayScheduleMonday, dayScheduleTuesday },
            StartDate = startDate,
            BusySlots = new List<Slot> { busySlot }
        };

        _appointmentRepositoryMock.GetScheduleAsync(Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Schedule?>(expectedSchedule));

        var slotService = new SlotService(_appointmentRepositoryMock, _patientRepositoryMock);

        // Act
        var result = await slotService.GetAvailabilityAsync("20250505");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedSchedule);
    }
}