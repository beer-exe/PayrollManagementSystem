using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollManagementSystem.Domain.Models
{
    public class VaiTro
    {
        public Guid IdVaiTro { get; set; }
        public string TenVaiTro { get; set; } = null!;

        // Navigation properties
        public ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
    }
}
