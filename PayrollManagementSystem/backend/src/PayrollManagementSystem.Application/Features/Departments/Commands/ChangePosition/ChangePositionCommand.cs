using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.ChangePosition
{
    public class ChangePositionCommand : IRequest<Response<bool>>
    {
        public string SoQuyetDinh { get; set; } = null!;
        public string Cccd { get; set; } = null!;
        public string IdChucVuMoi { get; set; } = null!;
        public string IdBacLuongMoi { get; set; } = null!;
        public DateTime NgayHieuLuc { get; set; }
        public string? LyDo { get; set; }
    }
}
