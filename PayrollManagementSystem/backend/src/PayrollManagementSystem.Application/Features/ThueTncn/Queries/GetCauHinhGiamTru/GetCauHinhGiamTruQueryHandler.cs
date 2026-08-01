using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ThueTncn.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetCauHinhGiamTru
{
    public class GetCauHinhGiamTruQueryHandler : IRequestHandler<GetCauHinhGiamTruQuery, Response<CauHinhGiamTruDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetCauHinhGiamTruQueryHandler(IApplicationDbContext context) { _context = context; }

        public async Task<Response<CauHinhGiamTruDto>> Handle(GetCauHinhGiamTruQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.CauHinhGiamTrus
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            var dto = entity != null
                ? new CauHinhGiamTruDto
                {
                    IdCauHinhGiamTru = entity.IdCauHinhGiamTru,
                    GiamTruBanThan = entity.GiamTruBanThan,
                    GiamTruNguoiPhuThuoc = entity.GiamTruNguoiPhuThuoc,
                    GhiChu = entity.GhiChu
                }
                : new CauHinhGiamTruDto
                {
                    GiamTruBanThan = 11_000_000m,
                    GiamTruNguoiPhuThuoc = 4_400_000m
                };

            return new Response<CauHinhGiamTruDto>(dto, "Lấy cấu hình giảm trừ thành công.");
        }
    }
}
