using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Response<string>>
    {
        private readonly IApplicationDbContext _context;

        public CreateDepartmentCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<string>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            if (await _context.PhongBans.AnyAsync(pb => pb.IdPb == request.IdPb, cancellationToken))
                throw new ApiException($"Phòng ban với mã '{request.IdPb}' đã tồn tại.");

            var phongBan = new PhongBan
            {
                IdPb = request.IdPb,
                TenPb = request.TenPb
            };

            _context.PhongBans.Add(phongBan);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<string>(phongBan.IdPb, "Tạo phòng ban thành công.");
        }
    }
}