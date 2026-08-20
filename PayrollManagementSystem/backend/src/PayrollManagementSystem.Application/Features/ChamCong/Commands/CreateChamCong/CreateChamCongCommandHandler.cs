using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.CreateChamCong
{
    public class CreateChamCongCommandHandler : IRequestHandler<CreateChamCongCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITimekeepingCalculatorService _calculatorService;

        public CreateChamCongCommandHandler(IApplicationDbContext context, ITimekeepingCalculatorService calculatorService)
        {
            _context = context;
            _calculatorService = calculatorService;
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

            var isKyLuongClosed = await _context.KyLuongs
                .AnyAsync(kl => kl.TrangThai != TrangThaiKyLuong.CHUA_CHOT
                             && request.NgayChamCong >= kl.NgayBatDau
                             && request.NgayChamCong <= kl.NgayKetThuc, cancellationToken);

            if (isKyLuongClosed)
                throw new ApiException("Không thể thêm dữ liệu chấm công vì kỳ lương tương ứng đã được chốt.");

            var isKyChamCongClosed = await _context.KyChamCongs
                .AnyAsync(kcc => kcc.Thang == request.NgayChamCong.Month
                              && kcc.Nam == request.NgayChamCong.Year
                              && kcc.TrangThai == TrangThaiKyChamCong.DA_CHOT, cancellationToken);

            if (isKyChamCongClosed)
                throw new ApiException($"Không thể thêm dữ liệu chấm công vì kỳ chấm công tháng {request.NgayChamCong.Month}/{request.NgayChamCong.Year} đã được chốt.");

            var calcResult = await _calculatorService.CalculateTimekeepingAsync(
                request.CccdNhanVien,
                request.NgayChamCong,
                request.GioVao,
                request.GioRa,
                cancellationToken);

            var chamCong = new Domain.Models.ChamCong
            {
                Id = Guid.NewGuid(),
                CccdNhanVien = request.CccdNhanVien,
                NgayChamCong = request.NgayChamCong,
                GioVao = request.GioVao,
                GioRa = request.GioRa,
                SoGioLamThucTe = calcResult.SoGioLamThucTe,
                SoNgayCong = calcResult.SoNgayCong,
                LoaiNgayCong = calcResult.LoaiNgayCong,
                SoPhutDiTre = calcResult.SoPhutDiTre,
                SoPhutVeSom = calcResult.SoPhutVeSom,
                IsNhapTay = true,
                GhiChu = request.GhiChu ?? calcResult.GhiChu,
                TrangThai = TrangThaiChamCong.CHUA_XAC_NHAN
            };

            await _context.ChamCongs.AddAsync(chamCong, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(chamCong.Id,
                $"Nhập chấm công thành công cho nhân viên {nhanVien.HoTen} ngày {request.NgayChamCong:dd/MM/yyyy}. " +
                $"Số ngày công: {calcResult.SoNgayCong:F2} ({calcResult.LoaiNgayCong.GetDescription()}).");
        }
    }
}
