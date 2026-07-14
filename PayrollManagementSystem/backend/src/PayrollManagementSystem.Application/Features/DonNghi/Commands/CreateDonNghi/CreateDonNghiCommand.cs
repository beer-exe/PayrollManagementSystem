using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.DonNghi.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.CreateDonNghi
{
    public class CreateDonNghiCommand : IRequest<Response<Guid>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string CccdNhanVien { get; set; } = null!;
        public string LoaiNghi { get; set; } = null!;       // enum name string
        public DateOnly NgayBatDau { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        public decimal SoNgayNghi { get; set; }
        public string LyDo { get; set; } = null!;
        public string? TaiLieuDinhKem { get; set; }

        public string CacheKeyPrefix => "DonNghi";
    }

    public class CreateDonNghiCommandHandler : IRequestHandler<CreateDonNghiCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;
        public CreateDonNghiCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<Guid>> Handle(CreateDonNghiCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<LoaiNghi>(request.LoaiNghi, out var loaiNghi))
                throw new ApiException($"Loại nghỉ không hợp lệ: {request.LoaiNghi}");

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(nv => nv.Cccd == request.CccdNhanVien && !nv.IsDeleted, cancellationToken);
            if (nhanVien == null)
                throw new ApiException("Nhân viên không tồn tại.");

            var donNghi = new Domain.Models.DonNghi
            {
                CccdNhanVien = request.CccdNhanVien,
                LoaiNghi = loaiNghi,
                NgayBatDau = request.NgayBatDau,
                NgayKetThuc = request.NgayKetThuc,
                SoNgayNghi = request.SoNgayNghi,
                LyDo = request.LyDo,
                TaiLieuDinhKem = request.TaiLieuDinhKem,
                TrangThai = TrangThaiDonNghi.CHO_DUYET,
            };

            await _context.DonNghis.AddAsync(donNghi, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(donNghi.Id, "Tạo đơn nghỉ thành công.");
        }
    }
}
