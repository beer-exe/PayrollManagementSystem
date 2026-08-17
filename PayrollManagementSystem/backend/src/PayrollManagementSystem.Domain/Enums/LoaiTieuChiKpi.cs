using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum LoaiTieuChiKpi
    {
        [Description("Càng nhiều càng tốt")]
        CANG_NHIEU_CANG_TOT = 0,
        
        [Description("Càng ít càng tốt")]
        CANG_IT_CANG_TOT = 1
    }
}
