using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Users.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Users.Queries.GetRoles
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Response<IEnumerable<RoleDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetRolesQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _context.VaiTros
                .Select(v => new RoleDto { IdVaiTro = v.IdVaiTro, TenVaiTro = v.TenVaiTro })
                .ToListAsync(cancellationToken);

            return new Response<IEnumerable<RoleDto>>(roles, "Lấy danh sách vai trò thành công.");
        }
    }
}