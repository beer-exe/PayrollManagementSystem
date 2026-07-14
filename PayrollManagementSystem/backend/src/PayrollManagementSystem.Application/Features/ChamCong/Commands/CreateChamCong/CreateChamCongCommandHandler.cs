using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Extensions;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.CreateChamCong
{
    public class CreateChamCongCommandHandler : IRequestHandler<CreateChamCongCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;

        // Cấu hình nghiệp vụ tính ngày công
        private const decimal GIO_TIEU_CHUAN = 8m;       // Số giờ tiêu chuẩn một ca
        private const decimal GRACE_PERIOD_PHUT = 15m;   // Phút ân hạn đi trễ/về sớm
        private const decimal GIO_NGHI_TRUA = 1m;        // Giờ nghỉ trưa trừ khi làm > 5h

        public CreateChamCongCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Guid>> Handle(CreateChamCongCommand request, CancellationToken cancellationToken)
        {
            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(nv => nv.Cccd == request.CccdNhanVien, cancellationToken);

            if (nhanVien == null)
                throw new ApiException($"Không tìm thấy nhân viên với CCCD: {request.CccdNhanVien}");

            var exists = await _context.ChamCongs
                .AnyAsync(cc => cc.CccdNhanVien == request.CccdNhanVien
                             && cc.NgayChamCong == request.NgayChamCong, cancellationToken);

            if (exists)
                throw new ApiException($"Đã tồn tại bản ghi chấm công của nhân viên {nhanVien.HoTen} vào ngày {request.NgayChamCong:dd/MM/yyyy}.");

            var chiTietLich = await _context.ChiTietLichLamViecs
                .FirstOrDefaultAsync(ct => ct.Ngay == request.NgayChamCong, cancellationToken);

            var loaiNgayTrongLich = chiTietLich?.LoaiNgay ?? LoaiNgay.NGAY_LAM_VIEC;
            var soGioChuanNgay = chiTietLich?.SoGioLam ?? GIO_TIEU_CHUAN;

            decimal soGioLamThucTe = 0;
            LoaiNgayCong loaiNgayCong;
            decimal soNgayCong = 0;

            if (loaiNgayTrongLich == LoaiNgay.NGHI_LE)
            {
                loaiNgayCong = LoaiNgayCong.NGHI_LE;
                soNgayCong = 0;
                soGioLamThucTe = 0;
            }
            else if (loaiNgayTrongLich == LoaiNgay.NGHI_CUOI_TUAN)
            {
                loaiNgayCong = LoaiNgayCong.NGHI_CUOI_TUAN;
                soNgayCong = 0;
                soGioLamThucTe = 0;
            }
            else if (request.GioVao == null || request.GioRa == null)
            {
                loaiNgayCong = LoaiNgayCong.VANG_KHONG_PHEP;
                soNgayCong = 0;
                soGioLamThucTe = 0;
            }
            else
            {
                var tongGioRaw = (decimal)(request.GioRa.Value - request.GioVao.Value).TotalHours;
                if (tongGioRaw < 0) tongGioRaw = 0;

                soGioLamThucTe = tongGioRaw > 5 ? tongGioRaw - GIO_NGHI_TRUA : tongGioRaw;
                soGioLamThucTe = Math.Round(Math.Max(soGioLamThucTe, 0), 2);

                var gracePeriodGio = GRACE_PERIOD_PHUT / 60m;
                var soGioHieuQua = Math.Min(soGioLamThucTe, soGioChuanNgay); // không vượt giờ chuẩn
                var heSo = soGioHieuQua / soGioChuanNgay;

                if (heSo >= 1m - (gracePeriodGio / soGioChuanNgay))
                {
                    soNgayCong = 1m;
                    loaiNgayCong = LoaiNgayCong.LAM_DU_CA;
                }
                else if (heSo >= 0.5m - (gracePeriodGio / soGioChuanNgay) && heSo < 1m)
                {
                    soNgayCong = Math.Round(heSo, 2);
                    loaiNgayCong = Math.Abs(heSo - 0.5m) < 0.01m
                        ? LoaiNgayCong.NUA_CA
                        : LoaiNgayCong.DI_TRE_VE_SOM;
                }
                else
                {
                    soNgayCong = Math.Round(heSo, 2);
                    loaiNgayCong = LoaiNgayCong.DI_TRE_VE_SOM;
                }
            }

            var chamCong = new Domain.Models.ChamCong
            {
                Id = Guid.NewGuid(),
                CccdNhanVien = request.CccdNhanVien,
                NgayChamCong = request.NgayChamCong,
                GioVao = request.GioVao,
                GioRa = request.GioRa,
                SoGioLamThucTe = soGioLamThucTe,
                SoNgayCong = soNgayCong,
                LoaiNgayCong = loaiNgayCong,
                IsNhapTay = true,
                GhiChu = request.GhiChu,
                TrangThai = TrangThaiChamCong.DA_XAC_NHAN
            };

            await _context.ChamCongs.AddAsync(chamCong, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(chamCong.Id,
                $"Nhập chấm công thành công cho nhân viên {nhanVien.HoTen} ngày {request.NgayChamCong:dd/MM/yyyy}. " +
                $"Số ngày công: {soNgayCong:F2} ({loaiNgayCong.GetDescription()}).");
        }
    }
}
