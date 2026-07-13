namespace PayrollManagementSystem.Application.Features.WorkSchedule.Services
{
    /// <summary>
    /// Service tính toán ngày lễ Việt Nam theo Bộ Luật Lao Động (Điều 112).
    /// Hỗ trợ chuyển đổi Âm - Dương lịch để tính Tết Nguyên Đán và Giỗ Tổ Hùng Vương.
    /// </summary>
    public static class VietnamHolidayService
    {
        /// <summary>
        /// Trả về danh sách ngày lễ trong năm dương lịch cho trước (key=DateOnly, value=tên lễ).
        /// </summary>
        public static Dictionary<DateOnly, string> GetHolidays(int year)
        {
            var holidays = new Dictionary<DateOnly, string>();

            // 1. Tết Dương Lịch: 01/01
            holidays[new DateOnly(year, 1, 1)] = "Tết Dương lịch";

            // 2. Lễ Giải Phóng Miền Nam: 30/04
            holidays[new DateOnly(year, 4, 30)] = "Lễ Giải phóng miền Nam";

            // 3. Quốc Tế Lao Động: 01/05
            holidays[new DateOnly(year, 5, 1)] = "Quốc tế Lao động";

            // 4. Quốc Khánh: 02/09
            holidays[new DateOnly(year, 9, 2)] = "Quốc khánh";
            // Theo luật: nếu ngày lễ trùng T7/CN thì được nghỉ bù
            var quocKhanh = new DateOnly(year, 9, 2);
            if (quocKhanh.DayOfWeek == DayOfWeek.Saturday)
                holidays[new DateOnly(year, 9, 4)] = "Quốc khánh (nghỉ bù)";
            else if (quocKhanh.DayOfWeek == DayOfWeek.Sunday)
                holidays[new DateOnly(year, 9, 3)] = "Quốc khánh (nghỉ bù)";

            // 5. Tết Nguyên Đán: 5 ngày (29/12 ÂL đến 03/01 ÂL năm sau)
            AddTetNguyenDan(year, holidays);

            // 6. Giỗ Tổ Hùng Vương: 10/03 Âm lịch
            AddGioToHungVuong(year, holidays);

            return holidays;
        }

        private static void AddTetNguyenDan(int year, Dictionary<DateOnly, string> holidays)
        {
            // Ngày mùng 1 Tết = ngày 1 tháng 1 Âm lịch
            var mong1Tet = LunarToSolar(year, 1, 1);
            if (mong1Tet.Year != year)
                mong1Tet = LunarToSolar(year + 1, 1, 1); // phòng trường hợp Tết rơi sang năm sau

            // 5 ngày nghỉ: 30/12 AL đến 4/1 AL (tức -1 đến +3 so với mùng 1)
            var tetDays = new (int Offset, string Name)[]
            {
                (-1, "Tết Nguyên Đán (30 tháng Chạp)"),
                (0, "Tết Nguyên Đán (Mùng 1)"),
                (1, "Tết Nguyên Đán (Mùng 2)"),
                (2, "Tết Nguyên Đán (Mùng 3)"),
                (3, "Tết Nguyên Đán (Mùng 4)"),
            };

            foreach (var (offset, name) in tetDays)
            {
                var d = DateOnly.FromDateTime(mong1Tet.ToDateTime(TimeOnly.MinValue).AddDays(offset));
                if (d.Year == year)
                    holidays.TryAdd(d, name);
            }
        }

        private static void AddGioToHungVuong(int year, Dictionary<DateOnly, string> holidays)
        {
            var gioTo = LunarToSolar(year, 3, 10);
            if (gioTo.Year == year)
                holidays.TryAdd(gioTo, "Giỗ Tổ Hùng Vương (10/3 Âm lịch)");
        }

        /// <summary>
        /// Chuyển đổi ngày Âm lịch → Dương lịch theo thuật toán Jean Meeus / Hồng Đức.
        /// Thuật toán dựa trên tính toán múi giờ UTC+7.
        /// </summary>
        public static DateOnly LunarToSolar(int lunarYear, int lunarMonth, int lunarDay)
        {
            // Tính Julian Day Number của ngày mùng 1 tháng lunarMonth âm lịch
            double jd = GetNewMoonJD(lunarYear, lunarMonth);
            double jdOfDay = jd + lunarDay - 1;
            return JulianToDateOnly(jdOfDay);
        }

        /// <summary>
        /// Trả về Julian Day Number của ngày mùng 1 tháng âm lịch.
        /// Dùng múi giờ UTC+7 (Việt Nam).
        /// </summary>
        private static double GetNewMoonJD(int year, int month)
        {
            // Số tháng synodic kể từ epoch (01/01/1900 dương lịch, JD = 2415020.5)
            double k = Math.Floor((year - 1900 + (month - 1) / 12.0) * 12.3685);
            return NewMoonJD(k);
        }

        private static double NewMoonJD(double k)
        {
            double T = k / 1236.85;
            double T2 = T * T;
            double T3 = T2 * T;
            double jd = 2415020.75933 + 29.53058868 * k
                         + 0.0001178 * T2 - 0.000000155 * T3
                         + 0.00033 * Math.Sin((166.56 + 132.87 * T - 0.009173 * T2) * Math.PI / 180);

            double M = 359.2242 + 29.10535608 * k - 0.0000333 * T2 - 0.00000347 * T3;
            double MPrime = 306.0253 + 385.81691806 * k + 0.0107306 * T2 + 0.00001236 * T3;
            double F = 21.2964 + 390.67050646 * k - 0.0016528 * T2 - 0.00000239 * T3;

            M *= Math.PI / 180;
            MPrime *= Math.PI / 180;
            F *= Math.PI / 180;

            double correction = (0.1734 - 0.000393 * T) * Math.Sin(M)
                + 0.0021 * Math.Sin(2 * M)
                - 0.4068 * Math.Sin(MPrime)
                + 0.0161 * Math.Sin(2 * MPrime)
                - 0.0004 * Math.Sin(3 * MPrime)
                + 0.0104 * Math.Sin(2 * F)
                - 0.0051 * Math.Sin(M + MPrime)
                - 0.0074 * Math.Sin(M - MPrime)
                + 0.0004 * Math.Sin(2 * F + M)
                - 0.0004 * Math.Sin(2 * F - M)
                - 0.0006 * Math.Sin(2 * F + MPrime)
                + 0.0010 * Math.Sin(2 * F - MPrime)
                + 0.0005 * Math.Sin(M + 2 * MPrime);

            // Offset múi giờ +7
            return jd + correction + 7.0 / 24.0;
        }

        private static DateOnly JulianToDateOnly(double jd)
        {
            int z = (int)Math.Floor(jd + 0.5);
            int a = z < 2299161 ? z : (int)(z + 1 + Math.Floor((Math.Floor((z - 1867216.25) / 36524.25)) - Math.Floor(Math.Floor((z - 1867216.25) / 36524.25) / 4)));
            int b = a + 1524;
            int c = (int)Math.Floor((b - 122.1) / 365.25);
            int d = (int)Math.Floor(365.25 * c);
            int e = (int)Math.Floor((b - d) / 30.6001);

            int day = b - d - (int)Math.Floor(30.6001 * e);
            int month = e < 14 ? e - 1 : e - 13;
            int year = month > 2 ? c - 4716 : c - 4715;

            return new DateOnly(year, month, day);
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
