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

            // Check if shift is used in the past
            bool isUsedInPast = await _context.ChiTietLichLamViecs
                .AnyAsync(c => c.IdCaLamViecMacDinh == request.Id && c.Ngay < DateOnly.FromDateTime(DateTime.Now) && !c.IsDeleted, cancellationToken)
                || await _context.PhanCongCas
                .AnyAsync(p => p.IdCaLamViec == request.Id && p.NgayLamViec < DateOnly.FromDateTime(DateTime.Now) && !p.IsDeleted, cancellationToken);

            if (isUsedInPast)
            {
                bool isCoreModified = 
                    caLamViec.GioBatDau != TimeSpan.Parse(request.GioBatDau) ||
                    caLamViec.GioKetThuc != TimeSpan.Parse(request.GioKetThuc) ||
                    caLamViec.XuyenNgay != request.XuyenNgay ||
                    caLamViec.HeSoLuong != request.HeSoLuong;

                if (!isCoreModified)
                {
                    if (request.KhungGioNghis.Count != caLamViec.KhungGioNghis.Count(k => !k.IsDeleted))
                    {
                        isCoreModified = true;
                    }
                    else
                    {
                        foreach (var kgn in request.KhungGioNghis)
                        {
                            if (!kgn.Id.HasValue) 
                            {
                                isCoreModified = true;
                                break;
                            }
                            var existing = caLamViec.KhungGioNghis.FirstOrDefault(k => k.Id == kgn.Id.Value && !k.IsDeleted);
                            if (existing == null || 
                                existing.GioBatDau != TimeSpan.Parse(kgn.GioBatDau) ||
                                existing.GioKetThuc != TimeSpan.Parse(kgn.GioKetThuc) ||
                                existing.TinhVaoGioLam != kgn.TinhVaoGioLam)
                            {
                                isCoreModified = true;
                                break;
                            }
                        }
                    }
                }

                if (isCoreModified)
                {
                    //throw new ApiException("Ca làm việc này đã có dữ liệu chấm công nên không thể thay đổi giờ giấc hoặc hệ số lương. Vui lòng vô hiệu hoá ca này và tạo ca mới.");
                    throw new ApiException("Ca làm việc này đã có dữ liệu chấm công nên không thể thay đổi giờ giấc hoặc hệ số lương.");
                }
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
                    caLamViec.KhungGioNghis.Add(new KhungGioNghi
                    {
                        TenKhoangNghi = kgn.TenKhoangNghi,
                        GioBatDau = TimeSpan.Parse(kgn.GioBatDau),
                        GioKetThuc = TimeSpan.Parse(kgn.GioKetThuc),
                        TinhVaoGioLam = kgn.TinhVaoGioLam
                    });
                }
            }

            // Recalculate working hours for future schedules using this shift
            decimal newHours = caLamViec.CalculateWorkingHours();
            
            var futureDetails = await _context.ChiTietLichLamViecs
                .Where(c => c.IdCaLamViecMacDinh == caLamViec.Id 
                         && c.Ngay >= DateOnly.FromDateTime(DateTime.Now) 
                         && !c.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var detail in futureDetails)
            {
                if (detail.LoaiNgay == PayrollManagementSystem.Domain.Enums.LoaiNgay.NGAY_LAM_VIEC)
                {
                    detail.SoGioLam = newHours;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Cập nhật ca làm việc thành công.");
        }
    }
}
