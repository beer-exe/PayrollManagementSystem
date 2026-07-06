using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.Employees.Commands.ChangeEmployeeStatus
{
    public class ChangeEmployeeStatusCommandHandler : IRequestHandler<ChangeEmployeeStatusCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public ChangeEmployeeStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(ChangeEmployeeStatusCommand request, CancellationToken cancellationToken)
        {
            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(nv => nv.Cccd == request.Cccd, cancellationToken);

            if (nhanVien == null)
                throw new ApiException($"Không tìm thấy nhân viên với CCCD '{request.Cccd}'.");

            if (nhanVien.TrangThai == request.TrangThaiMoi)
                return new Response<bool>(true, "Trạng thái mới giống với trạng thái hiện tại, không có thay đổi.");

            var nhatKy = new NhatKyTrangThai
            {
                Cccd = nhanVien.Cccd,
                TrangThaiCu = nhanVien.TrangThai,
                TrangThaiMoi = request.TrangThaiMoi,
                LyDo = request.LyDo,
                NgayThayDoi = DateTime.Now,
                NguoiThayDoi = request.NguoiThayDoi
            };

            nhanVien.TrangThai = request.TrangThaiMoi;

            if (request.TrangThaiMoi == TrangThaiNhanVien.DA_NGHI_VIEC)
            {
                nhanVien.NgayNghiViec = DateOnly.FromDateTime(DateTime.Now);
            }
            else if (request.TrangThaiMoi == TrangThaiNhanVien.DANG_LAM_VIEC)
            {
                nhanVien.NgayNghiViec = null;
            }

            _context.NhatKyTrangThais.Add(nhatKy);

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Cập nhật trạng thái nhân viên thành công.");
        }
    }
}