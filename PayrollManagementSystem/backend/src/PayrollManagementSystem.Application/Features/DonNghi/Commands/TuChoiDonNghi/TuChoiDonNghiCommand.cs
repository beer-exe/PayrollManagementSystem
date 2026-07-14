using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.TuChoiDonNghi
{
    public class TuChoiDonNghiCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid Id { get; set; }
        public string CccdNguoiDuyet { get; set; } = null!;
        public string LyDoTuChoi { get; set; } = null!;
        public string CacheKeyPrefix => "DonNghi";
    }
}
