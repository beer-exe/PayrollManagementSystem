using System;
using System.Collections.Generic;

namespace PayrollManagementSystem.Application.Features.ChamCong.DTOs
{
    public class ImportChamCongResultDto
    {
        public int TongSoDong { get; set; }
        public int ThanhCong { get; set; }
        public int ThatBai { get; set; }
        public List<string> LoiNhap { get; set; } = new();
    }
}
