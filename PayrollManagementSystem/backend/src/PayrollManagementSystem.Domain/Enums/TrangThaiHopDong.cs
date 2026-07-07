using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum TrangThaiHopDong
    {
        [Description("Hiệu lực")]
        HIEU_LUC,
        
        [Description("Hết hạn")]
        HET_HAN,
        
        [Description("Chấm dứt")]
        CHAM_DUT,
        
        [Description("Chờ duyệt")]
        CHO_DUYET
    }
}