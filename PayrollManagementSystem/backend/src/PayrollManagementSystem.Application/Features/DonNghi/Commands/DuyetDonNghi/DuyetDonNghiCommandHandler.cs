using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.DuyetDonNghi
{
    public class DuyetDonNghiCommandHandler : IRequestHandler<DuyetDonNghiCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DuyetDonNghiCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(DuyetDonNghiCommand request, CancellationToken cancellationToken)
        {
            var donNghi = await _context.DonNghis
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (donNghi == null)
                throw new ApiException("Không tìm thấy đơn nghỉ.");

            if (donNghi.TrangThai != TrangThaiDonNghi.CHO_DUYET)
                throw new ApiException("Chỉ có thể duyệt đơn đang ở trạng thái 'Chờ duyệt'.");

            if (donNghi.LoaiNghi == LoaiNghi.NGHI_PHEP_NAM)
            {
                var nam = donNghi.NgayBatDau.Year;
                var ngayPhep = await _context.NgayPhepNhanViens
                    .FirstOrDefaultAsync(n => n.CccdNhanVien == donNghi.CccdNhanVien && n.Nam == nam, cancellationToken);

                if (ngayPhep == null)
                    throw new ApiException($"Nhân viên chưa được cấu hình ngày phép năm {nam}. Vui lòng thiết lập trước khi duyệt.");

                if (ngayPhep.ConLai < donNghi.SoNgayNghi)
                    throw new ApiException($"Số ngày phép còn lại ({ngayPhep.ConLai}) không đủ cho đơn này ({donNghi.SoNgayNghi} ngày).");

                ngayPhep.DaSuDung += donNghi.SoNgayNghi;
            }

            donNghi.TrangThai = TrangThaiDonNghi.DA_DUYET;
            
            if (Guid.TryParse(request.CccdNguoiDuyet, out var userId))
            {
                var nguoiDuyetAccount = await _context.TaiKhoans.Include(t => t.NhanVien).FirstOrDefaultAsync(t => t.IdTaiKhoan == userId, cancellationToken);
                donNghi.CccdNguoiDuyet = nguoiDuyetAccount?.NhanVien?.Cccd ?? "SYSTEM";
            }
            else
            {
                donNghi.CccdNguoiDuyet = request.CccdNguoiDuyet;
            }

            donNghi.NgayDuyet = DateTime.Now;

            // Đồng bộ dữ liệu sang Chấm công
            var chiTietLich = await _context.ChiTietLichLamViecs
                .Include(c => c.CaLamViecMacDinh)
                    .ThenInclude(ca => ca.KhungGioNghis)
                .Where(c => c.Ngay >= donNghi.NgayBatDau && c.Ngay <= donNghi.NgayKetThuc && c.LoaiNgay == LoaiNgay.NGAY_LAM_VIEC)
                .OrderBy(c => c.Ngay)
                .ToListAsync(cancellationToken);

            var existingChamCongs = await _context.ChamCongs
                .Where(c => c.CccdNhanVien == donNghi.CccdNhanVien && c.NgayChamCong >= donNghi.NgayBatDau && c.NgayChamCong <= donNghi.NgayKetThuc)
                .ToListAsync(cancellationToken);

            var phanCongCas = await _context.PhanCongCas
                .Include(p => p.CaLamViec)
                    .ThenInclude(c => c.KhungGioNghis)
                .Where(p => p.CccdNhanVien == donNghi.CccdNhanVien 
                         && p.NgayLamViec >= donNghi.NgayBatDau 
                         && p.NgayLamViec <= donNghi.NgayKetThuc)
                .ToListAsync(cancellationToken);

            decimal remainingNgayNghi = donNghi.SoNgayNghi;
            if (donNghi.LoaiNghi == LoaiNghi.NGHI_THAI_SAN)
            {
                remainingNgayNghi = chiTietLich.Count; // For maternity, assign all working days within the calendar range
            }

            foreach (var lich in chiTietLich)
            {
                if (remainingNgayNghi <= 0) break;

                decimal ngayTru = Math.Min(1m, remainingNgayNghi);
                remainingNgayNghi -= ngayTru;

                var chamCong = existingChamCongs.FirstOrDefault(c => c.NgayChamCong == lich.Ngay);
                
                LoaiNgayCong loaiCongToAssign;
                if (ngayTru == 1m) 
                {
                    loaiCongToAssign = (donNghi.LoaiNghi == LoaiNghi.NGHI_KHONG_LUONG) 
                        ? LoaiNgayCong.VANG_CO_PHEP_KHONG_LUONG 
                        : LoaiNgayCong.VANG_CO_PHEP;
                }
                else 
                {
                    loaiCongToAssign = LoaiNgayCong.NUA_CA;
                }

                var assignedShift = phanCongCas.FirstOrDefault(p => p.NgayLamViec == lich.Ngay)?.CaLamViec 
                                    ?? lich.CaLamViecMacDinh;
                decimal shiftHours = assignedShift?.CalculateWorkingHours() ?? 8m;

                bool isPaidLeave = donNghi.LoaiNghi != LoaiNghi.NGHI_KHONG_LUONG;
                decimal soGio = isPaidLeave ? (ngayTru * shiftHours) : 0m;
                decimal soNgayCong = isPaidLeave ? ngayTru : 0m;

                if (chamCong == null)
                {
                    chamCong = new Domain.Models.ChamCong
                    {
                        CccdNhanVien = donNghi.CccdNhanVien,
                        NgayChamCong = lich.Ngay,
                        LoaiNgayCong = loaiCongToAssign,
                        SoGioLamThucTe = soGio,
                        SoNgayCong = soNgayCong,
                        GhiChu = $"Nghỉ phép: {donNghi.LyDo}",
                        TrangThai = TrangThaiChamCong.DA_XAC_NHAN
                    };
                    await _context.ChamCongs.AddAsync(chamCong, cancellationToken);
                }
                else
                {
                    chamCong.LoaiNgayCong = loaiCongToAssign;
                    chamCong.SoGioLamThucTe = soGio;
                    chamCong.SoNgayCong = soNgayCong;
                    chamCong.GhiChu = $"Nghỉ phép: {donNghi.LyDo}";
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Duyệt đơn nghỉ thành công.");
        }
    }
}
