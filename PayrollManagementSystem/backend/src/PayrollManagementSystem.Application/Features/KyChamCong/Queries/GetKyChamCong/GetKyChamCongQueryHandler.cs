using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.KyChamCong.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.KyChamCong.Queries.GetKyChamCong
{
    public class GetKyChamCongQueryHandler : IRequestHandler<GetKyChamCongQuery, Response<KyChamCongDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetKyChamCongQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<KyChamCongDto>> Handle(GetKyChamCongQuery request, CancellationToken cancellationToken)
        {
            var kyChamCong = await _context.KyChamCongs
                .FirstOrDefaultAsync(x => x.Thang == request.Thang && x.Nam == request.Nam, cancellationToken);

            if (kyChamCong == null)
            {
                return new Response<KyChamCongDto>(new KyChamCongDto
                {
                    Id = Guid.Empty,
                    Thang = request.Thang,
                    Nam = request.Nam,
                    TrangThai = TrangThaiKyChamCong.DANG_MO.ToString(),
                    TrangThaiText = TrangThaiKyChamCong.DANG_MO.GetDescription()
                });
            }

            return new Response<KyChamCongDto>(new KyChamCongDto
            {
                Id = kyChamCong.Id,
                Thang = kyChamCong.Thang,
                Nam = kyChamCong.Nam,
                TrangThai = kyChamCong.TrangThai.ToString(),
                TrangThaiText = kyChamCong.TrangThai.GetDescription()
            });
        }
    }
}
