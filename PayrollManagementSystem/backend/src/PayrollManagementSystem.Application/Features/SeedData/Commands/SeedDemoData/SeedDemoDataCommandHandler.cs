using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.SeedData.Commands.SeedDemoData
{
    public class SeedDemoDataCommandHandler : IRequestHandler<SeedDemoDataCommand, Response<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public SeedDemoDataCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<Response<string>> Handle(SeedDemoDataCommand request, CancellationToken cancellationToken)
        {
            if (await _context.NhanViens.AnyAsync(cancellationToken))
            {
                return new Response<string>("Database đã có dữ liệu, không thể seed.");
            }

            var vaiTroAdmin = await _context.VaiTros.FirstOrDefaultAsync(v => v.TenVaiTro == "Admin", cancellationToken);
            var vaiTroHR = await _context.VaiTros.FirstOrDefaultAsync(v => v.TenVaiTro == "HR", cancellationToken);
            var vaiTroEmployee = await _context.VaiTros.FirstOrDefaultAsync(v => v.TenVaiTro == "Employee", cancellationToken);

            if (vaiTroAdmin == null || vaiTroHR == null || vaiTroEmployee == null)
            {
                return new Response<string>("Cần có các Role: Admin, HR, Employee trong database trước khi seed.");
            }

            // 1. Seed Ngach Luong
            var nlQuanLy = new NgachLuong { IdNgachLuong = Guid.NewGuid().ToString().Substring(0, 10).ToUpper(), TenNgachLuong = "Quản lý" };
            var nlChuyenVien = new NgachLuong { IdNgachLuong = Guid.NewGuid().ToString().Substring(0, 10).ToUpper(), TenNgachLuong = "Chuyên viên" };
            _context.NgachLuongs.AddRange(nlQuanLy, nlChuyenVien);

            // 2. Seed Bac Luong
            var date2023 = new DateOnly(2023, 1, 1);
            var blQuanLy1 = new BacLuong { IdBacLuong = Guid.NewGuid().ToString().Substring(0, 10).ToUpper(), IdNgachLuong = nlQuanLy.IdNgachLuong, TenBacLuong = "Bậc 1", LuongP1 = 20000000, NgayApDung = date2023 };
            var blQuanLy2 = new BacLuong { IdBacLuong = Guid.NewGuid().ToString().Substring(0, 10).ToUpper(), IdNgachLuong = nlQuanLy.IdNgachLuong, TenBacLuong = "Bậc 2", LuongP1 = 25000000, NgayApDung = date2023 };
            var blChuyenVien1 = new BacLuong { IdBacLuong = Guid.NewGuid().ToString().Substring(0, 10).ToUpper(), IdNgachLuong = nlChuyenVien.IdNgachLuong, TenBacLuong = "Bậc 1", LuongP1 = 10000000, NgayApDung = date2023 };
            var blChuyenVien2 = new BacLuong { IdBacLuong = Guid.NewGuid().ToString().Substring(0, 10).ToUpper(), IdNgachLuong = nlChuyenVien.IdNgachLuong, TenBacLuong = "Bậc 2", LuongP1 = 15000000, NgayApDung = date2023 };
            _context.BacLuongs.AddRange(blQuanLy1, blQuanLy2, blChuyenVien1, blChuyenVien2);

            // 3. Seed Phong Ban
            var pbBGD = new PhongBan { IdPb = "PB_BGD", TenPb = "Ban Giám Đốc" };
            var pbNS = new PhongBan { IdPb = "PB_NS", TenPb = "Phòng Nhân Sự" };
            var pbIT = new PhongBan { IdPb = "PB_IT", TenPb = "Phòng IT" };
            _context.PhongBans.AddRange(pbBGD, pbNS, pbIT);

            // 4. Seed Chuc Vu (Manager hierarchy)
            var cvCEO = new ChucVu { IdChucVu = "CV_CEO", TenChucVu = "Giám Đốc", IdPhongBan = pbBGD.IdPb, IdNgachLuong = nlQuanLy.IdNgachLuong, TrangThai = TrangThaiChucVu.HOAT_DONG, IdChucVuQuanLy = null };
            var cvHRM = new ChucVu { IdChucVu = "CV_HRM", TenChucVu = "Trưởng Phòng Nhân Sự", IdPhongBan = pbNS.IdPb, IdNgachLuong = nlQuanLy.IdNgachLuong, TrangThai = TrangThaiChucVu.HOAT_DONG, IdChucVuQuanLy = cvCEO.IdChucVu };
            var cvHRS = new ChucVu { IdChucVu = "CV_HRS", TenChucVu = "Chuyên Viên Nhân Sự", IdPhongBan = pbNS.IdPb, IdNgachLuong = nlChuyenVien.IdNgachLuong, TrangThai = TrangThaiChucVu.HOAT_DONG, IdChucVuQuanLy = cvHRM.IdChucVu };
            var cvITM = new ChucVu { IdChucVu = "CV_ITM", TenChucVu = "Trưởng Phòng IT", IdPhongBan = pbIT.IdPb, IdNgachLuong = nlQuanLy.IdNgachLuong, TrangThai = TrangThaiChucVu.HOAT_DONG, IdChucVuQuanLy = cvCEO.IdChucVu };
            var cvDEV = new ChucVu { IdChucVu = "CV_DEV", TenChucVu = "Lập Trình Viên", IdPhongBan = pbIT.IdPb, IdNgachLuong = nlChuyenVien.IdNgachLuong, TrangThai = TrangThaiChucVu.HOAT_DONG, IdChucVuQuanLy = cvITM.IdChucVu };
            _context.ChucVus.AddRange(cvCEO, cvHRM, cvHRS, cvITM, cvDEV);

            // 5. Seed Moi Quan He
            var mqhVo = new MoiQuanHe { IdMqh = Guid.NewGuid(), TenQuanHe = "Vợ" };
            var mqhChong = new MoiQuanHe { IdMqh = Guid.NewGuid(), TenQuanHe = "Chồng" };
            var mqhCon = new MoiQuanHe { IdMqh = Guid.NewGuid(), TenQuanHe = "Con" };
            var mqhCha = new MoiQuanHe { IdMqh = Guid.NewGuid(), TenQuanHe = "Cha" };
            var mqhMe = new MoiQuanHe { IdMqh = Guid.NewGuid(), TenQuanHe = "Mẹ" };
            _context.MoiQuanHes.AddRange(mqhVo, mqhChong, mqhCon, mqhCha, mqhMe);

            // 6. Seed Accounts & Employees
            var dummyPassword = "123abc@";
            
            var employeesData = new List<(string Email, string Name, string Cccd, string ChucVuId, string PhongBanId, string BacLuongId, Guid RoleId, string MqhName, string RelativeName, decimal Luong)>
            {
                ("admin@company.com", "Nguyễn Văn Admin", "001001001001", cvCEO.IdChucVu, pbBGD.IdPb, blQuanLy2.IdBacLuong, vaiTroAdmin.IdVaiTro, mqhVo.TenQuanHe, "Trần Thị Vợ CEO", blQuanLy2.LuongP1),
                ("hr_manager@company.com", "Trần Thị HR", "001001001002", cvHRM.IdChucVu, pbNS.IdPb, blQuanLy1.IdBacLuong, vaiTroHR.IdVaiTro, mqhChong.TenQuanHe, "Lê Văn Chồng HR", blQuanLy1.LuongP1),
                ("it_manager@company.com", "Lê Văn IT Manager", "001001001003", cvITM.IdChucVu, pbIT.IdPb, blQuanLy1.IdBacLuong, vaiTroEmployee.IdVaiTro, mqhCon.TenQuanHe, "Lê Bé Con IT", blQuanLy1.LuongP1),
                ("dev@company.com", "Phạm Văn Dev", "001001001004", cvDEV.IdChucVu, pbIT.IdPb, blChuyenVien1.IdBacLuong, vaiTroEmployee.IdVaiTro, mqhMe.TenQuanHe, "Đào Thị Mẹ Dev", blChuyenVien1.LuongP1)
            };

            var mqhDict = new Dictionary<string, Guid> {
                { "Vợ", mqhVo.IdMqh }, { "Chồng", mqhChong.IdMqh }, { "Con", mqhCon.IdMqh }, { "Cha", mqhCha.IdMqh }, { "Mẹ", mqhMe.IdMqh }
            };

            foreach (var empData in employeesData)
            {
                var tk = new TaiKhoan
                {
                    IdTaiKhoan = Guid.NewGuid(),
                    TenTaiKhoan = empData.Email,
                    MatKhauHash = "",
                    IdVaiTro = empData.RoleId,
                    TrangThai = TrangThaiTaiKhoan.HOAT_DONG
                };
                tk.MatKhauHash = _passwordHasher.HashPasswordEnhanced(dummyPassword);
                _context.TaiKhoans.Add(tk);

                var nv = new NhanVien
                {
                    Cccd = empData.Cccd,
                    HoTen = empData.Name,
                    Email = empData.Email,
                    Sdt = "0900000000",
                    GioiTinh = true,
                    NgaySinh = new DateOnly(1990, 1, 1),
                    DiaChi = "TP. Hồ Chí Minh",
                    DanToc = "Kinh",
                    ChuyenNganh = "Công nghệ thông tin",
                    NgayVaoLam = new DateOnly(2023, 1, 1),
                    TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC,
                    IdPb = empData.PhongBanId,
                    IdTaiKhoan = tk.IdTaiKhoan
                };
                _context.NhanViens.Add(nv);

                var qd = new QuyetDinhNhanSu
                {
                    SoQuyetDinh = "QD_" + empData.Cccd,
                    Cccd = nv.Cccd,
                    LoaiQuyetDinh = "Bổ nhiệm",
                    IdChucVuMoi = empData.ChucVuId,
                    IdBacLuongMoi = empData.BacLuongId,
                    NgayHieuLuc = new DateOnly(2023, 1, 1),
                    TrangThai = TrangThaiQuyetDinh.HIEU_LUC
                };
                _context.QuyetDinhNhanSus.Add(qd);

                var tn = new ThanNhan
                {
                    MaDinhDanh = "TN_" + empData.Cccd,
                    TenTn = empData.RelativeName,
                    NgaySinh = new DateOnly(1995, 1, 1)
                };
                _context.ThanNhans.Add(tn);

                var tnnv = new ThanNhanNhanVien
                {
                    Cccd = nv.Cccd,
                    MaDinhDanh = tn.MaDinhDanh,
                    IdMqh = mqhDict[empData.MqhName]
                };
                _context.TNhanNviens.Add(tnnv);

                var hd = new HopDongLaoDong
                {
                    SoHopDong = "HD_" + empData.Cccd,
                    Cccd = nv.Cccd,
                    LoaiHopDong = "Hợp đồng vô thời hạn",
                    NgayBatDau = new DateOnly(2023, 1, 1),
                    LuongCoBan = empData.Luong,
                    TrangThai = TrangThaiHopDong.HIEU_LUC
                };
                _context.HopDongLaoDongs.Add(hd);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<string>("Seed data thành công.", "Tạo dữ liệu mẫu thành công.");
        }
    }
}
