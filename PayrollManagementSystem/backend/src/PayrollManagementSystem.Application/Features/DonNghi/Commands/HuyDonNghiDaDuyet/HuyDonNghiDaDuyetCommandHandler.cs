using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.HuyDonNghiDaDuyet
{
    public class HuyDonNghiDaDuyetCommandHandler : IRequestHandler<HuyDonNghiDaDuyetCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public HuyDonNghiDaDuyetCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(HuyDonNghiDaDuyetCommand request, CancellationToken cancellationToken)
        {
            var donNghi = await _context.DonNghis
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (donNghi == null)
                throw new ApiException("Không tìm thấy đơn nghỉ.");

            if (donNghi.TrangThai != TrangThaiDonNghi.DA_DUYET)
                throw new ApiException("Chỉ có thể hủy đơn đang ở trạng thái 'Đã duyệt'.");

            if (DateOnly.FromDateTime(DateTime.Now) >= donNghi.NgayBatDau)
                throw new ApiException("Chỉ có thể hủy đơn đã duyệt trước khi ngày nghỉ bắt đầu.");


            if (donNghi.LoaiNghi == LoaiNghi.NGHI_PHEP_NAM)
            {
                var nam = donNghi.NgayBatDau.Year;
                var ngayPhep = await _context.NgayPhepNhanViens
                    .FirstOrDefaultAsync(n => n.CccdNhanVien == donNghi.CccdNhanVien && n.Nam == nam, cancellationToken);

                if (ngayPhep != null)
                {
                    ngayPhep.DaSuDung -= donNghi.SoNgayNghi;
                    if (ngayPhep.DaSuDung < 0) ngayPhep.DaSuDung = 0;
                }
            }

            var chamCongsToRevert = await _context.ChamCongs
                .Where(c => c.CccdNhanVien == donNghi.CccdNhanVien 
                            && c.NgayChamCong >= donNghi.NgayBatDau 
                            && c.NgayChamCong <= donNghi.NgayKetThuc
                            && (c.LoaiNgayCong == LoaiNgayCong.VANG_CO_PHEP || c.LoaiNgayCong == LoaiNgayCong.NUA_CA))
                .ToListAsync(cancellationToken);

            foreach (var cc in chamCongsToRevert)
            {
                cc.LoaiNgayCong = LoaiNgayCong.LAM_DU_CA;
                cc.SoGioLamThucTe = 8;
                cc.SoNgayCong = 1;
                cc.GhiChu = "Đã hủy đơn nghỉ";
                cc.TrangThai = TrangThaiChamCong.CHUA_XAC_NHAN;
                
                // Note: Alternatively, we could delete the ChamCong record if it was completely auto-generated,
                // but setting it back to a default state is safer if there was other attendance data.
            }

            donNghi.TrangThai = TrangThaiDonNghi.TU_CHOI; 
            donNghi.LyDoTuChoi = "Hủy đơn đã duyệt theo yêu cầu";

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Hủy đơn đã duyệt thành công và đã hoàn trả ngày phép.");
        }
    }
}
