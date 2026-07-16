using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.UpdateChamCong
{
    public class UpdateChamCongCommandHandler : IRequestHandler<UpdateChamCongCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        private const decimal GIO_TIEU_CHUAN = 8m;
        private const decimal GRACE_PERIOD_PHUT = 15m;
        private const decimal GIO_NGHI_TRUA = 1m;

        public UpdateChamCongCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(UpdateChamCongCommand request, CancellationToken cancellationToken)
        {
            var chamCong = await _context.ChamCongs
                .FirstOrDefaultAsync(cc => cc.Id == request.Id, cancellationToken);

            if (chamCong == null)
                throw new ApiException($"Không tìm thấy bản ghi chấm công.");

            var chiTietLich = await _context.ChiTietLichLamViecs
                .FirstOrDefaultAsync(ct => ct.Ngay == chamCong.NgayChamCong, cancellationToken);

            var loaiNgayTrongLich = chiTietLich?.LoaiNgay ?? LoaiNgay.NGAY_LAM_VIEC;
            var soGioChuanNgay = chiTietLich?.SoGioLam ?? GIO_TIEU_CHUAN;

            chamCong.GioVao = request.GioVao;
            chamCong.GioRa = request.GioRa;
            chamCong.GhiChu = request.GhiChu;
            chamCong.IsNhapTay = true;

            if (loaiNgayTrongLich is LoaiNgay.NGHI_LE or LoaiNgay.NGHI_CUOI_TUAN)
            {
                chamCong.SoGioLamThucTe = 0;
                chamCong.SoNgayCong = 0;
                chamCong.LoaiNgayCong = loaiNgayTrongLich == LoaiNgay.NGHI_LE
                    ? LoaiNgayCong.NGHI_LE : LoaiNgayCong.NGHI_CUOI_TUAN;
            }
            else if (request.GioVao == null || request.GioRa == null)
            {
                chamCong.SoGioLamThucTe = 0;
                chamCong.SoNgayCong = 0;
                chamCong.LoaiNgayCong = LoaiNgayCong.VANG_KHONG_PHEP;
            }
            else
            {
                var tongGioRaw = (decimal)(request.GioRa.Value - request.GioVao.Value).TotalHours;
                if (tongGioRaw < 0) tongGioRaw = 0;
                var soGioLam = tongGioRaw > 5 ? tongGioRaw - GIO_NGHI_TRUA : tongGioRaw;
                soGioLam = Math.Round(Math.Max(soGioLam, 0), 2);

                var gracePeriodGio = GRACE_PERIOD_PHUT / 60m;
                var heSo = Math.Min(soGioLam, soGioChuanNgay) / soGioChuanNgay;

                if (heSo >= 1m - (gracePeriodGio / soGioChuanNgay))
                {
                    chamCong.SoNgayCong = 1m;
                    chamCong.LoaiNgayCong = LoaiNgayCong.LAM_DU_CA;
                }
                else if (heSo >= 0.5m - (gracePeriodGio / soGioChuanNgay))
                {
                    chamCong.SoNgayCong = Math.Round(heSo, 2);
                    chamCong.LoaiNgayCong = Math.Abs(heSo - 0.5m) < 0.01m
                        ? LoaiNgayCong.NUA_CA : LoaiNgayCong.DI_TRE_VE_SOM;
                }
                else
                {
                    chamCong.SoNgayCong = Math.Round(heSo, 2);
                    chamCong.LoaiNgayCong = LoaiNgayCong.DI_TRE_VE_SOM;
                }

                chamCong.SoGioLamThucTe = soGioLam;
                chamCong.TrangThai = TrangThaiChamCong.DA_XAC_NHAN;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Cập nhật chấm công thành công.");
        }
    }
}
