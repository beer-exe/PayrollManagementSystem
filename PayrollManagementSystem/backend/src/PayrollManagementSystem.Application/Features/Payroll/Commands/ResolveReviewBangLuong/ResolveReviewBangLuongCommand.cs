using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Constants;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.ResolveReviewBangLuong
{
    public class ResolveReviewBangLuongCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        // không kế thừa ITransactionalCommand  ở class này vì handler gọi CalculatePayrollCommand
        public Guid IdBangLuong { get; set; }
        public string Action { get; set; } = null!; // "REJECT" hoặc "RECALCULATE"
        public string? PhanHoiKhieuNai { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.Payroll;
    }
}
