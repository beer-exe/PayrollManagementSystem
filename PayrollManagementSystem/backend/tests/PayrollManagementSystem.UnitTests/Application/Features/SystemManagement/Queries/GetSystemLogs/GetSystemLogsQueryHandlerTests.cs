using FluentAssertions;
using Moq;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.SystemManagement.DTOs;
using PayrollManagementSystem.Application.Features.SystemManagement.Queries.GetSystemLogs;
using PayrollManagementSystem.Application.Wrappers;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.SystemManagement.Queries.GetSystemLogs
{
    public class GetSystemLogsQueryHandlerTests
    {
        private readonly Mock<ISystemLogRepository> _repositoryMock;
        private readonly GetSystemLogsQueryHandler _handler;

        public GetSystemLogsQueryHandlerTests()
        {
            _repositoryMock = new Mock<ISystemLogRepository>();
            _handler = new GetSystemLogsQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsLogsFromRepository()
        {
            var command = new GetSystemLogsQuery
            {
                Level = "Error",
                Keyword = "test",
                PageNumber = 1,
                PageSize = 10
            };

            var expectedLogs = new List<SystemLogDto>
            {
                new SystemLogDto { Id = 1, Message = "Test Error 1", Level = "Error" },
                new SystemLogDto { Id = 2, Message = "Test Error 2", Level = "Error" }
            };

            var pagedResponse = new PagedResponse<List<SystemLogDto>>(expectedLogs, 1, 10, 2);

            _repositoryMock.Setup(x => x.GetLogsAsync(
                command.Level,
                command.FromDate,
                command.ToDate,
                command.Keyword,
                command.SortBy,
                command.SortDirection,
                command.PageNumber,
                command.PageSize,
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(pagedResponse);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.TotalRecords.Should().Be(2);

            _repositoryMock.Verify(x => x.GetLogsAsync(
                "Error",
                null,
                null,
                "test",
                It.IsAny<string>(),
                It.IsAny<string>(),
                1,
                10,
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }
    }
}
