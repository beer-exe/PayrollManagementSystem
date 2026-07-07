using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum TrangThaiQuyetDinh
    {
        [Description("Hiệu lực")]
        HIEU_LUC,
        
        [Description("Hết hạn")]
        HET_HAN,
        
        [Description("Hủy bỏ")]
        HUY_BO
    }
}
