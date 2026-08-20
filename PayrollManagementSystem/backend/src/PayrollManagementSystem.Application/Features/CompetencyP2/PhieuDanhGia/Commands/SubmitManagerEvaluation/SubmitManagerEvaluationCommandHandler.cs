using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.SubmitManagerEvaluation
{
    public class SubmitManagerEvaluationCommandHandler : IRequestHandler<SubmitManagerEvaluationCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public SubmitManagerEvaluationCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(SubmitManagerEvaluationCommand request, CancellationToken cancellationToken)
        {
            var manager = await _context.NhanViens.FirstOrDefaultAsync(x => x.IdTaiKhoan == request.TaiKhoanId, cancellationToken);
            if (manager == null) return new Response<bool>("Không tìm thấy tài khoản quản lý.");

            var phieu = await _context.PhieuDanhGiaNangLucs
                .Include(x => x.ChiTietDanhGias)
                .ThenInclude(c => c.TieuChi)
                .FirstOrDefaultAsync(x => x.IdPhieu == request.IdPhieu, cancellationToken);

            if (phieu == null) return new Response<bool>("Không tìm thấy phiếu đánh giá.");

            bool isCurrentManager = false;
            var empQd = await _context.QuyetDinhNhanSus
                .Where(x => x.Cccd == phieu.CccdNhanVien && x.TrangThai == Domain.Enums.TrangThaiQuyetDinh.HIEU_LUC && x.NgayHieuLuc <= DateOnly.FromDateTime(DateTime.Today))
                .OrderByDescending(x => x.NgayHieuLuc)
                .ThenByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (empQd != null)
            {
                var empChucVu = await _context.ChucVus.FirstOrDefaultAsync(c => c.IdChucVu == empQd.IdChucVuMoi, cancellationToken);
                if (empChucVu != null && !string.IsNullOrEmpty(empChucVu.IdChucVuQuanLy))
                {
                    var userQd = await _context.QuyetDinhNhanSus
                        .Where(x => x.Cccd == manager.Cccd && x.TrangThai == Domain.Enums.TrangThaiQuyetDinh.HIEU_LUC && x.NgayHieuLuc <= DateOnly.FromDateTime(DateTime.Today))
                        .OrderByDescending(x => x.NgayHieuLuc)
                        .ThenByDescending(x => x.CreatedAt)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (userQd != null && userQd.IdChucVuMoi == empChucVu.IdChucVuQuanLy)
                    {
                        isCurrentManager = true;
                    }
                }
            }

            if (!request.IsHr && phieu.CccdQuanLy != manager.Cccd && !isCurrentManager)
            {
                return new Response<bool>("Bạn không có quyền duyệt phiếu này.");
            }
            if (phieu.TrangThai == Domain.Enums.TrangThaiPhieuDanhGia.DA_HOAN_THANH)
                return new Response<bool>("Phiếu này đã được chốt và hoàn thành.");

            decimal totalScore = 0;

            foreach (var reqChiTiet in request.ChiTiets)
            {
                var dbChiTiet = phieu.ChiTietDanhGias.FirstOrDefault(x => x.IdChiTiet == reqChiTiet.IdChiTiet);
                if (dbChiTiet != null)
                {
                    dbChiTiet.DiemQuanLyDanhGia = reqChiTiet.DiemQuanLyDanhGia;
                    dbChiTiet.NhanXetQuanLy = reqChiTiet.NhanXetQuanLy;

                    if (dbChiTiet.TieuChi != null)
                    {
                        totalScore += (decimal)reqChiTiet.DiemQuanLyDanhGia * dbChiTiet.TieuChi.TyTrong;
                    }
                }
            }

            phieu.NhanXetChung = request.NhanXetChung;

            if (request.IsSubmit)
            {
                phieu.DiemTongHop = totalScore;

                var mucQuyDois = await _context.MucQuyDoiP2s.ToListAsync(cancellationToken);
                var matched = mucQuyDois.FirstOrDefault(x => totalScore >= (decimal)x.DiemToiThieu && totalScore <= (decimal)x.DiemToiDa);

                if (matched != null)
                {
                    phieu.HeSoP2 = matched.HeSoP2;
                    phieu.XepLoai = matched.XepLoai;
                }
                else
                {
                    if (mucQuyDois.Any())
                    {
                        var max = mucQuyDois.OrderByDescending(x => x.DiemToiDa).First();
                        var min = mucQuyDois.OrderBy(x => x.DiemToiThieu).First();
                        if (totalScore > (decimal)max.DiemToiDa)
                        {
                            phieu.HeSoP2 = max.HeSoP2;
                            phieu.XepLoai = max.XepLoai;
                        }
                        else
                        {
                            phieu.HeSoP2 = min.HeSoP2;
                            phieu.XepLoai = min.XepLoai;
                        }
                    }
                }

                phieu.TrangThai = Domain.Enums.TrangThaiPhieuDanhGia.DA_HOAN_THANH;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, request.IsSubmit ? "Đã chốt phiếu thành công." : "Lưu nháp thành công.");
        }
    }
}
