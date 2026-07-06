using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.UpdateKhungNangLuc
{
    public class UpdateKhungNangLucCommandHandler : IRequestHandler<UpdateKhungNangLucCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public UpdateKhungNangLucCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(UpdateKhungNangLucCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.KhungNangLucP2s.FindAsync(new object[] { request.IdTieuChi }, cancellationToken);
            if (entity == null) return new Response<bool>("Không tìm thấy tiêu chí.");

            entity.TenNangLuc = request.TenNangLuc;
            entity.YeuCauToiThieu = request.YeuCauToiThieu;
            entity.TyTrong = request.TyTrong;

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Cập nhật thành công.");
        }
    }
}
