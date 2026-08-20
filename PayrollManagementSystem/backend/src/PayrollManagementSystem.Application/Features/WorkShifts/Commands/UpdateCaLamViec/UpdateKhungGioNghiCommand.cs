namespace PayrollManagementSystem.Application.Features.WorkShifts.Commands.UpdateCaLamViec
{
    public class UpdateKhungGioNghiCommand
    {
        public Guid? Id { get; set; }
        public string TenKhoangNghi { get; set; } = null!;
        public string GioBatDau { get; set; } = null!;
        public string GioKetThuc { get; set; } = null!;
        public bool TinhVaoGioLam { get; set; }
    }
}
