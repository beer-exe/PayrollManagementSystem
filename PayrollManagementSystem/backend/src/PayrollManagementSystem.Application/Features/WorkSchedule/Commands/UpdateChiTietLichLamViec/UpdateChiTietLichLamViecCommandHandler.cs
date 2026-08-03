using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Commands.UpdateChiTietLichLamViec
{
    public class UpdateChiTietLichLamViecCommandHandler : IRequestHandler<UpdateChiTietLichLamViecCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateChiTietLichLamViecCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(UpdateChiTietLichLamViecCommand request, CancellationToken cancellationToken)
        {
            var chiTiet = await _context.ChiTietLichLamViecs
                .Include(c => c.LichLamViec)
                .FirstOrDefaultAsync(c => c.Id == request.IdChiTiet && !c.IsDeleted, cancellationToken);

            if (chiTiet == null)
            {
                throw new ApiException("Không tìm thấy chi tiết lịch làm việc.");
            }

            if (chiTiet.LichLamViec.TrangThai == TrangThaiLichLamViec.HET_HIEU_LUC)
            {
                throw new ApiException("Không thể cập nhật ngày thuộc về Lịch làm việc đã hết hiệu lực.");
            }

            if (chiTiet.Ngay < DateOnly.FromDateTime(DateTime.Now))
            {
                throw new ApiException("Không thể cập nhật thông tin của những ngày trong quá khứ.");
            }

            if (!Domain.Extensions.EnumExtensions.TryGetValueFromDescription<LoaiNgay>(request.LoaiNgay, out var loaiNgay))
            {
                throw new ApiException("Loại ngày không hợp lệ.");
            }

            if (loaiNgay == LoaiNgay.NGHI_CUOI_TUAN || loaiNgay == LoaiNgay.NGHI_LE)
            {
                var hasConflictingLeaves = await _context.DonNghis
                    .AnyAsync(d => d.NgayBatDau <= chiTiet.Ngay && d.NgayKetThuc >= chiTiet.Ngay
                                && (d.TrangThai == TrangThaiDonNghi.CHO_DUYET || d.TrangThai == TrangThaiDonNghi.DA_DUYET)
                                && !d.IsDeleted, cancellationToken);
                
                if (hasConflictingLeaves)
                {
                    throw new ApiException("Có đơn xin nghỉ phép của nhân viên trong ngày này. Vui lòng huỷ các đơn nghỉ phép liên quan trước khi đổi thành ngày nghỉ.");
                }
            }

            chiTiet.LoaiNgay = loaiNgay;
            
            if (loaiNgay == LoaiNgay.NGHI_CUOI_TUAN && string.IsNullOrWhiteSpace(request.TenNgayNghi))
            {
                chiTiet.TenNgayNghi = $"Nghỉ {chiTiet.Thu}";
            }
            else
            {
                chiTiet.TenNgayNghi = string.IsNullOrWhiteSpace(request.TenNgayNghi) ? null : request.TenNgayNghi.Trim();
            }
            
            // Adjust working hours based on day type
            if (loaiNgay == LoaiNgay.NGAY_LAM_VIEC)
            {
                decimal workingHours = 8;
                
                if (request.IdCaLamViecMacDinh.HasValue)
                {
                    var shift = await _context.CaLamViecs
                        .Include(c => c.KhungGioNghis)
                        .FirstOrDefaultAsync(c => c.Id == request.IdCaLamViecMacDinh.Value, cancellationToken);
                        
                    if (shift == null)
                        throw new ApiException("Không tìm thấy ca làm việc đã chọn.");
                        
                    workingHours = shift.CalculateWorkingHours();
                }
                
                chiTiet.SoGioLam = workingHours;
                chiTiet.IdCaLamViecMacDinh = request.IdCaLamViecMacDinh;
            }
            else
            {
                chiTiet.SoGioLam = 0;
                chiTiet.IdCaLamViecMacDinh = null;
            }
            
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Cập nhật ngày thành công.");
        }
    }
}
