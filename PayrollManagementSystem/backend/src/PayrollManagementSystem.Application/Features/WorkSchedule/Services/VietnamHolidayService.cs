using System.Globalization;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Services
{
    public static class VietnamHolidayService
    {
        public static Dictionary<DateOnly, string> GetHolidays(int year)
        {
            var holidays = new Dictionary<DateOnly, string>();

            holidays[new DateOnly(year, 1, 1)] = "Tết dương lịch";
            holidays[new DateOnly(year, 4, 30)] = "Ngày Giải phóng miền Nam";
            holidays[new DateOnly(year, 5, 1)] = "Quốc tế Lao động";

            holidays[new DateOnly(year, 9, 2)] = "Quốc Khánh";
            var quocKhanh = new DateOnly(year, 9, 2);
            if (quocKhanh.DayOfWeek == DayOfWeek.Saturday)
                holidays[new DateOnly(year, 9, 4)] = "Quốc Khánh (nghỉ bù)";
            else if (quocKhanh.DayOfWeek == DayOfWeek.Sunday)
                holidays[new DateOnly(year, 9, 3)] = "Quốc Khánh (nghỉ bù)";

            var cal = new ChineseLunisolarCalendar();

            DateTime m1Tet = default;
            for (var d = new DateTime(year, 1, 15); d <= new DateTime(year, 2, 28); d = d.AddDays(1))
            {
                var lMonth = cal.GetMonth(d);
                var lDay = cal.GetDayOfMonth(d);
                if (lMonth == 1 && lDay == 1 && !cal.IsLeapMonth(cal.GetYear(d), lMonth))
                {
                    m1Tet = d;
                    break;
                }
            }

            if (m1Tet != default)
            {
                var tetDays = new (int Offset, string Name)[]
                {
                    (-1, "Tết Nguyên Đán (30)"),
                    (0, "Tết Nguyên Đán (Mùng 1)"),
                    (1, "Tết Nguyên Đán (Mùng 2)"),
                    (2, "Tết Nguyên Đán (Mùng 3)"),
                    (3, "Tết Nguyên Đán (Mùng 4)"),
                };
                foreach (var (offset, name) in tetDays)
                {
                    var d = m1Tet.AddDays(offset);
                    if (d.Year == year || d.Year == year - 1)
                    {
                        var dOnly = DateOnly.FromDateTime(d);
                        holidays.TryAdd(dOnly, name);
                    }
                }
            }

            for (var d = new DateTime(year, 3, 15); d <= new DateTime(year, 5, 15); d = d.AddDays(1))
            {
                var lMonth = cal.GetMonth(d);
                var lDay = cal.GetDayOfMonth(d);
                if (lMonth == 3 && lDay == 10 && !cal.IsLeapMonth(cal.GetYear(d), lMonth))
                {
                    holidays.TryAdd(DateOnly.FromDateTime(d), "Giỗ Tổ Hùng Vương (10/3 Âm Lịch)");
                    break;
                }
            }

            return holidays;
        }

        public static string GetDayOfWeekVietnamese(DayOfWeek dow) => dow switch
        {
            DayOfWeek.Monday => "Thứ Hai",
            DayOfWeek.Tuesday => "Thứ Ba",
            DayOfWeek.Wednesday => "Thứ Tư",
            DayOfWeek.Thursday => "Thứ Năm",
            DayOfWeek.Friday => "Thứ Sáu",
            DayOfWeek.Saturday => "Thứ Bảy",
            DayOfWeek.Sunday => "Chủ Nhật",
            _ => ""
        };
    }
}
