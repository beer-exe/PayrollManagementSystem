using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum TrangThaiPhieuKpi
    {
        [Description("Chờ giao mục tiêu")]
        CHO_GIAO_MUC_TIEU = 0,
        
        [Description("Đang thực hiện")]
        DANG_THUC_HIEN = 1,
        
        [Description("Chờ phê duyệt")]
        CHO_PHE_DUYET = 2,
        
        [Description("Đã phê duyệt")]
        DA_PHE_DUYET = 3
    }
}
