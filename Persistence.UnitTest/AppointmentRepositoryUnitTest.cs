using System.Threading.Tasks;
using AutoMapper;
using Core.Persistence.Contract;
using EntityFrameworkCore.Testing.Moq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Persistence.Repository;
using Service;
using Xunit;

namespace Persistence.UnitTest;

public class AppointmentRepositoryUnitTest
{
    private AppointmentContext _context;
    private IAppointmentRepository _repository;

    public AppointmentRepositoryUnitTest()
    {
        var mapperConfig = new MapperConfigurationExpression();
        mapperConfig.AddProfile(new MappingProfile());

        _context = Create.MockedDbContextFor<AppointmentContext>();
        _repository = new AppointmentRepository(_context);

        TestDataFeed.Create(_context);
    }

    [Fact]
    public async Task GetBookAsync_ProperIdGiven_BookFound()
    {
        // Arrange
        var id = 1;

        // Act
        var result = await _repository.GetBookAsync(id).ConfigureAwait(false);

        // Assert
        result.Should().NotBeNull();
        result?.Id.Should().Be(id);
    }

    [TestMethod]
    public async Task GetBookAsync_UnrecognizedIdGiven_BookNotFound()
    {
        // Arrange
        var id = 5;

        // Act
        var result = await _repository.GetBookAsync(id).ConfigureAwait(false);

        // Assert
        result.Should().BeNull();
    }
}

