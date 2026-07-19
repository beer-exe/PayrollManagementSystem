using System.ComponentModel;

namespace PayrollManagementSystem.Domain.Enums
{
    public enum LoaiNghi
    {
        [Description("Nghỉ phép năm")] NGHI_PHEP_NAM,
        [Description("Nghỉ không lương")] NGHI_KHONG_LUONG,
        [Description("Nghỉ ốm đau")] NGHI_OM_DAU,
        [Description("Nghỉ thai sản")] NGHI_THAI_SAN,
        [Description("Nghỉ theo chế độ")] NGHI_CHE_DO,
    }
}
