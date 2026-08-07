namespace PayrollManagementSystem.Application.Features.WorkShifts.Commands.CreateCaLamViec
{
    public class CreateKhungGioNghiCommand
    {
        public string TenKhoangNghi { get; set; } = null!;
        public string GioBatDau { get; set; } = null!;
        public string GioKetThuc { get; set; } = null!;
        public bool TinhVaoGioLam { get; set; }
    }
}
