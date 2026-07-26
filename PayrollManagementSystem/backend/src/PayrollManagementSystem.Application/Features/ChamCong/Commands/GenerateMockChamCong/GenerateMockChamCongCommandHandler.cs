using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.GenerateMockChamCong
{
    public class GenerateMockChamCongCommandHandler : IRequestHandler<GenerateMockChamCongCommand, FileDto>
    {
        private readonly IApplicationDbContext _context;

        public GenerateMockChamCongCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FileDto> Handle(GenerateMockChamCongCommand request, CancellationToken cancellationToken)
        {
            int year = request.Nam;
            int month = request.Thang;
            var daysInMonth = DateTime.DaysInMonth(year, month);

            // Fetch all active employees
            var employees = await _context.NhanViens
                .Where(nv => nv.TrangThai == TrangThaiNhanVien.DANG_LAM_VIEC)
                .Select(nv => nv.Cccd)
                .ToListAsync(cancellationToken);

            // Fetch ChiTietLichLamViec for the month
            var chiTietLichs = await _context.ChiTietLichLamViecs
                .Include(ct => ct.CaLamViecMacDinh)
                .Where(ct => ct.Ngay.Year == year && ct.Ngay.Month == month)
                .ToDictionaryAsync(ct => ct.Ngay, cancellationToken);

            // Fetch PhanCongCa for the month
            var phanCongCas = await _context.PhanCongCas
                .Include(p => p.CaLamViec)
                .Where(p => p.NgayLamViec.Year == year && p.NgayLamViec.Month == month)
                .ToListAsync(cancellationToken);

            // Fast lookup: Dictionary<Cccd, Dictionary<DateOnly, PhanCongCa>>
            var phanCongMap = phanCongCas
                .GroupBy(p => p.CccdNhanVien)
                .ToDictionary(g => g.Key, g => g.ToDictionary(p => p.NgayLamViec));

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("CCCD,NgayChamCong,GioVao,GioRa,GhiChu");

            var random = new Random();

            foreach (var cccd in employees)
            {
                phanCongMap.TryGetValue(cccd, out var empPhanCongs);

                for (int day = 1; day <= daysInMonth; day++)
                {
                    var currentDate = new DateOnly(year, month, day);

                    Domain.Models.CaLamViec? expectedCa = null;
                    bool isOff = false;

                    // 1. Check PhanCongCa
                    if (empPhanCongs != null && empPhanCongs.TryGetValue(currentDate, out var phanCong))
                    {
                        if (phanCong.IdCaLamViec == null)
                        {
                            isOff = true; // Assigned OFF
                        }
                        else
                        {
                            expectedCa = phanCong.CaLamViec;
                        }
                    }
                    else
                    {
                        // 2. Fallback to ChiTietLichLamViec
                        chiTietLichs.TryGetValue(currentDate, out var chiTiet);
                        if (chiTiet != null)
                        {
                            if (chiTiet.LoaiNgay == LoaiNgay.NGHI_LE || chiTiet.LoaiNgay == LoaiNgay.NGHI_CUOI_TUAN)
                            {
                                isOff = true;
                            }
                            else
                            {
                                expectedCa = chiTiet.CaLamViecMacDinh;
                            }
                        }
                    }

                    if (isOff || expectedCa == null)
                    {
                        // Leave empty rows to test VANG_KHONG_PHEP / NGHI_CUOI_TUAN logic on import
                        csvBuilder.AppendLine($"{cccd},{currentDate:dd/MM/yyyy},,,Không có lịch làm việc");
                        continue;
                    }

                    // 3. Generate Mock Times based on expectedCa
                    // Random scenarios: 
                    // 70% chance exact or early by up to 10 mins
                    // 20% chance late by up to 30 mins
                    // 10% chance absent (leave blank)

                    var r = random.Next(100);

                    if (r < 10)
                    {
                        // Absent
                        csvBuilder.AppendLine($"{cccd},{currentDate:dd/MM/yyyy},,,Quên chấm công / Vắng");
                    }
                    else
                    {
                        TimeSpan shiftStart = expectedCa.GioBatDau;
                        TimeSpan shiftEnd = expectedCa.GioKetThuc;
                        
                        // Night shift end time logic just for calculation
                        if (expectedCa.XuyenNgay || shiftEnd < shiftStart)
                        {
                            shiftEnd = shiftEnd.Add(TimeSpan.FromDays(1));
                        }

                        TimeSpan actualStart;
                        TimeSpan actualEnd;

                        if (r < 80) // 70% Normal / On-time
                        {
                            // Start earlier by 0 to 15 mins
                            int earlyStartMins = random.Next(0, 16);
                            actualStart = shiftStart.Subtract(TimeSpan.FromMinutes(earlyStartMins));

                            // End later by 0 to 15 mins
                            int lateEndMins = random.Next(0, 16);
                            actualEnd = shiftEnd.Add(TimeSpan.FromMinutes(lateEndMins));
                        }
                        else // 20% Late / Early leave
                        {
                            // Late start by 10 to 60 mins
                            int lateStartMins = random.Next(10, 61);
                            actualStart = shiftStart.Add(TimeSpan.FromMinutes(lateStartMins));

                            // Early end by 10 to 60 mins
                            int earlyEndMins = random.Next(10, 61);
                            actualEnd = shiftEnd.Subtract(TimeSpan.FromMinutes(earlyEndMins));
                        }

                        // Normalize formatting (00:00 to 23:59)
                        var gioVaoStr = NormalizeTime(actualStart);
                        var gioRaStr = NormalizeTime(actualEnd);

                        csvBuilder.AppendLine($"{cccd},{currentDate:dd/MM/yyyy},{gioVaoStr},{gioRaStr},Mocked");
                    }
                }
            }

            // UTF8 with BOM so Excel opens it correctly
            var preamble = Encoding.UTF8.GetPreamble();
            var data = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            var fileData = new byte[preamble.Length + data.Length];
            Buffer.BlockCopy(preamble, 0, fileData, 0, preamble.Length);
            Buffer.BlockCopy(data, 0, fileData, preamble.Length, data.Length);

            return new FileDto
            {
                Data = fileData,
                ContentType = "text/csv",
                FileName = $"Mock_ChamCong_{month:00}_{year}.csv"
            };
        }

        private string NormalizeTime(TimeSpan time)
        {
            // If it exceeds 24 hours (night shift end time), modulo 24
            var normalized = time.TotalHours >= 24 ? time.Subtract(TimeSpan.FromDays(1)) : time;
            if (normalized.TotalHours < 0) normalized = normalized.Add(TimeSpan.FromDays(1));
            return $"{normalized.Hours:00}:{normalized.Minutes:00}";
        }
    }
}
