using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateEmployeeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(nv => nv.Cccd == request.Cccd, cancellationToken);

            if (nhanVien == null)
                throw new KeyNotFoundException($"Không tìm thấy thông tin hồ sơ nhân viên với số CCCD '{request.Cccd}'.");

            if (!string.IsNullOrEmpty(request.Email))
            {
                var emailExists = await _context.NhanViens
                    .AnyAsync(nv => nv.Email == request.Email && nv.Cccd != request.Cccd, cancellationToken);
                if (emailExists)
                    throw new ApiException("Email này đã được sử dụng bởi một nhân viên khác.");
            }

            if (!string.IsNullOrEmpty(request.IdPb))
            {
                var deptExists = await _context.PhongBans.AnyAsync(pb => pb.IdPb == request.IdPb, cancellationToken);
                if (!deptExists)
                    throw new ApiException("Phòng ban được chọn không tồn tại trong hệ thống.");

                nhanVien.IdPb = request.IdPb;
            }

            nhanVien.HoTen = request.HoTen.Trim();
            nhanVien.GioiTinh = request.GioiTinh;
            nhanVien.Sdt = request.Sdt?.Trim();
            nhanVien.Email = request.Email?.Trim();
            nhanVien.NgaySinh = request.NgaySinh;
            nhanVien.DanToc = request.DanToc?.Trim();
            nhanVien.DiaChi = request.DiaChi?.Trim();
            nhanVien.ChuyenNganh = request.ChuyenNganh?.Trim();
            nhanVien.SoBhxh = request.SoBhxh?.Trim();
            nhanVien.SoBhyt = request.SoBhyt?.Trim();
            nhanVien.SoTaiKhoan = request.SoTaiKhoan?.Trim();
            nhanVien.TenNganHang = request.TenNganHang?.Trim();
            nhanVien.MaSoThue = request.MaSoThue?.Trim();

            // Xử lý Thân nhân
            if (request.ThanNhans != null)
            {
                var currentRelations = await _context.TNhanNviens
                    .Include(tn => tn.ThanNhan)
                    .Where(tn => tn.Cccd == request.Cccd)
                    .ToListAsync(cancellationToken);

                // Các thân nhân cần giữ lại (có mã định danh)
                var requestMaDinhDanhs = request.ThanNhans
                    .Where(t => !string.IsNullOrEmpty(t.MaDinhDanh))
                    .Select(t => t.MaDinhDanh)
                    .ToList();

                // 1. Xóa các thân nhân không còn nằm trong danh sách gửi lên
                var relationsToRemove = currentRelations
                    .Where(tn => !requestMaDinhDanhs.Contains(tn.MaDinhDanh))
                    .ToList();

                if (relationsToRemove.Any())
                {
                    _context.SoftRemoveRange(relationsToRemove);
                    var thanNhansToRemove = relationsToRemove.Select(r => r.ThanNhan).ToList();
                    _context.SoftRemoveRange(thanNhansToRemove);
                }

                // 2. Cập nhật và thêm mới
                foreach (var reqTn in request.ThanNhans)
                {
                    if (!string.IsNullOrEmpty(reqTn.MaDinhDanh))
                    {
                        // Cập nhật
                        var relation = currentRelations.FirstOrDefault(r => r.MaDinhDanh == reqTn.MaDinhDanh);
                        if (relation != null)
                        {
                            relation.IdMqh = reqTn.IdMqh;
                            relation.LaNguoiPhuThuoc = reqTn.LaNguoiPhuThuoc;
                            relation.ThanNhan.TenTn = reqTn.TenTn;
                            relation.ThanNhan.NgaySinh = reqTn.NgaySinh;
                        }
                    }
                    else
                    {
                        // Thêm mới
                        var newMaDinhDanh = Guid.NewGuid().ToString();
                        var newThanNhan = new PayrollManagementSystem.Domain.Models.ThanNhan
                        {
                            MaDinhDanh = newMaDinhDanh,
                            TenTn = reqTn.TenTn,
                            NgaySinh = reqTn.NgaySinh
                        };
                        _context.ThanNhans.Add(newThanNhan);

                        var newRelation = new PayrollManagementSystem.Domain.Models.ThanNhanNhanVien
                        {
                            Cccd = request.Cccd,
                            MaDinhDanh = newMaDinhDanh,
                            IdMqh = reqTn.IdMqh,
                            LaNguoiPhuThuoc = reqTn.LaNguoiPhuThuoc
                        };
                        _context.TNhanNviens.Add(newRelation);
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Cập nhật hồ sơ thông tin nhân viên thành công.");
        }
    }
}
