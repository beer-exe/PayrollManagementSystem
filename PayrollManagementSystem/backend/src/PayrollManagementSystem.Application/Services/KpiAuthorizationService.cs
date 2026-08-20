using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Services
{
    public class KpiAuthorizationService : IKpiAuthorizationService
    {
        private readonly IApplicationDbContext _context;

        public KpiAuthorizationService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetSubordinateCccdsAsync(Guid managerTaiKhoanId, CancellationToken cancellationToken)
        {
            var quanLy = await _context.NhanViens
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.IdTaiKhoan == managerTaiKhoanId, cancellationToken);

            if (quanLy == null) return new List<string>();

            var managerQd = await _context.QuyetDinhNhanSus
                .AsNoTracking()
                .Where(q => q.Cccd == quanLy.Cccd && q.TrangThai == Domain.Enums.TrangThaiQuyetDinh.HIEU_LUC)
                .OrderByDescending(q => q.NgayHieuLuc)
                .FirstOrDefaultAsync(cancellationToken);

            if (managerQd == null || string.IsNullOrEmpty(managerQd.IdChucVuMoi))
                return new List<string>();

            var subordinateChucVus = await _context.ChucVus
                .AsNoTracking()
                .Where(c => c.IdChucVuQuanLy == managerQd.IdChucVuMoi)
                .Select(c => c.IdChucVu)
                .ToListAsync(cancellationToken);

            if (!subordinateChucVus.Any())
                return new List<string>();

            var subordinateCccds = await _context.QuyetDinhNhanSus
                .AsNoTracking()
                .Where(q => q.TrangThai == Domain.Enums.TrangThaiQuyetDinh.HIEU_LUC && subordinateChucVus.Contains(q.IdChucVuMoi!))
                .Select(q => q.Cccd!)
                .Distinct()
                .ToListAsync(cancellationToken);

            return subordinateCccds;
        }

        public async Task<bool> CanManageAsync(Guid managerTaiKhoanId, string subordinateCccd, CancellationToken cancellationToken)
        {
            var subordinateCccds = await GetSubordinateCccdsAsync(managerTaiKhoanId, cancellationToken);
            return subordinateCccds.Contains(subordinateCccd);
        }
    }
}
