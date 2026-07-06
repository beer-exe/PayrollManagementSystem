using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommand : IRequest<Response<bool>>
    {
        public string Cccd { get; set; } = null!;

        public string HoTen { get; set; } = null!;
        public bool? GioiTinh { get; set; }
        public string? Sdt { get; set; }
        public string? Email { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string? DanToc { get; set; }
        public string? DiaChi { get; set; }
        public string? ChuyenNganh { get; set; }
        public string? SoBhxh { get; set; }
        public string? SoBhyt { get; set; }
        public string? IdPb { get; set; }

        public string? SoTaiKhoan { get; set; }
        public string? TenNganHang { get; set; }
        public string? MaSoThue { get; set; }

        public List<UpdateThanNhanDto>? ThanNhans { get; set; }
    }

    public class UpdateThanNhanDto
    {
        public string? MaDinhDanh { get; set; } // Nếu có mã định danh thì là cập nhật, nếu null/rỗng thì là thêm mới
        public string TenTn { get; set; } = null!;
        public DateOnly? NgaySinh { get; set; }
        public Guid? IdMqh { get; set; } // Quan hệ
    }
}