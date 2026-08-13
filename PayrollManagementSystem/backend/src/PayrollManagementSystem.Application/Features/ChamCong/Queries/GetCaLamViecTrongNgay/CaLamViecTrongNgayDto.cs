using System;

namespace PayrollManagementSystem.Application.Features.ChamCong.Queries.GetCaLamViecTrongNgay
{
    public class CaLamViecTrongNgayDto
    {
        public TimeOnly? GioVao { get; set; }
        public TimeOnly? GioRa { get; set; }
        public bool IsDayOff { get; set; }
        public string Source { get; set; } = string.Empty;
    }
}
