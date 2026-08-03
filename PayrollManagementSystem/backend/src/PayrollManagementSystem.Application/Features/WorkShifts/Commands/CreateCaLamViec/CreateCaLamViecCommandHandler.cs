using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.WorkShifts.Commands.CreateCaLamViec
{
    public class CreateCaLamViecCommandHandler : IRequestHandler<CreateCaLamViecCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;

        public CreateCaLamViecCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Guid>> Handle(CreateCaLamViecCommand request, CancellationToken cancellationToken)
        {
            var caLamViec = new CaLamViec
            {
                TenCa = request.TenCa,
                GioBatDau = TimeSpan.Parse(request.GioBatDau),
                GioKetThuc = TimeSpan.Parse(request.GioKetThuc),
                XuyenNgay = request.XuyenNgay,
                HeSoLuong = request.HeSoLuong,
                TrangThai = request.TrangThai
            };

            if (request.KhungGioNghis != null && request.KhungGioNghis.Any())
            {
                foreach (var kgn in request.KhungGioNghis)
                {
                    caLamViec.KhungGioNghis.Add(new KhungGioNghi
                    {
                        TenKhoangNghi = kgn.TenKhoangNghi,
                        GioBatDau = TimeSpan.Parse(kgn.GioBatDau),
                        GioKetThuc = TimeSpan.Parse(kgn.GioKetThuc),
                        TinhVaoGioLam = kgn.TinhVaoGioLam
                    });
                }
            }

            _context.CaLamViecs.Add(caLamViec);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(caLamViec.Id, "Tạo ca làm việc thành công.");
        }
    }
}
