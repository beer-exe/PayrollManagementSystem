namespace PayrollManagementSystem.Application.Features.WorkShifts.DTOs
{
    public class KhungGioNghiDto
    {
        public Guid Id { get; set; }
        public Guid? IdCaLamViec { get; set; }
        public string TenKhoangNghi { get; set; } = null!;
        public string GioBatDau { get; set; } = null!;
        public string GioKetThuc { get; set; } = null!;
        public bool TinhVaoGioLam { get; set; }
    }
}
