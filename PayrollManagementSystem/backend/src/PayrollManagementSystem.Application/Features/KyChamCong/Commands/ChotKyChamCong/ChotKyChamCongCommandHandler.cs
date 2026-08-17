using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.KyChamCong.Commands.ChotKyChamCong
{
    public class ChotKyChamCongCommandHandler : IRequestHandler<ChotKyChamCongCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public ChotKyChamCongCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(ChotKyChamCongCommand request, CancellationToken cancellationToken)
        {
            var kyChamCong = await _context.KyChamCongs
                .FirstOrDefaultAsync(x => x.Thang == request.Thang && x.Nam == request.Nam, cancellationToken);

            if (kyChamCong != null)
            {
                if (kyChamCong.TrangThai == TrangThaiKyChamCong.DA_CHOT)
                    throw new ApiException($"Kỳ chấm công tháng {request.Thang}/{request.Nam} đã được chốt trước đó.");

                kyChamCong.TrangThai = TrangThaiKyChamCong.DA_CHOT;
                _context.KyChamCongs.Update(kyChamCong);
            }
            else
            {
                kyChamCong = new Domain.Models.KyChamCong
                {
                    Id = Guid.NewGuid(),
                    Thang = request.Thang,
                    Nam = request.Nam,
                    TrangThai = TrangThaiKyChamCong.DA_CHOT
                };
                await _context.KyChamCongs.AddAsync(kyChamCong, cancellationToken);
            }

            // Update all ChamCong records for this month to point to this KyChamCong
            var chamCongs = await _context.ChamCongs
                .Where(x => x.NgayChamCong.Year == request.Nam && x.NgayChamCong.Month == request.Thang)
                .ToListAsync(cancellationToken);

            foreach (var cc in chamCongs)
            {
                cc.IdKyChamCong = kyChamCong.Id;
                cc.TrangThai = TrangThaiChamCong.DA_XAC_NHAN;
            }

            if (chamCongs.Any())
            {
                _context.ChamCongs.UpdateRange(chamCongs);
            }
            
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, $"Đã chốt công tháng {request.Thang}/{request.Nam} thành công.");
        }
    }
}
