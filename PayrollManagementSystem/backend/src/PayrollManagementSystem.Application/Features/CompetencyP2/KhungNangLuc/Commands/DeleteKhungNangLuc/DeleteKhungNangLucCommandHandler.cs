using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.DeleteKhungNangLuc
{
    public class DeleteKhungNangLucCommandHandler : IRequestHandler<DeleteKhungNangLucCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteKhungNangLucCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(DeleteKhungNangLucCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.KhungNangLucP2s.FindAsync(new object[] { request.IdTieuChi }, cancellationToken);
            if (entity == null) return new Response<bool>("Không tìm thấy tiêu chí.");

            _context.KhungNangLucP2s.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Xóa tiêu chí thành công.");
        }
    }
}
