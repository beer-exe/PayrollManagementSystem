using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.CreateKhungNangLuc
{
    public class CreateKhungNangLucCommandHandler : IRequestHandler<CreateKhungNangLucCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public CreateKhungNangLucCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<Guid>> Handle(CreateKhungNangLucCommand request, CancellationToken cancellationToken)
        {
            var entity = new KhungNangLucP2
            {
                IdChucVu = request.IdChucVu,
                TenNangLuc = request.TenNangLuc,
                YeuCauToiThieu = request.YeuCauToiThieu,
                TyTrong = request.TyTrong
            };

            _context.KhungNangLucP2s.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(entity.IdTieuChi, "Thêm tiêu chí thành công.");
        }
    }
}
