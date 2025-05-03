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

        var dayScheduleThursday = new DaySchedule
        {
            Id = 3,
            Day = DayOfWeek.Thursday,
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
            DaySchedules = new List<DaySchedule> { dayScheduleMonday, dayScheduleTuesday, dayScheduleThursday },
            StartDate = startDate,
            BusySlots = new List<Slot> { busySlot }
        };

        var appointment = new Appointment
        {
            Id = 1,
            Slot = new Slot
            {
                Id = 1,
                Start = new DateTime(2025, 5, 8, 9, 0, 0),
                End = new DateTime(2025, 5, 8, 9, 30, 0),
                Day = DayOfWeek.Thursday,
                ScheduleStartDate = new DateOnly(2025, 5, 5)
            },
            Comments = "Initial consultation",
            Patient = new Patient
            {
                Id = 1,
                Name = "John",
                SecondName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1234567890"
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
            Id = 1,
            Slot = new Slot
            {
                Id = 1,
                Start = new DateTime(2025, 5, 1, 9, 0, 0),
                End = new DateTime(2025, 5, 1, 10, 0, 0),
                Day = DayOfWeek.Thursday,
                ScheduleStartDate = new DateOnly(2025, 5, 1)
            },
            Comments = "Initial consultation",
            Patient = new Patient
            {
                Id = 1,
                Name = "John",
                SecondName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1234567890"
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
            Id = 1,
            Slot = new Slot
            {
                Id = 1,
                Start = new DateTime(2025, 5, 1, 9, 0, 0),
                End = new DateTime(2025, 5, 1, 10, 0, 0),
                Day = DayOfWeek.Thursday,
                ScheduleStartDate = new DateOnly(2025, 5, 1)
            },
            Comments = "Initial consultation",
            Patient = new Patient
            {
                Id = 1,
                Name = "John",
                SecondName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1234567890"
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

    [Fact]
    public async Task TakeSlotAsync_DayScheduleNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 1,
            Slot = new Slot
            {
                Id = 1,
                Start = new DateTime(2025, 5, 7, 9, 0, 0),
                End = new DateTime(2025, 5, 7, 10, 0, 0),
                Day = DayOfWeek.Wednesday,
                ScheduleStartDate = new DateOnly(2025, 5, 1)
            },
            Comments = "Initial consultation",
            Patient = new Patient
            {
                Id = 1,
                Name = "John",
                SecondName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1234567890"
            }
        };
        _patientRepositoryMock.GetPatientIdByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(1UL);
        _appointmentRepositoryMock.GetScheduleAsync(Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(new Schedule
            {
                Facility = new Facility { Id = 1, Name = "Facility", Address = "Address" },
                SlotDurationMinutes = 30,
                DaySchedules = new List<DaySchedule>() // Empty list to simulate no day schedule found
            });

        // Act & Assert
        await _slotService.Invoking(s => s.TakeSlotAsync(appointment))
            .Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage("Day schedule for appointment day cannot be found.");
    }

    [Fact]
    public async Task TakeSlotAsync_SlotDurationMismatch_ThrowsScheduleException()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 1,
            Slot = new Slot
            {
                Id = 1,
                Start = new DateTime(2025, 5, 7, 9, 0, 0),
                End = new DateTime(2025, 5, 7, 9, 45, 0), // 45 minutes duration
                Day = DayOfWeek.Wednesday,
                ScheduleStartDate = new DateOnly(2025, 5, 5)
            },
            Comments = "Initial consultation",
            Patient = new Patient
            {
                Id = 1,
                Name = "John",
                SecondName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1234567890"
            }
        };
        _patientRepositoryMock.GetPatientIdByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(1UL);
        _appointmentRepositoryMock.GetScheduleAsync(Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(new Schedule
            {
                Facility = new Facility { Id = 1, Name = "Facility", Address = "Address" },
                SlotDurationMinutes = 30, // Expected 30 minutes duration
                DaySchedules = new List<DaySchedule>
                {
                    new()
                    {
                        Id = 1,
                        Day = DayOfWeek.Wednesday,
                        WorkPeriod = new WorkPeriod { StartHour = 9, EndHour = 17 }
                    }
                }
            });

        // Act & Assert
        await _slotService.Invoking(s => s.TakeSlotAsync(appointment))
            .Should().ThrowAsync<ScheduleException>()
            .WithMessage("Slot duration does not match schedule slot duration.");
    }

    [Fact]
    public async Task TakeSlotAsync_SlotOutOfWorkingHours_ThrowsScheduleException()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 1,
            Slot = new Slot
            {
                Id = 1,
                Start = new DateTime(2025, 5, 8, 18, 0, 0), // 18:00, outside work hours
                End = new DateTime(2025, 5, 8, 19, 0, 0),
                Day = DayOfWeek.Thursday,
                ScheduleStartDate = new DateOnly(2025, 5, 5)
            },
            Comments = "Initial consultation",
            Patient = new Patient
            {
                Id = 1,
                Name = "John",
                SecondName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1234567890"
            }
        };
        _patientRepositoryMock.GetPatientIdByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(1UL);
        _appointmentRepositoryMock.GetScheduleAsync(Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(new Schedule
            {
                Facility = new Facility { Id = 1, Name = "Facility", Address = "Address" },
                SlotDurationMinutes = 60,
                DaySchedules = new List<DaySchedule>
                {
                    new()
                    {
                        Id = 1,
                        Day = DayOfWeek.Thursday,
                        WorkPeriod = new WorkPeriod { StartHour = 9, EndHour = 17 } // Work hours: 9:00 to 17:00
                    }
                }
            });

        // Act & Assert
        await _slotService.Invoking(s => s.TakeSlotAsync(appointment))
            .Should().ThrowAsync<ScheduleException>()
            .WithMessage("Slot is out of working hours.");
    }

    [Fact]
    public async Task TakeSlotAsync_SlotDuringPlannedBreak_ThrowsScheduleException()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 1,
            Slot = new Slot
            {
                Id = 1,
                Start = new DateTime(2025, 5, 8, 12, 30, 0), // During lunch break
                End = new DateTime(2025, 5, 8, 13, 30, 0),
                Day = DayOfWeek.Thursday,
                ScheduleStartDate = new DateOnly(2025, 5, 5)
            },
            Comments = "Initial consultation",
            Patient = new Patient
            {
                Id = 1,
                Name = "John",
                SecondName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1234567890"
            }
        };
        _patientRepositoryMock.GetPatientIdByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(1UL);
        _appointmentRepositoryMock.GetScheduleAsync(Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(new Schedule
            {
                StartDate = new DateOnly(2025, 5, 5),
                Facility = new Facility { Id = 1, Name = "Facility", Address = "Address" },
                SlotDurationMinutes = 60,
                DaySchedules = new List<DaySchedule>
                {
                    new()
                    {
                        Id = 1,
                        Day = DayOfWeek.Thursday,
                        WorkPeriod = new WorkPeriod
                        {
                            StartHour = 9,
                            EndHour = 17,
                            LunchStartHour = 12,
                            LunchEndHour = 13 // Lunch break from 12:00 to 13:00
                        }
                    }
                }
            });

        // Act & Assert
        await _slotService.Invoking(s => s.TakeSlotAsync(appointment))
            .Should().ThrowAsync<ScheduleException>()
            .WithMessage("Slot is during planned break.");
    }
}