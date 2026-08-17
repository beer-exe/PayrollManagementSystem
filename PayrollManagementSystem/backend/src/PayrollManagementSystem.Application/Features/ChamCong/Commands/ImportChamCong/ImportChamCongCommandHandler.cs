using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ChamCong.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.ImportChamCong
{
    public class ImportChamCongCommandHandler : IRequestHandler<ImportChamCongCommand, Response<ImportChamCongResultDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITimekeepingCalculatorService _calculatorService;

        public ImportChamCongCommandHandler(IApplicationDbContext context, ITimekeepingCalculatorService calculatorService)
        {
            _context = context;
            _calculatorService = calculatorService;
        }

        public async Task<Response<ImportChamCongResultDto>> Handle(ImportChamCongCommand request, CancellationToken cancellationToken)
        {
            if (request.FileStream == null || request.FileStream.Length == 0)
                throw new ApiException("File không hợp lệ hoặc rỗng.");

            var extension = Path.GetExtension(request.FileName).ToLower();
            if (extension != ".csv")
                throw new ApiException("Chỉ hỗ trợ file định dạng CSV (.csv).");

            // Parse CSV
            var rows = new List<string[]>();
            using var reader = new System.IO.StreamReader(request.FileStream);

            var headerLine = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(headerLine))
                throw new ApiException("File CSV không có header hoặc rỗng.");

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (!string.IsNullOrWhiteSpace(line))
                    rows.Add(line.Split(','));
            }

            if (rows.Count == 0)
                throw new ApiException("File CSV không có dữ liệu (chỉ có header).");

            var allCccd = rows
                .Where(r => r.Length >= 1)
                .Select(r => r[0].Trim())
                .Distinct()
                .ToList();

            var nhanVienMap = await _context.NhanViens
                .Where(nv => allCccd.Contains(nv.Cccd))
                .ToDictionaryAsync(nv => nv.Cccd, cancellationToken);

            var parsedDates = rows
                .Where(r => r.Length >= 2)
                .Select(r => DateOnly.TryParseExact(r[1].Trim(), "dd/MM/yyyy", out var d) ? d : (DateOnly?)null)
                .Where(d => d.HasValue)
                .Select(d => d!.Value)
                .Distinct()
                .ToList();

            var lichLamViecMap = await _context.ChiTietLichLamViecs
                .Where(ct => parsedDates.Contains(ct.Ngay))
                .ToDictionaryAsync(ct => ct.Ngay, cancellationToken);

            var existingSet = await _context.ChamCongs
                .Where(cc => allCccd.Contains(cc.CccdNhanVien))
                .Select(cc => new { cc.CccdNhanVien, cc.NgayChamCong })
                .ToListAsync(cancellationToken);

            var existingKeySet = existingSet
                .Select(e => $"{e.CccdNhanVien}_{e.NgayChamCong:yyyy-MM-dd}")
                .ToHashSet();

            var minDate = parsedDates.Any() ? parsedDates.Min() : DateOnly.MinValue;
            var maxDate = parsedDates.Any() ? parsedDates.Max() : DateOnly.MaxValue;
            var closedKyLuongs = await _context.KyLuongs
                .Where(kl => kl.TrangThai != TrangThaiKyLuong.CHUA_CHOT
                          && kl.NgayKetThuc >= minDate
                          && kl.NgayBatDau <= maxDate)
                .ToListAsync(cancellationToken);

            var result = new ImportChamCongResultDto { TongSoDong = rows.Count };
            var toInsert = new List<Domain.Models.ChamCong>();

            for (int i = 0; i < rows.Count; i++)
            {
                var rowNum = i + 2; // +2 vì đã bỏ header (dòng 1)
                var cols = rows[i];

                try
                {
                    if (cols.Length < 2)
                        throw new Exception("Thiếu cột dữ liệu (tối thiểu: CCCD, NgayChamCong).");

                    var cccd = cols[0].Trim();
                    if (string.IsNullOrEmpty(cccd))
                        throw new Exception("CCCD không được để trống.");

                    if (!nhanVienMap.ContainsKey(cccd))
                        throw new Exception($"Không tìm thấy nhân viên với CCCD: {cccd}.");

                    if (!DateOnly.TryParseExact(cols[1].Trim(), "dd/MM/yyyy", out var ngay))
                        throw new Exception($"NgayChamCong '{cols[1].Trim()}' không đúng định dạng dd/MM/yyyy.");

                    if (ngay > DateOnly.FromDateTime(DateTime.Today))
                        throw new Exception($"Ngày chấm công {ngay:dd/MM/yyyy} không được lớn hơn ngày hiện tại.");

                    if (closedKyLuongs.Any(kl => ngay >= kl.NgayBatDau && ngay <= kl.NgayKetThuc))
                        throw new Exception($"Không thể import vì kỳ lương của ngày {ngay:dd/MM/yyyy} đã được chốt.");

                    var key = $"{cccd}_{ngay:yyyy-MM-dd}";
                    if (existingKeySet.Contains(key))
                        throw new Exception($"Đã tồn tại chấm công của nhân viên {cccd} ngày {ngay:dd/MM/yyyy}.");

                    TimeOnly? gioVao = null;
                    TimeOnly? gioRa = null;

                    if (cols.Length >= 3 && !string.IsNullOrWhiteSpace(cols[2]))
                    {
                        if (!TimeOnly.TryParseExact(cols[2].Trim(), "HH:mm", out var gv))
                            throw new Exception($"GioVao '{cols[2].Trim()}' không đúng định dạng HH:mm.");
                        gioVao = gv;
                    }

                    if (cols.Length >= 4 && !string.IsNullOrWhiteSpace(cols[3]))
                    {
                        if (!TimeOnly.TryParseExact(cols[3].Trim(), "HH:mm", out var gr))
                            throw new Exception($"GioRa '{cols[3].Trim()}' không đúng định dạng HH:mm.");
                        gioRa = gr;
                    }

                    var ghiChu = cols.Length >= 5 ? cols[4].Trim() : null;

                    var calcResult = await _calculatorService.CalculateTimekeepingAsync(
                        cccd,
                        ngay,
                        gioVao,
                        gioRa,
                        cancellationToken);

                    toInsert.Add(new Domain.Models.ChamCong
                    {
                        Id = Guid.NewGuid(),
                        CccdNhanVien = cccd,
                        NgayChamCong = ngay,
                        GioVao = gioVao,
                        GioRa = gioRa,
                        SoGioLamThucTe = calcResult.SoGioLamThucTe,
                        SoNgayCong = calcResult.SoNgayCong,
                        LoaiNgayCong = calcResult.LoaiNgayCong,
                        SoPhutDiTre = calcResult.SoPhutDiTre,
                        SoPhutVeSom = calcResult.SoPhutVeSom,
                        IsNhapTay = false,
                        GhiChu = ghiChu ?? calcResult.GhiChu,
                        TrangThai = TrangThaiChamCong.DA_XAC_NHAN
                    });

                    // Thêm vào set để tránh trùng trong cùng 1 file
                    existingKeySet.Add(key);
                    result.ThanhCong++;
                }
                catch (Exception ex)
                {
                    result.ThatBai++;
                    result.LoiNhap.Add($"Dòng {rowNum}: {ex.Message}");
                }
            }

            if (toInsert.Count > 0)
            {
                await _context.ChamCongs.AddRangeAsync(toInsert, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return new Response<ImportChamCongResultDto>(result,
                $"Import hoàn tất: {result.ThanhCong} thành công, {result.ThatBai} thất bại.");
        }
    }
}
