using MediatR;
using Microsoft.Extensions.Logging;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SystemManagement.Commands.ClearCache
{
    public class ClearCacheCommandHandler : IRequestHandler<ClearCacheCommand, Response<bool>>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<ClearCacheCommandHandler> _logger;

        public ClearCacheCommandHandler(ICacheService cacheService, ILogger<ClearCacheCommandHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Response<bool>> Handle(ClearCacheCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _cacheService.ClearAllAsync(cancellationToken);
                return new Response<bool>(true, "Xóa toàn bộ cache thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi trong quá trình xóa cache hệ thống.");
                throw new ApiException("Đã xảy ra lỗi khi xóa cache. Vui lòng thử lại sau.");
            }
        }
    }
}
