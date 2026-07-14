using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.DuyetDonNghi
{
    public class DuyetDonNghiCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid Id { get; set; }
        public string CccdNguoiDuyet { get; set; } = null!;
        public string CacheKeyPrefix => "DonNghi,NgayPhep"; // Clear cache for both DonNghi and NgayPhep
    }
}
