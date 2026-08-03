using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.WorkShifts.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.WorkShifts.Queries.GetCaLamViecs
{
    public class GetCaLamViecsQueryHandler : IRequestHandler<GetCaLamViecsQuery, Response<List<CaLamViecDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetCaLamViecsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<List<CaLamViecDto>>> Handle(GetCaLamViecsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.CaLamViecs
                .Include(c => c.KhungGioNghis)
                .AsNoTracking()
                .AsQueryable();

            if (request.TrangThai.HasValue)
            {
                query = query.Where(c => c.TrangThai == request.TrangThai.Value);
            }

            var entities = await query
                .OrderBy(c => c.GioBatDau)
                .ToListAsync(cancellationToken);

            var list = entities
                .Select(c => new CaLamViecDto
                {
                    Id = c.Id,
                    TenCa = c.TenCa,
                    GioBatDau = c.GioBatDau.ToString(@"hh\:mm\:ss"),
                    GioKetThuc = c.GioKetThuc.ToString(@"hh\:mm\:ss"),
                    XuyenNgay = c.XuyenNgay,
                    HeSoLuong = c.HeSoLuong,
                    TrangThai = c.TrangThai,
                    KhungGioNghis = c.KhungGioNghis.Where(k => !k.IsDeleted).Select(k => new KhungGioNghiDto
                    {
                        Id = k.Id,
                        IdCaLamViec = k.IdCaLamViec,
                        TenKhoangNghi = k.TenKhoangNghi,
                        GioBatDau = k.GioBatDau.ToString(@"hh\:mm\:ss"),
                        GioKetThuc = k.GioKetThuc.ToString(@"hh\:mm\:ss"),
                        TinhVaoGioLam = k.TinhVaoGioLam
                    }).OrderBy(k => k.GioBatDau).ToList()
                })
                .ToList();

            return new Response<List<CaLamViecDto>>(list);
        }
    }
}
