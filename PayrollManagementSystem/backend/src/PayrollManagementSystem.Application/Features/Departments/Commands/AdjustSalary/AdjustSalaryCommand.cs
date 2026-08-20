using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.AdjustSalary
{
    public class AdjustSalaryCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string SoQuyetDinh { get; set; } = null!;
        public string Cccd { get; set; } = null!;
        public string IdBacLuongMoi { get; set; } = null!;
        public DateTime NgayHieuLuc { get; set; }
        public string? LyDo { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.Departments;
    }
}
