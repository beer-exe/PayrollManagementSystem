using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ChamCong.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PayrollManagementSystem.Application.Features.ChamCong.Queries.GetMyChamCong
{
    public class GetMyChamCongQueryHandler : IRequestHandler<GetMyChamCongQuery, Response<List<ChamCongDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetMyChamCongQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<List<ChamCongDto>>> Handle(GetMyChamCongQuery request, CancellationToken cancellationToken)
        {
            var nhanVien = await _context.NhanViens
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.IdTaiKhoan == request.UserId, cancellationToken);

            if (nhanVien == null)
            {
                throw new ApiException("Không tìm thấy thông tin nhân viên.");
            }

            var list = await _context.ChamCongs
                .Include(cc => cc.NhanVien)
                .Where(cc => cc.CccdNhanVien == nhanVien.Cccd
                          && cc.NgayChamCong.Month == request.Thang
                          && cc.NgayChamCong.Year == request.Nam)
                .OrderBy(cc => cc.NgayChamCong)
                .ToListAsync(cancellationToken);

            var result = list.Select(cc => new ChamCongDto
            {
                Id = cc.Id,
                CccdNhanVien = cc.CccdNhanVien,
                HoTenNhanVien = cc.NhanVien.HoTen,
                NgayChamCong = cc.NgayChamCong.ToString("yyyy-MM-dd"),
                GioVao = cc.GioVao?.ToString("HH:mm"),
                GioRa = cc.GioRa?.ToString("HH:mm"),
                SoGioLamThucTe = cc.SoGioLamThucTe,
                SoNgayCong = cc.SoNgayCong,
                LoaiNgayCong = cc.LoaiNgayCong.GetDescription(),
                TrangThai = cc.TrangThai.GetDescription(),
                IsNhapTay = cc.IsNhapTay,
                GhiChu = cc.GhiChu,
                NgayTao = cc.CreatedAt.DateTime,
            }).ToList();

            return new Response<List<ChamCongDto>>(result);
        }
    }
}
