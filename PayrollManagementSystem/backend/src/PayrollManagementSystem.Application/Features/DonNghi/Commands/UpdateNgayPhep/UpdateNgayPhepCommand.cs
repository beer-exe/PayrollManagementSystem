using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.UpdateNgayPhep
{
    public class UpdateNgayPhepCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public int Nam { get; set; }
        public decimal TongNgayPhep { get; set; } = 12;
        public string CacheKeyPrefix => CacheKeyConstants.NgayPhep;
    }
}
