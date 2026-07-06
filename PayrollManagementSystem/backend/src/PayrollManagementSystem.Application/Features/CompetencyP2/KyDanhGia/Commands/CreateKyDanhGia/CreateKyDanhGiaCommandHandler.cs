using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.CreateKyDanhGia
{
    public class CreateKyDanhGiaCommandHandler : IRequestHandler<CreateKyDanhGiaCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public CreateKyDanhGiaCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<Guid>> Handle(CreateKyDanhGiaCommand request, CancellationToken cancellationToken)
        {
            var entity = new Domain.Models.KyDanhGia
            {
                TenKyDanhGia = request.TenKyDanhGia,
                Nam = request.NgayBatDau.Year,
                NgayBatDau = request.NgayBatDau,
                NgayKetThuc = request.NgayKetThuc,
                TrangThai = Domain.Enums.TrangThaiKyDanhGia.KHOI_TAO
            };
            _context.KyDanhGias.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return new Response<Guid>(entity.IdKyDanhGia, "Tạo kỳ đánh giá thành công.");
        }
    }
}
