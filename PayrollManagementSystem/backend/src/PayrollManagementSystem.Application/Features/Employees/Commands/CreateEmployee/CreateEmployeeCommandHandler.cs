using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Response<string>>
    {
        private readonly IApplicationDbContext _context;

        public CreateEmployeeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<string>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (await _context.NhanViens.AnyAsync(x => x.Cccd == request.Cccd, cancellationToken))
                throw new ApiException($"Nhân viên với CCCD '{request.Cccd}' đã tồn tại trong hệ thống.");

            if (await _context.HopDongLaoDongs.AnyAsync(x => x.SoHopDong == request.SoHopDong, cancellationToken))
                throw new ApiException($"Số hợp đồng '{request.SoHopDong}' đã tồn tại.");

            if (await _context.QuyetDinhNhanSus.AnyAsync(x => x.SoQuyetDinh == request.SoQuyetDinh, cancellationToken))
                throw new ApiException($"Số quyết định '{request.SoQuyetDinh}' đã tồn tại.");

            if (!await _context.PhongBans.AnyAsync(pb => pb.IdPb == request.IdPb, cancellationToken))
                throw new ApiException($"Phòng ban với mã '{request.IdPb}' không tồn tại.");

            if (!await _context.ChucVus.AnyAsync(cv => cv.IdChucVu == request.IdChucVu, cancellationToken))
                throw new ApiException($"Chức vụ với mã '{request.IdChucVu}' không tồn tại.");

            if (!string.IsNullOrEmpty(request.IdBacLuong) &&
                !await _context.BacLuongs.AnyAsync(bl => bl.IdBacLuong == request.IdBacLuong, cancellationToken))
                throw new ApiException($"Bậc lương với mã '{request.IdBacLuong}' không tồn tại.");

            var nhanVien = new NhanVien
            {
                Cccd = request.Cccd,
                HoTen = request.HoTen,
                GioiTinh = request.GioiTinh,
                Sdt = request.Sdt,
                Email = request.Email,
                NgaySinh = request.NgaySinh,
                DiaChi = request.DiaChi,
                DanToc = request.DanToc,
                ChuyenNganh = request.ChuyenNganh,
                SoBhxh = request.SoBhxh,
                SoBhyt = request.SoBhyt,
                SoTaiKhoan = request.SoTaiKhoan,
                TenNganHang = request.TenNganHang,
                MaSoThue = request.MaSoThue,
                IdPb = request.IdPb,
                NgayVaoLam = request.NgayBatDauHopDong,
                TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC
            };

            var hopDong = new HopDongLaoDong
            {
                SoHopDong = request.SoHopDong,
                Cccd = request.Cccd,
                LoaiHopDong = request.LoaiHopDong,
                NgayBatDau = request.NgayBatDauHopDong,
                NgayKetThuc = request.NgayKetThucHopDong,
                LuongCoBan = request.LuongCoBan,
                TrangThai = TrangThaiHopDong.HIEU_LUC
            };

            var quyetDinh = new QuyetDinhNhanSu
            {
                SoQuyetDinh = request.SoQuyetDinh,
                Cccd = request.Cccd,
                LoaiQuyetDinh = "Tuyển dụng",
                IdChucVuMoi = request.IdChucVu,
                IdBacLuongMoi = request.IdBacLuong,
                NgayHieuLuc = request.NgayBatDauHopDong,
                NguoiKy = request.NguoiKyQuyetDinh,
                TrangThai = TrangThaiQuyetDinh.HIEU_LUC
            };

            _context.NhanViens.Add(nhanVien);
            _context.HopDongLaoDongs.Add(hopDong);
            _context.QuyetDinhNhanSus.Add(quyetDinh);

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<string>(nhanVien.Cccd, "Thêm mới nhân viên, tạo hợp đồng và phân bổ vị trí thành công.");
        }
    }
}