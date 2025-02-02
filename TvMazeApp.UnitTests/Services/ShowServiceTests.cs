using Microsoft.Extensions.Logging;
using Moq;
using TvMazeApp.Application.Services;
using TvMazeApp.Domain.Dtos;
using TvMazeApp.Domain.Entities;
using TvMazeApp.Domain.Interfaces;
using Xunit;

namespace TvMazeApp.UnitTests.Services;

public class ShowServiceTests
{
    private readonly Mock<ITvMazeService> _tvMazeServiceMock;
    private readonly Mock<IShowRepository> _showRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<ILogger<ShowService>> _loggerMock;
    private readonly ShowService _showService;

    public ShowServiceTests()
    {
        _tvMazeServiceMock = new Mock<ITvMazeService>();
        _showRepositoryMock = new Mock<IShowRepository>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _loggerMock = new Mock<ILogger<ShowService>>();

        _showService = new ShowService(
            _tvMazeServiceMock.Object,
            _showRepositoryMock.Object,
            _genreRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task AddShowAsync_ShouldThrowException_WhenShowAlreadyExists()
    {
        // Arrange
        var showDto = new ShowDto { Id = 1, Name = "Test Show", Language = "English", Genres = new List<string>() };

        _showRepositoryMock.Setup(r => r.FindByIdNameAndLanguageAsync(showDto.Id, showDto.Name, showDto.Language))
            .ReturnsAsync(new Show { Id = 1, Name = "Test Show", Language = "English" });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _showService.AddShowAsync(showDto));
        Assert.Equal($"Show already exists: ID {showDto.Id}, Name '{showDto.Name}', Language '{showDto.Language}'", exception.Message);

        _showRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Show>()), Times.Never);
        _showRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task FetchAndStoreShowsAsync_ShouldSkipOldShows()
    {
        // Arrange
        var oldShow = new ShowDto { Id = 1, Name = "Old Show", Premiered = new DateTime(2013, 12, 31), Genres = new List<string>() };
        var newShow = new ShowDto { Id = 2, Name = "New Show", Premiered = new DateTime(2015, 1, 1), Genres = new List<string>() };

        _tvMazeServiceMock.Setup(s => s.FetchShowsAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<ShowDto> { oldShow, newShow });

        _showRepositoryMock.Setup(r => r.GetLastShowAsync())
            .ReturnsAsync((Show)null);

        _genreRepositoryMock.Setup(g => g.GetAll()).ReturnsAsync(new List<Genre>());

        // Act
        await _showService.FetchAndStoreShowsAsync();

        // Assert
        _showRepositoryMock.Verify(r => r.AddAsync(It.Is<Show>(s => s.Id == 2)), Times.Once);
        _showRepositoryMock.Verify(r => r.AddAsync(It.Is<Show>(s => s.Id == 1)), Times.Never);
        _showRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}