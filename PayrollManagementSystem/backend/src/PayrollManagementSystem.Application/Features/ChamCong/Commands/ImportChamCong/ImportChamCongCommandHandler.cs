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

        private const decimal GIO_TIEU_CHUAN = 8m;
        private const decimal GRACE_PERIOD_PHUT = 15m;
        private const decimal GIO_NGHI_TRUA = 1m;

        public ImportChamCongCommandHandler(IApplicationDbContext context)
        {
            _context = context;
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

                    if (gioVao.HasValue && gioRa.HasValue && gioRa <= gioVao)
                        throw new Exception("GioRa phải sau GioVao.");

                    var ghiChu = cols.Length >= 5 ? cols[4].Trim() : null;

                    lichLamViecMap.TryGetValue(ngay, out var chiTietLich);
                    var loaiNgayTrongLich = chiTietLich?.LoaiNgay ?? LoaiNgay.NGAY_LAM_VIEC;
                    var soGioChuanNgay = chiTietLich?.SoGioLam ?? GIO_TIEU_CHUAN;

                    decimal soGioLam = 0;
                    LoaiNgayCong loaiNgayCong;
                    decimal soNgayCong = 0;

                    if (loaiNgayTrongLich == LoaiNgay.NGHI_LE)
                    {
                        loaiNgayCong = LoaiNgayCong.NGHI_LE;
                    }
                    else if (loaiNgayTrongLich == LoaiNgay.NGHI_CUOI_TUAN)
                    {
                        loaiNgayCong = LoaiNgayCong.NGHI_CUOI_TUAN;
                    }
                    else if (gioVao == null || gioRa == null)
                    {
                        loaiNgayCong = LoaiNgayCong.VANG_KHONG_PHEP;
                    }
                    else
                    {
                        var tongGioRaw = (decimal)(gioRa.Value - gioVao.Value).TotalHours;
                        if (tongGioRaw < 0) tongGioRaw = 0;
                        soGioLam = tongGioRaw > 5 ? tongGioRaw - GIO_NGHI_TRUA : tongGioRaw;
                        soGioLam = Math.Round(Math.Max(soGioLam, 0), 2);

                        var gracePeriodGio = GRACE_PERIOD_PHUT / 60m;
                        var heSo = Math.Min(soGioLam, soGioChuanNgay) / soGioChuanNgay;

                        if (heSo >= 1m - (gracePeriodGio / soGioChuanNgay))
                        {
                            soNgayCong = 1m;
                            loaiNgayCong = LoaiNgayCong.LAM_DU_CA;
                        }
                        else if (heSo >= 0.5m - (gracePeriodGio / soGioChuanNgay))
                        {
                            soNgayCong = Math.Round(heSo, 2);
                            loaiNgayCong = Math.Abs(heSo - 0.5m) < 0.01m
                                ? LoaiNgayCong.NUA_CA : LoaiNgayCong.DI_TRE_VE_SOM;
                        }
                        else
                        {
                            soNgayCong = Math.Round(heSo, 2);
                            loaiNgayCong = LoaiNgayCong.DI_TRE_VE_SOM;
                        }
                    }

                    toInsert.Add(new Domain.Models.ChamCong
                    {
                        Id = Guid.NewGuid(),
                        CccdNhanVien = cccd,
                        NgayChamCong = ngay,
                        GioVao = gioVao,
                        GioRa = gioRa,
                        SoGioLamThucTe = soGioLam,
                        SoNgayCong = soNgayCong,
                        LoaiNgayCong = loaiNgayCong,
                        IsNhapTay = false,
                        GhiChu = ghiChu,
                        TrangThai = TrangThaiChamCong.CHUA_XAC_NHAN
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
