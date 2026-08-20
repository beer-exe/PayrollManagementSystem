using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Queries.GetKhungNangLucs
{
    public class GetKhungNangLucsQueryHandler : IRequestHandler<GetKhungNangLucsQuery, Response<IEnumerable<KhungNangLucDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetKhungNangLucsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<KhungNangLucDto>>> Handle(GetKhungNangLucsQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.KhungNangLucP2s
                .Where(x => x.IdChucVu == request.IdChucVu)
                .Select(x => new KhungNangLucDto
                {
                    IdTieuChi = x.IdTieuChi,
                    IdChucVu = x.IdChucVu,
                    TenNangLuc = x.TenNangLuc,
                    MoTa = x.MoTa,
                    TyTrong = x.TyTrong
                })
                .ToListAsync(cancellationToken);

            return new Response<IEnumerable<KhungNangLucDto>>(data);
        }
    }
}
