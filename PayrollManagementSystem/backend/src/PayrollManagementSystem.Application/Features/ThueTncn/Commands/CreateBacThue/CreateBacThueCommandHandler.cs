using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.CreateBacThue
{
    public class CreateBacThueCommandHandler : IRequestHandler<CreateBacThueCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public CreateBacThueCommandHandler(IApplicationDbContext context) { _context = context; }

        public async Task<Response<Guid>> Handle(CreateBacThueCommand request, CancellationToken cancellationToken)
        {
            var exists = await _context.BacThues.AnyAsync(x => x.Bac == request.Bac, cancellationToken);
            if (exists)
                throw new ApiException($"Bậc thuế số {request.Bac} đã tồn tại.");

            var bacThue = new BacThue
            {
                Bac = request.Bac,
                TuGia = request.TuGia,
                DenGia = request.DenGia,
                ThueSuat = request.ThueSuat,
                IsActive = true
            };

            _context.BacThues.Add(bacThue);
            await _context.SaveChangesAsync(cancellationToken);
            return new Response<Guid>(bacThue.IdBacThue, "Thêm bậc thuế thành công.");
        }
    }
}
