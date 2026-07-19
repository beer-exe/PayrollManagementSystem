using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.WorkShifts.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.WorkShifts.Commands.CreateCaLamViec
{
    public class CreateCaLamViecCommand : IRequest<Response<Guid>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string TenCa { get; set; } = null!;
        public string GioBatDau { get; set; } = null!;
        public string GioKetThuc { get; set; } = null!;
        public bool XuyenNgay { get; set; }
        public decimal HeSoLuong { get; set; } = 1.0m;
        public bool TrangThai { get; set; } = true;

        public List<CreateKhungGioNghiCommand> KhungGioNghis { get; set; } = new();

        public string CacheKeyPrefix => "CaLamViec_";
    }

    public class CreateKhungGioNghiCommand
    {
        public string TenKhoangNghi { get; set; } = null!;
        public string GioBatDau { get; set; } = null!;
        public string GioKetThuc { get; set; } = null!;
        public bool TinhVaoGioLam { get; set; }
    }
}
