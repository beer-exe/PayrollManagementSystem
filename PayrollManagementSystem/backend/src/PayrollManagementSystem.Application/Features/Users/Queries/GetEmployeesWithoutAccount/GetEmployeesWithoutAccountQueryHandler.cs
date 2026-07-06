using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Users.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Users.Queries.GetEmployeesWithoutAccount
{
    public class GetEmployeesWithoutAccountQueryHandler : IRequestHandler<GetEmployeesWithoutAccountQuery, Response<IEnumerable<EmployeeNoAccountDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetEmployeesWithoutAccountQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<IEnumerable<EmployeeNoAccountDto>>> Handle(GetEmployeesWithoutAccountQuery request, CancellationToken cancellationToken)
        {
            var employees = await _context.NhanViens
                            .Where(nv => nv.IdTaiKhoan == null)
                            .OrderByDescending(nv => nv.NgayVaoLam)
                            .Select(nv => new EmployeeNoAccountDto
                            {
                                Cccd = nv.Cccd,
                                HoTen = nv.HoTen,
                                Email = nv.Email,
                                TenPhongBan = nv.PhongBan != null ? nv.PhongBan.TenPb : "Chưa có phòng ban."
                            })
                            .ToListAsync(cancellationToken);

            return new Response<IEnumerable<EmployeeNoAccountDto>>(employees, "Lấy danh sách nhân viên thành công.");
        }
    }
}
