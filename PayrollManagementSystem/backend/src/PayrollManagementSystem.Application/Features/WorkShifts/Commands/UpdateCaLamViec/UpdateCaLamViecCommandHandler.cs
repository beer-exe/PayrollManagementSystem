using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.WorkShifts.Commands.UpdateCaLamViec
{
    public class UpdateCaLamViecCommandHandler : IRequestHandler<UpdateCaLamViecCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCaLamViecCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(UpdateCaLamViecCommand request, CancellationToken cancellationToken)
        {
            var caLamViec = await _context.CaLamViecs
                .Include(c => c.KhungGioNghis)
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

            if (caLamViec == null)
            {
                throw new ApiException("Không tìm thấy ca làm việc.");
            }

            caLamViec.TenCa = request.TenCa;
            caLamViec.GioBatDau = TimeSpan.Parse(request.GioBatDau);
            caLamViec.GioKetThuc = TimeSpan.Parse(request.GioKetThuc);
            caLamViec.XuyenNgay = request.XuyenNgay;
            caLamViec.HeSoLuong = request.HeSoLuong;
            caLamViec.TrangThai = request.TrangThai;

            var newBreakIds = request.KhungGioNghis.Where(k => k.Id.HasValue).Select(k => k.Id.Value).ToList();
            var breaksToRemove = caLamViec.KhungGioNghis.Where(k => !newBreakIds.Contains(k.Id)).ToList();
            
            foreach (var b in breaksToRemove)
            {
                b.IsDeleted = true;
            }

            foreach (var kgn in request.KhungGioNghis)
            {
                if (kgn.Id.HasValue)
                {
                    var existingBreak = caLamViec.KhungGioNghis.FirstOrDefault(k => k.Id == kgn.Id.Value);
                    if (existingBreak != null)
                    {
                        existingBreak.TenKhoangNghi = kgn.TenKhoangNghi;
                        existingBreak.GioBatDau = TimeSpan.Parse(kgn.GioBatDau);
                        existingBreak.GioKetThuc = TimeSpan.Parse(kgn.GioKetThuc);
                        existingBreak.TinhVaoGioLam = kgn.TinhVaoGioLam;
                    }
                }
                else
                {
                    _context.KhungGioNghis.Add(new KhungGioNghi
                    {
                        IdCaLamViec = caLamViec.Id,
                        TenKhoangNghi = kgn.TenKhoangNghi,
                        GioBatDau = TimeSpan.Parse(kgn.GioBatDau),
                        GioKetThuc = TimeSpan.Parse(kgn.GioKetThuc),
                        TinhVaoGioLam = kgn.TinhVaoGioLam
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Cập nhật ca làm việc thành công.");
        }
    }
}
