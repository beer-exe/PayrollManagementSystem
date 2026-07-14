using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.ChangePosition
{
    public class ChangePositionCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string SoQuyetDinh { get; set; } = null!;
        public string Cccd { get; set; } = null!;
        public string IdChucVuMoi { get; set; } = null!;
        public string IdBacLuongMoi { get; set; } = null!;
        public DateTime NgayHieuLuc { get; set; }
        public string? LyDo { get; set; }

        public string CacheKeyPrefix => "Departments_";
    }
}
