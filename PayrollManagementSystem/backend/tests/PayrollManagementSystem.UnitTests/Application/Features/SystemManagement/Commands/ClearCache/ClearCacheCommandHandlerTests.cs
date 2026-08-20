using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.SystemManagement.Commands.ClearCache;

namespace PayrollManagementSystem.UnitTests.Application.Features.SystemManagement.Commands.ClearCache
{
    public class ClearCacheCommandHandlerTests
    {
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<ILogger<ClearCacheCommandHandler>> _loggerMock;
        private readonly ClearCacheCommandHandler _handler;

        public ClearCacheCommandHandlerTests()
        {
            _cacheServiceMock = new Mock<ICacheService>();
            _loggerMock = new Mock<ILogger<ClearCacheCommandHandler>>();
            _handler = new ClearCacheCommandHandler(_cacheServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ClearsCacheAndReturnsSuccess()
        {
            var command = new ClearCacheCommand();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.Message.Should().Contain("Xóa toàn bộ cache thành công");
            _cacheServiceMock.Verify(x => x.ClearAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ServiceThrowsException_ThrowsApiException()
        {
            _cacheServiceMock.Setup(x => x.ClearAllAsync(It.IsAny<CancellationToken>()))
                             .ThrowsAsync(new Exception("Redis connection failed"));

            var command = new ClearCacheCommand();

            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Contain("Đã xảy ra lỗi khi xóa cache");

            // Check if logger was called
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }
    }
}
