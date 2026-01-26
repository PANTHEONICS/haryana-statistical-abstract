using FluentAssertions;
using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Models.Dtos;
using HaryanaStatAbstract.API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HaryanaStatAbstract.API.Tests.Services
{
    public class CensusPopulationServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ICensusPopulationService _service;

        public CensusPopulationServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new CensusPopulationService(
                _context,
                new Microsoft.Extensions.Logging.LoggerFactory().CreateLogger<CensusPopulationService>());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateRecord_WhenDataIsValid()
        {
            // Arrange
            var createDto = new CreateCensusPopulationDto
            {
                Year = 2011,
                TotalPopulation = 25351462,
                MalePopulation = 13494734,
                FemalePopulation = 11856728,
                SexRatio = 879
            };

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Year.Should().Be(2011);
            result.TotalPopulation.Should().Be(25351462);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenTotalDoesNotEqualSum()
        {
            // Arrange
            var createDto = new CreateCensusPopulationDto
            {
                Year = 2011,
                TotalPopulation = 25351462,
                MalePopulation = 13494734,
                FemalePopulation = 11850000, // Doesn't equal total
                SexRatio = 879
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
        }

        [Fact]
        public async Task GetByYearAsync_ShouldReturnRecord_WhenExists()
        {
            // Arrange
            var createDto = new CreateCensusPopulationDto
            {
                Year = 2011,
                TotalPopulation = 25351462,
                MalePopulation = 13494734,
                FemalePopulation = 11856728,
                SexRatio = 879
            };
            await _service.CreateAsync(createDto);

            // Act
            var result = await _service.GetByYearAsync(2011);

            // Assert
            result.Should().NotBeNull();
            result!.Year.Should().Be(2011);
        }

        [Fact]
        public async Task GetByYearAsync_ShouldReturnNull_WhenDoesNotExist()
        {
            // Act
            var result = await _service.GetByYearAsync(2021);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateRecord_WhenExists()
        {
            // Arrange
            var createDto = new CreateCensusPopulationDto
            {
                Year = 2011,
                TotalPopulation = 25351462,
                MalePopulation = 13494734,
                FemalePopulation = 11856728,
                SexRatio = 879
            };
            await _service.CreateAsync(createDto);

            var updateDto = new UpdateCensusPopulationDto
            {
                TotalPopulation = 26000000,
                MalePopulation = 13000000,
                FemalePopulation = 13000000,
                SexRatio = 1000
            };

            // Act
            var result = await _service.UpdateAsync(2011, updateDto);

            // Assert
            result.Should().NotBeNull();
            result!.TotalPopulation.Should().Be(26000000);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenRecordExists()
        {
            // Arrange
            var createDto = new CreateCensusPopulationDto
            {
                Year = 2011,
                TotalPopulation = 25351462,
                MalePopulation = 13494734,
                FemalePopulation = 11856728,
                SexRatio = 879
            };
            await _service.CreateAsync(createDto);

            // Act
            var result = await _service.DeleteAsync(2011);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenRecordDoesNotExist()
        {
            // Act
            var result = await _service.DeleteAsync(2021);

            // Assert
            result.Should().BeFalse();
        }
    }
}