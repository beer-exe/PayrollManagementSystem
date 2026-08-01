using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.UpsertCauHinhGiamTru
{
    public class UpsertCauHinhGiamTruCommandHandler : IRequestHandler<UpsertCauHinhGiamTruCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public UpsertCauHinhGiamTruCommandHandler(IApplicationDbContext context) { _context = context; }

        public async Task<Response<bool>> Handle(UpsertCauHinhGiamTruCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.CauHinhGiamTrus
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                entity = new CauHinhGiamTru
                {
                    GiamTruBanThan = request.GiamTruBanThan,
                    GiamTruNguoiPhuThuoc = request.GiamTruNguoiPhuThuoc,
                    GhiChu = request.GhiChu,
                    IsActive = true
                };
                _context.CauHinhGiamTrus.Add(entity);
            }
            else
            {
                entity.GiamTruBanThan = request.GiamTruBanThan;
                entity.GiamTruNguoiPhuThuoc = request.GiamTruNguoiPhuThuoc;
                entity.GhiChu = request.GhiChu;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Cập nhật cấu hình giảm trừ thành công.");
        }
    }
}
