namespace PayrollManagementSystem.Application.Features.ThueTncn.DTOs
{
    public class BacThueDto
    {
        public Guid IdBacThue { get; set; }
        public int Bac { get; set; }
        public decimal TuGia { get; set; }
        public decimal? DenGia { get; set; }
        public decimal ThueSuat { get; set; }
        public bool IsActive { get; set; }
    }

    public class CauHinhGiamTruDto
    {
        public Guid? IdCauHinhGiamTru { get; set; }
        public decimal GiamTruBanThan { get; set; }
        public decimal GiamTruNguoiPhuThuoc { get; set; }
        public string? GhiChu { get; set; }
    }
}
