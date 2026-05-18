namespace PayrollManagementSystem.Domain.Models
{
    public class ThanNhanNhanVien
    {
        public string Cccd { get; set; } = null!;
        public string MaDinhDanh { get; set; } = null!;
        public Guid? IdMqh { get; set; }

        // Navigation properties
        public NhanVien NhanVien { get; set; } = null!;
        public MoiQuanHe? MoiQuanHe { get; set; }
        public ThanNhan ThanNhan { get; set; } = null!;
    }
}
