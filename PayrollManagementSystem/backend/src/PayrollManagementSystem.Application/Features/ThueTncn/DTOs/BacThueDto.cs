using System;

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
}
