using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.TransferEmployee
{
    public class TransferEmployeeCommand : IRequest<Response<bool>>
    {
        public string Cccd { get; set; } = null!;
        public string IdPbMoi { get; set; } = null!;
        public string IdChucVuMoi { get; set; } = null!;
        public string SoQuyetDinh { get; set; } = null!;
        public DateOnly NgayHieuLuc { get; set; }
        public string? NguoiKy { get; set; }
    }
}
