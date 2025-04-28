using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application;
using Core.Domain;
using Core.Persistence.Contract;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Application.UnitTest;

public class SlotServiceUnitTest
{
    [Fact]
    public async Task GetAvailabilityAsync_ShouldReturnSchedule_WhenDateIsValid()
    {
        // Arrange
        var mockRepository = Substitute.For<IAppointmentRepository>();
        var expectedSchedule = new Schedule { Id = 1, StartDate = new DateOnly(2025, 4, 28) };
        mockRepository.GetScheduleAsync(Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Schedule?>(expectedSchedule));

        var slotService = new SlotService(mockRepository);

        // Act
        var result = await slotService.GetAvailabilityAsync("20250428");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedSchedule);
    }
}