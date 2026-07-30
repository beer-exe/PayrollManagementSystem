using MediatR;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommand : IRequest<Response<string>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string CacheKeyPrefix => "Departments_";

        public string Cccd { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public bool? GioiTinh { get; set; }
        public string? Sdt { get; set; }
        public string? Email { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string? DiaChi { get; set; }
        public string? DanToc { get; set; }
        public string? ChuyenNganh { get; set; }
        
        public string? SoBhxh { get; set; }
        public string? SoBhyt { get; set; }
        public string? SoTaiKhoan { get; set; }
        public string? TenNganHang { get; set; }
        public string? MaSoThue { get; set; }

        public string IdPb { get; set; } = null!;

        public string SoHopDong { get; set; } = null!;
        public string LoaiHopDong { get; set; } = null!;
        public DateOnly NgayBatDauHopDong { get; set; }
        public DateOnly? NgayKetThucHopDong { get; set; }
        public decimal LuongCoBan { get; set; }

        public string SoQuyetDinh { get; set; } = null!;
        public string IdChucVu { get; set; } = null!;
        public string? IdBacLuong { get; set; }
        public string? NguoiKyQuyetDinh { get; set; }
    }
}