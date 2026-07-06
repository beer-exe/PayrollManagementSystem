using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Departments.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Departments.Queries.GetEmployeesByDepartment
{
    public class GetEmployeesByDepartmentQueryHandler : IRequestHandler<GetEmployeesByDepartmentQuery, Response<IEnumerable<EmployeeInDepartmentDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetEmployeesByDepartmentQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<EmployeeInDepartmentDto>>> Handle(GetEmployeesByDepartmentQuery request, CancellationToken cancellationToken)
        {
            var employees = await _context.NhanViens
                .Where(nv => nv.IdPb == request.IdPb)
                .Select(nv => new EmployeeInDepartmentDto
                {
                    Cccd = nv.Cccd,
                    HoTen = nv.HoTen,
                    Email = nv.Email,
                    TrangThai = nv.TrangThai.ToString(),
                    NgayVaoLam = nv.NgayVaoLam,

                    TenChucVu = _context.QuyetDinhNhanSus
                        .Where(qd => qd.Cccd == nv.Cccd && qd.TrangThai == TrangThaiQuyetDinh.HIEU_LUC)
                        .OrderByDescending(qd => qd.NgayHieuLuc)
                        .Select(qd => _context.ChucVus.FirstOrDefault(cv => cv.IdChucVu == qd.IdChucVuMoi).TenChucVu)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            return new Response<IEnumerable<EmployeeInDepartmentDto>>(employees, "Lấy danh sách nhân viên theo phòng ban thành công.");
        }
    }
}
