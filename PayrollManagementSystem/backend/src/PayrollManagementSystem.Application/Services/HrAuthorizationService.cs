using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Services
{
    public class HrAuthorizationService : IHrAuthorizationService
    {
        private readonly IApplicationDbContext _context;

        public HrAuthorizationService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsHrManagerAsync(Guid? userId, CancellationToken cancellationToken = default)
        {
            if (!userId.HasValue) return false;

            var taiKhoan = await _context.TaiKhoans
                .AsNoTracking()
                .Include(t => t.VaiTro)
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(t => t.IdTaiKhoan == userId.Value, cancellationToken);

            if (taiKhoan == null || taiKhoan.VaiTro == null) return false;

            // Nghiệp vụ chỉ chấp nhận role HR cấp quản lý (Không chấp nhận Admin hoặc các role khác)
            if (taiKhoan.VaiTro.TenVaiTro != "HR") return false;

            if (taiKhoan.NhanVien == null || string.IsNullOrEmpty(taiKhoan.NhanVien.Cccd)) return false;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var activeQd = await _context.QuyetDinhNhanSus
                .AsNoTracking()
                .Where(q => q.Cccd == taiKhoan.NhanVien.Cccd && q.TrangThai == TrangThaiQuyetDinh.HIEU_LUC && q.NgayHieuLuc <= today)
                .OrderByDescending(q => q.NgayHieuLuc)
                .ThenByDescending(q => q.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (activeQd == null || string.IsNullOrEmpty(activeQd.IdChucVuMoi)) return false;

            var hasDirectReports = await _context.ChucVus
                .AsNoTracking()
                .AnyAsync(c => c.IdChucVuQuanLy == activeQd.IdChucVuMoi, cancellationToken);

            return hasDirectReports;
        }
    }
}
