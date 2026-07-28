using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.TransferEmployee
{
    public class TransferEmployeeCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string Cccd { get; set; } = null!;
        public string IdPbMoi { get; set; } = null!;
        public string IdChucVuMoi { get; set; } = null!;
        public string SoQuyetDinh { get; set; } = null!;
        public DateOnly NgayHieuLuc { get; set; }
        public string? NguoiKy { get; set; }
        public string IdBacLuongMoi { get; set; } = null!;
        public string? LoaiQuyetDinh { get; set; }

        public string CacheKeyPrefix => "Departments_";
    }
}
