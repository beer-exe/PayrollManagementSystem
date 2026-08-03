using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ChamCong.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.ImportChamCong
{
    public class ImportChamCongCommand : IRequest<Response<ImportChamCongResultDto>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Stream FileStream { get; set; } = null!;
        public string FileName { get; set; } = null!;

        public string CacheKeyPrefix => CacheKeyConstants.ChamCong;
    }
}
