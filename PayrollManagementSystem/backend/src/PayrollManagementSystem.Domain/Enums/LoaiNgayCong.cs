using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum LoaiNgayCong
    {
        [Description("Làm đủ ca")]
        LAM_DU_CA = 0,

        [Description("Nửa ca")]
        NUA_CA = 1,

        [Description("Đi trễ / Về sớm")]
        DI_TRE_VE_SOM = 2,

        [Description("Vắng có phép")]
        VANG_CO_PHEP = 3,

        [Description("Vắng không phép")]
        VANG_KHONG_PHEP = 4,

        [Description("Nghỉ lễ")]
        NGHI_LE = 5,

        [Description("Nghỉ cuối tuần")]
        NGHI_CUOI_TUAN = 6,

        [Description("Vắng có phép (Không lương)")]
        VANG_CO_PHEP_KHONG_LUONG = 7,
    }
}
