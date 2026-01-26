using FluentAssertions;
using HaryanaStatAbstract.API.Controllers;
using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Models.Dtos;
using HaryanaStatAbstract.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HaryanaStatAbstract.API.Tests.Controllers
{
    public class CensusPopulationControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CensusPopulationController _controller;
        private readonly ICensusPopulationService _service;

        public CensusPopulationControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new CensusPopulationService(
                _context,
                new Microsoft.Extensions.Logging.LoggerFactory().CreateLogger<CensusPopulationService>());
            _controller = new CensusPopulationController(
                _service,
                new Microsoft.Extensions.Logging.LoggerFactory().CreateLogger<CensusPopulationController>());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfRecords()
        {
            // Arrange
            var record = new CreateCensusPopulationDto
            {
                Year = 2011,
                TotalPopulation = 25351462,
                MalePopulation = 13494734,
                FemalePopulation = 11856728,
                SexRatio = 879
            };
            await _controller.Create(record);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var records = okResult!.Value as IEnumerable<CensusPopulationDto>;
            records.Should().NotBeNull();
            records.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetByYear_ReturnsOkResult_WhenRecordExists()
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
            await _controller.Create(createDto);

            // Act
            var result = await _controller.GetByYear(2011);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var record = okResult!.Value as CensusPopulationDto;
            record.Should().NotBeNull();
            record!.Year.Should().Be(2011);
        }

        [Fact]
        public async Task GetByYear_ReturnsNotFound_WhenRecordDoesNotExist()
        {
            // Act
            var result = await _controller.GetByYear(2021);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Create_ReturnsCreatedResult_WithValidData()
        {
            // Arrange
            var createDto = new CreateCensusPopulationDto
            {
                Year = 2021,
                TotalPopulation = 30000000,
                MalePopulation = 15000000,
                FemalePopulation = 15000000,
                SexRatio = 1000
            };

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdAtResult = result.Result as CreatedAtActionResult;
            var record = createdAtResult!.Value as CensusPopulationDto;
            record.Should().NotBeNull();
            record!.Year.Should().Be(2021);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenTotalDoesNotEqualSum()
        {
            // Arrange
            var createDto = new CreateCensusPopulationDto
            {
                Year = 2021,
                TotalPopulation = 30000000,
                MalePopulation = 15000000,
                FemalePopulation = 14000000, // Doesn't equal total
                SexRatio = 933
            };

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WhenRecordExists()
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
            await _controller.Create(createDto);

            var updateDto = new UpdateCensusPopulationDto
            {
                TotalPopulation = 26000000,
                MalePopulation = 13000000,
                FemalePopulation = 13000000,
                SexRatio = 1000
            };

            // Act
            var result = await _controller.Update(2011, updateDto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var record = okResult!.Value as CensusPopulationDto;
            record.Should().NotBeNull();
            record!.TotalPopulation.Should().Be(26000000);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenRecordDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateCensusPopulationDto
            {
                TotalPopulation = 26000000
            };

            // Act
            var result = await _controller.Update(2021, updateDto);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenRecordExists()
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
            await _controller.Create(createDto);

            // Act
            var result = await _controller.Delete(2011);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenRecordDoesNotExist()
        {
            // Act
            var result = await _controller.Delete(2021);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetByRange_ReturnsOkResult_WithRecordsInRange()
        {
            // Arrange
            var record1 = new CreateCensusPopulationDto
            {
                Year = 2001,
                TotalPopulation = 21144564,
                MalePopulation = 11363953,
                FemalePopulation = 9780611,
                SexRatio = 861
            };
            var record2 = new CreateCensusPopulationDto
            {
                Year = 2011,
                TotalPopulation = 25351462,
                MalePopulation = 13494734,
                FemalePopulation = 11856728,
                SexRatio = 879
            };
            await _controller.Create(record1);
            await _controller.Create(record2);

            // Act
            var result = await _controller.GetByRange(2000, 2010);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var records = okResult!.Value as IEnumerable<CensusPopulationDto>;
            records.Should().NotBeNull();
            records.Should().HaveCount(1); // Only 2001 is in range
            records!.First().Year.Should().Be(2001);
        }
    }
}