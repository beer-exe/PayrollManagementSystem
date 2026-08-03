using System;
using System.Threading;
using System.Threading.Tasks;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Common.Interfaces
{
    public interface ITimekeepingCalculatorService
    {
        Task<TimekeepingResult> CalculateTimekeepingAsync(
            string cccdNhanVien,
            DateOnly ngayChamCong,
            TimeOnly? gioVaoThucTe,
            TimeOnly? gioRaThucTe,
            CancellationToken cancellationToken = default);
    }

    public class TimekeepingResult
    {
        public decimal SoGioLamThucTe { get; set; }
        public decimal SoNgayCong { get; set; }
        public LoaiNgayCong LoaiNgayCong { get; set; }
        public int SoPhutDiTre { get; set; }
        public int SoPhutVeSom { get; set; }
        public string? GhiChu { get; set; }
    }
}
