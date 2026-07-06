using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.PhieuDanhGia.Commands.SubmitTuDanhGia
{
    public class SubmitTuDanhGiaCommandHandler : IRequestHandler<SubmitTuDanhGiaCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public SubmitTuDanhGiaCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(SubmitTuDanhGiaCommand request, CancellationToken cancellationToken)
        {
            var phieu = await _context.PhieuDanhGiaNangLucs
                .Include(x => x.ChiTietDanhGias)
                .FirstOrDefaultAsync(x => x.IdPhieu == request.IdPhieu, cancellationToken);

            if (phieu == null) return new Response<bool>("Không tìm thấy phiếu.");
            if (phieu.TrangThai != Domain.Enums.TrangThaiPhieuDanhGia.CHO_NV_DANH_GIA)
                return new Response<bool>("Phiếu không ở trạng thái chờ nhân viên đánh giá.");

            foreach (var reqChiTiet in request.ChiTiets)
            {
                var dbChiTiet = phieu.ChiTietDanhGias.FirstOrDefault(x => x.IdChiTiet == reqChiTiet.IdChiTiet);
                if (dbChiTiet != null)
                {
                    dbChiTiet.DiemTuDanhGia = reqChiTiet.DiemTuDanhGia;
                    dbChiTiet.NhanXetNhanVien = reqChiTiet.NhanXetNhanVien;
                }
            }

            if (request.IsSubmit)
            {
                phieu.TrangThai = Domain.Enums.TrangThaiPhieuDanhGia.CHO_QL_DANH_GIA;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Lưu đánh giá thành công.");
        }
    }
}
