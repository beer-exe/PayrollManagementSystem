using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.ChangeStatusKyDanhGia
{
    public class ChangeStatusKyDanhGiaCommandHandler : IRequestHandler<ChangeStatusKyDanhGiaCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public ChangeStatusKyDanhGiaCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(ChangeStatusKyDanhGiaCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.KyDanhGias.FindAsync(new object[] { request.IdKyDanhGia }, cancellationToken);
            if (entity == null) return new Response<bool>("Không tìm thấy kỳ đánh giá.");

            if (request.TrangThaiMoi == TrangThaiKyDanhGia.DANG_DANH_GIA)
            {
                if (entity.TrangThai != TrangThaiKyDanhGia.KHOI_TAO)
                    return new Response<bool>("Chỉ có thể mở đánh giá từ trạng thái Khởi tạo.");

                // Lệnh gửi Email thông báo
                // await _mediator.Send(new NotifyEvaluationOpenedCommand(request.IdKyDanhGia), cancellationToken);
            }
            else if (request.TrangThaiMoi == TrangThaiKyDanhGia.DA_CHOT)
            {
                if (entity.TrangThai != TrangThaiKyDanhGia.DANG_DANH_GIA)
                    return new Response<bool>("Chỉ có thể chốt khi kỳ đang diễn ra.");

                var phieus = await _context.PhieuDanhGiaNangLucs
                    .Where(x => x.IdKyDanhGia == request.IdKyDanhGia)
                    .ToListAsync(cancellationToken);

                if (!request.Force && phieus.Any(p => p.TrangThai != TrangThaiPhieuDanhGia.DA_HOAN_THANH))
                {
                    return new Response<bool>("HienTaiCoPhieuChuaXong");
                }

                var configs = await _context.MucQuyDoiP2s.ToListAsync(cancellationToken);
                var nhanViens = await _context.NhanViens.ToDictionaryAsync(n => n.Cccd, cancellationToken);

                foreach (var phieu in phieus.Where(p => p.TrangThai == TrangThaiPhieuDanhGia.DA_HOAN_THANH))
                {
                    decimal diem = phieu.DiemTongHop ?? 0;
                    var matchedConfig = configs.FirstOrDefault(c => diem >= c.DiemToiThieu && diem <= c.DiemToiDa);
                    if (matchedConfig != null)
                    {
                        phieu.HeSoP2 = matchedConfig.HeSoP2;
                        phieu.XepLoai = matchedConfig.XepLoai;

                        if (nhanViens.TryGetValue(phieu.CccdNhanVien, out var nv))
                        {
                            nv.HeSoP2 = matchedConfig.HeSoP2;
                        }
                    }
                }
            }
            else if (request.TrangThaiMoi == TrangThaiKyDanhGia.DA_HUY)
            {
                var phieus = await _context.PhieuDanhGiaNangLucs
                    .Where(x => x.IdKyDanhGia == request.IdKyDanhGia)
                    .ToListAsync(cancellationToken);

                foreach (var phieu in phieus)
                {
                    phieu.TrangThai = TrangThaiPhieuDanhGia.DA_HUY;
                    phieu.NhanXetChung = "Đã hủy do Kỳ đánh giá bị hủy.";
                }
            }

            entity.TrangThai = request.TrangThaiMoi;
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Đã cập nhật trạng thái kỳ đánh giá thành công.");
        }
    }
}
