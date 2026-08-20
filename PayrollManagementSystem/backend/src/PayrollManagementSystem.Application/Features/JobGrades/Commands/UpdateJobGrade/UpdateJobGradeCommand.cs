using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.JobGrades.Commands.UpdateJobGrade
{
    public class UpdateJobGradeCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string IdNgachLuong { get; set; } = null!;
        public string TenNgachLuong { get; set; } = null!;
        public string? MoTa { get; set; }
        public int TrangThai { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.JobGrades;
    }
}
