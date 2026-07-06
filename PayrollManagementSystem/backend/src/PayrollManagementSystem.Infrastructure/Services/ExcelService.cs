using ClosedXML.Excel;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Employees.DTOs;

namespace PayrollManagementSystem.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        public byte[] ExportEmployeesToExcel(IEnumerable<EmployeeDto> employees)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách nhân viên");
            var currentRow = 1;

            // Header
            worksheet.Cell(currentRow, 1).Value = "Mã NV (CCCD)";
            worksheet.Cell(currentRow, 2).Value = "Họ tên";
            worksheet.Cell(currentRow, 3).Value = "Giới tính";
            worksheet.Cell(currentRow, 4).Value = "Ngày sinh";
            worksheet.Cell(currentRow, 5).Value = "Số điện thoại";
            worksheet.Cell(currentRow, 6).Value = "Email";
            worksheet.Cell(currentRow, 7).Value = "Phòng ban";
            worksheet.Cell(currentRow, 8).Value = "Chức vụ";
            worksheet.Cell(currentRow, 9).Value = "Ngày vào làm";
            worksheet.Cell(currentRow, 10).Value = "Lương P1";
            worksheet.Cell(currentRow, 11).Value = "Hệ số P2";
            worksheet.Cell(currentRow, 12).Value = "Số tài khoản";
            worksheet.Cell(currentRow, 13).Value = "Ngân hàng";
            worksheet.Cell(currentRow, 14).Value = "Mã số thuế";
            worksheet.Cell(currentRow, 15).Value = "Trạng thái";

            // Format Header
            var headerRange = worksheet.Range(1, 1, 1, 15);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

            // Data
            foreach (var emp in employees)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = emp.Cccd;
                worksheet.Cell(currentRow, 2).Value = emp.HoTen;
                worksheet.Cell(currentRow, 3).Value = emp.GioiTinh == true ? "Nam" : (emp.GioiTinh == false ? "Nữ" : "");
                worksheet.Cell(currentRow, 4).Value = emp.NgaySinh;
                worksheet.Cell(currentRow, 5).Value = emp.Sdt;
                worksheet.Cell(currentRow, 6).Value = emp.Email;
                worksheet.Cell(currentRow, 7).Value = emp.TenPhongBan;
                worksheet.Cell(currentRow, 8).Value = emp.TenChucVu;
                worksheet.Cell(currentRow, 9).Value = emp.NgayVaoLam;
                worksheet.Cell(currentRow, 10).Value = emp.LuongP1;
                worksheet.Cell(currentRow, 11).Value = emp.HeSoP2;
                worksheet.Cell(currentRow, 12).Value = emp.SoTaiKhoan;
                worksheet.Cell(currentRow, 13).Value = emp.TenNganHang;
                worksheet.Cell(currentRow, 14).Value = emp.MaSoThue;
                worksheet.Cell(currentRow, 15).Value = emp.TrangThai;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
