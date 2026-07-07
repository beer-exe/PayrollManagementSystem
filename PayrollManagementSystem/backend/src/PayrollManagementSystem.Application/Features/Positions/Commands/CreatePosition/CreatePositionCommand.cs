using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Positions.Commands.CreatePosition
{
    public class CreatePositionCommand : IRequest<Response<string>>
    {
        public string IdChucVu { get; set; } = null!;
        public string TenChucVu { get; set; } = null!;
        public string? MoTaCongViec { get; set; }
        public string? IdNgachLuong { get; set; }
        public string IdPhongBan { get; set; } = null!;
        public string? IdChucVuQuanLy { get; set; }
    }
}
