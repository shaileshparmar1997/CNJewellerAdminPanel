using CNJewellerAdmin.DTOs.Base;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CNJewellerAdmin.Helper.DateUtil
{
    public static class DateUtil
    {
        public static DateTime DefaultDate {
            get { return new DateTime(1900, 01, 01); }
        }
        public static DateTime GetCurrentDate()
        {
            var os = Environment.OSVersion.Platform.ToString();

            TimeZoneInfo ti;
            if (os.ToUpper().Contains("WIN"))
                ti = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            else
                ti = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");

            var currentDatetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ti);

            return currentDatetime;

        }
        public static void GetFirstDayinMonth(int Month, int Year, out string FirstDay)
        {
            DateTime dt = new DateTime(Year, Month, 01);

            DateTime firstDayOfThisMonth = dt.Subtract(TimeSpan.FromDays(dt.Day - 1));
            FirstDay = Convert.ToDateTime(firstDayOfThisMonth).ToString("dd/MM/yyyy");
        }
        public static void GetLastDayinMonth(int Month, int Year, out string LastDay)
        {
            DateTime dt = new DateTime(Year, Month, 01);

            DateTime firstDayOfThisMonth = dt.Subtract(TimeSpan.FromDays(dt.Day - 1));
            DateTime firstDayOfNextMonth = firstDayOfThisMonth.AddMonths(1);
            DateTime lastDayOfThisMonth = firstDayOfNextMonth.Subtract(TimeSpan.FromDays(1));
            LastDay = Convert.ToDateTime(lastDayOfThisMonth).ToString("dd/MM/yyyy");
        }
        public static void FormatDate(string date, string dateFormat, out string formatedDate)
        {
            DateTime dt = Convert.ToDateTime(date);
            formatedDate = dt.ToString(dateFormat);
        }
        public static int GetDaysInMonth(int year, int month)
        {
            DateTime dt = new DateTime(year, month, 01);

            DateTime firstDayOfThisMonth = dt.Subtract(TimeSpan.FromDays(dt.Day - 1));
            DateTime firstDayOfNextMonth = firstDayOfThisMonth.AddMonths(1);
            DateTime lastDayOfThisMonth = firstDayOfNextMonth.Subtract(TimeSpan.FromDays(1));
            return Convert.ToInt32(Convert.ToDateTime(lastDayOfThisMonth).ToString("dd"));

        }
        public static int GetNetworkDaysInMonth(int Month, int Year)
        {
            DateTime dtStart = new DateTime(Year, Month, 1);
            int NDays = 0;
            for (int i = 0; i < DateTime.DaysInMonth(Year, Month); i++)
            {
                if ((dtStart.DayOfWeek == DayOfWeek.Saturday) || (dtStart.DayOfWeek == DayOfWeek.Sunday))
                {
                    NDays++;
                }
                dtStart = dtStart.AddDays(1);
            }

            return NDays;
        }

        // converts string in "d/M/yyyy' format to DateTime object
        public static DateTime GetDate(string stringDate)
        {
            return DateTime.ParseExact(stringDate, "d/M/yyyy", CultureInfo.InvariantCulture);
        }

        public static DateTime GetTime(string stringDate)
        {
            return DateTime.ParseExact(stringDate, "HH:mm", CultureInfo.InvariantCulture);
        }

        public static DateTime? GetDateFromString(string stringDate)
        {
            if (string.IsNullOrEmpty(stringDate))
                return null;

            DateTime parsedDate;
            string[] formats = { "d/M/yyyy", "d/M/yy", "d-M-yyyy", "d-M-yy", "d M yyyy", "d M yy", "d/M/yyyy HH:mm tt", "d/M/yy HH:mm tt", "d-M-yyyy HH:mm tt", "d-M-yy HH:mm tt", "d M yyyy HH:mm tt", "d M yy HH:mm tt", "d-M-yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss" };

            bool result = DateTime.TryParseExact(stringDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
            return result ? parsedDate : null;
        }

        public static string GetDateString(DateTime? date)
        {
            if (date != null)
            {
                var dateStr = Convert.ToDateTime(date).ToString("dd/MM/yyyy");
                return dateStr.Replace("-", "/");
            }
            else
                return "";
        }

        public static string GetTimeString(DateTime? date)
        {
            if (date != null)
            {
                var dateStr = Convert.ToDateTime(date).ToString("HH:mm");
                return dateStr;
            }
            else
                return "";
        }

        public static string GetDateTimeString(DateTime? date)
        {
            if (date != null)
            {
                return Convert.ToDateTime(date).ToString("hh:mm tt  dd MMMM, yyyy");
            }
            else
                return "";
        }

        public static string GetDateTimeFromString(DateTime? date)
        {
            if (date != null)
            {
                return Convert.ToDateTime(date).ToString("dd/MM/yyyy HH:mm tt");
            }
            else
                return "";
        }

        public static int GetDueDateDays(DateTime dueDate)
        {
            DateTime dtStart = dueDate;
            int NDays = 0;
            if (dtStart.DayOfWeek == DayOfWeek.Saturday)
            {
                NDays = 2;
            }
            if (dtStart.DayOfWeek == DayOfWeek.Sunday)
            {
                NDays = 1;
            }

            return NDays;
        }

        public static DaysCountResponse WorkingDaysCount(string fromDateSTR, string toDateSTR)
        {
            DaysCountResponse daysData = new DaysCountResponse();
            daysData.TotalDays = new List<DateTime>();
            daysData.WorkingDays = new List<DateTime>();
            int workingDayCount = 0;
            int totalDayCount = 0;
            if (!string.IsNullOrEmpty(fromDateSTR) && !string.IsNullOrEmpty(toDateSTR))
            {
                var fromDate = DateUtil.GetDate(fromDateSTR);
                var toDate = DateUtil.GetDate(toDateSTR);

                var dates = new List<DateTime>();
                for (var dt = fromDate; dt <= toDate; dt = dt.AddDays(1))
                {
                    if (dt.DayOfWeek == DayOfWeek.Monday || dt.DayOfWeek == DayOfWeek.Tuesday || dt.DayOfWeek == DayOfWeek.Wednesday || dt.DayOfWeek == DayOfWeek.Thursday || dt.DayOfWeek == DayOfWeek.Friday)
                    {
                        workingDayCount += 1;
                        daysData.WorkingDays.Add(dt);
                    }
                    totalDayCount += 1;
                    daysData.TotalDays.Add(dt);
                }
            }
            daysData.WorkingDayCount = workingDayCount;
            daysData.TotalDayCount = totalDayCount;
            return daysData;
        }
        public static List<DateTime> SplitDateRange(string dateRange)
        {
            List<DateTime> dates = new List<DateTime>();
            var datest = dateRange.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (datest.Length == 2)
            {
                var format = "dd/MM/yyyy";
                var provider = CultureInfo.InvariantCulture;
                var startDate = DateTime.ParseExact(datest[0].Trim(), format, provider);
                var endDate = DateTime.ParseExact(datest[1].Trim(), format, provider);
                dates.Add(startDate);
                dates.Add(endDate);
            }
            return dates;
        }

        public static List<string> MatchDateString(string text)
        {
            // dd MMM yyyy or dd MM yy fromat
            var matches = Regex.Matches(text, @"[0-3]*\d{1}(st|nd|rd|th)?[\s\-\/]{1}(Jan|January|Feb|February|Mar|March|Apr|April|May|May|Jun|June|Jul|July|Aug|August|Sep|September|Oct|October|Nov|November|Dec|December|[0-1]?\d{1})[,]?[\s\-\/]{1}(\d{4}|\d{2})", RegexOptions.IgnoreCase);

            var list = new List<string>();
            if (matches.Count != 0)
            {
                list = matches.Select(m => m.Value).ToList();
            }
            return list;
        }

        public static string ConvertDateDDMMYYYY(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr))
                return null;

            string formattedDate = null;
            string[] formats = { "d/M/yyyy", "d-M-yyyy", "d/MM/yy", "d-MM-yy", "dd MMM yyyy", "dd MM yy" };
            DateTime parsedDate;

            formattedDate = dateStr.Replace("st", "", StringComparison.InvariantCultureIgnoreCase).
                                                                Replace("nd", "", StringComparison.InvariantCultureIgnoreCase).
                                                                Replace("rd", "", StringComparison.InvariantCultureIgnoreCase).
                                                                Replace("th", "", StringComparison.InvariantCultureIgnoreCase).
                                                                Replace(",", "");
            if (DateTime.TryParseExact(formattedDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate.ToString("dd/MM/yyyy");
            }
            else
            {
                if (DateTime.TryParse(formattedDate, out parsedDate))
                {
                    return parsedDate.ToString("dd/MM/yyyy");
                }
            }
            return null;
        }
    }

}
