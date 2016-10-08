using DotCore.Common;
using DotCore.Util;
using DotTime.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotTime.Ranges
{
    // Note: We use this instead of TimeRangeUtil for now...
    // ...
    // Note: All time periods (day, week, month, etc.) are [begin, end) (e.g., begin is included, but end is excluded)...
    public static class DateRangeUtil
    {
        // UTC based.... for now... ???
        // TBD: Make it timezone dependent ????
        // Howt to handle daylight saving time????
        // private const string TZNAME_USPACIFIC = "US/Pacific";   // ??
        private static readonly TimeZoneInfo TIMEZONE_UTC = TimeZoneInfo.Utc;


        // ????
        private static readonly Regex sRegex = new Regex("\\d{4}-\\d{2}-\\d{2}");

        // temporary
        // Need testing...
        public static bool IsValid(string dayHour)
        {
            if (dayHour == null || dayHour.Length < 10) {
                return false;
            }
            return sRegex.Matches(dayHour).Count > 0;
        }

        // TBD...
        //public static bool IsValid(string termType, string dayHour)
        // ...
        // ...


        public static string GetTermTime(string termType)
        {
            if (TermTypes.TERM_HOURLY == termType) {
                return GetDayHour();
            } else if (TermTypes.TERM_DAILY == termType) {
                return GetDay();
            } else if (TermTypes.TERM_WEEKLY == termType) {
                return GetWeek();
            } else if (TermTypes.TERM_MONTHLY == termType) {
                return GetMonth();
            } else {
                return null;   // ????
            }
        }

        public static string GetTermTime(string termType, long time)
        {
            if (TermTypes.TERM_HOURLY == termType) {
                return GetDayHour(time);
            } else if (TermTypes.TERM_DAILY == termType) {
                return GetDay(time);
            } else if (TermTypes.TERM_WEEKLY == termType) {
                return GetWeek(time);
            } else if (TermTypes.TERM_MONTHLY == termType) {
                return GetMonth(time);
            } else {
                return null;   // ????
            }
        }


        public static string GetMonth()
        {
            return GetMonth(DateTimeUtil.CurrentUnixEpochMillis());
        }
        public static string GetMonth(long time)
        {
            var date = time.ToLocalDateTime().ToString("yyyy-MM-01");
            return date;
        }

        // tbd:
        // This is based on the assumption, a "week" starts from Sunday.
        // In some culture, a week may start from a different day....
        public static string GetWeek()
        {
            return GetWeek(DateTimeUtil.CurrentUnixEpochMillis());
        }
        public static string GetWeek(long time)
        {
            long t = 0L;
            int day = GetDayOfWeek(time);
            t = time - day * 3600 * 24 * 1000L;
            return GetDay(t);
        }
        // temporary
        private static int GetDayOfWeek(long time)
        {
            string day = time.ToLocalDateTime().ToString("dddd");
            // System.Diagnostics.Debug.WriteLine("GetDayOfWeek(): day = " + day + "; time = " + time);

            int dayOfWeek = 0;
            if (day.StartsWith("Su")) {
                dayOfWeek = 0;
            } else if (day.StartsWith("Mo")) {
                dayOfWeek = 1;
            } else if (day.StartsWith("Tu")) {
                dayOfWeek = 2;
            } else if (day.StartsWith("We")) {
                dayOfWeek = 3;
            } else if (day.StartsWith("Th")) {
                dayOfWeek = 4;
            } else if (day.StartsWith("Fr")) {
                dayOfWeek = 5;
            } else if (day.StartsWith("Sa")) {
                dayOfWeek = 6;
            }
            return dayOfWeek;
        }

        public static string GetDay()
        {
            return GetDay(DateTimeUtil.CurrentUnixEpochMillis());
        }
        public static string GetDay(long time)
        {
            var date = time.ToLocalDateTime().ToString("yyyy-MM-dd");
            return date;
        }

        public static string GetDayHour()
        {
            return GetDayHour(DateTimeUtil.CurrentUnixEpochMillis());
        }
        public static string GetDayHour(long time)
        {
            var date = time.ToLocalDateTime().ToString("yyyy-MM-dd HH:mm");
            return date;
        }


        // ???
        public static string GetMonthFromDayHour(string dayHour)
        {
            if (dayHour == null || dayHour.Length < 10) {
                System.Diagnostics.Debug.WriteLine("Invalid input: dayHour = " + dayHour);
                return dayHour;  // ???
            }
            //string month = dayHour.Substring(0, 7) + "-01 00:00";  // ???
            string month = dayHour.Substring(0, 7) + "-01";  // ???
            return month;
        }
        public static string GetWeekFromDayHour(string dayHour)
        {
            if (dayHour == null || dayHour.Length < 10) {
                System.Diagnostics.Debug.WriteLine("Invalid input: dayHour = " + dayHour);
                return dayHour;  // ???
            }
            return GetWeek(GetMilli(dayHour));  // ????
        }
        public static string GetDayFromDayHour(string dayHour)
        {
            if (dayHour == null || dayHour.Length < 10) {
                System.Diagnostics.Debug.WriteLine("Invalid input: dayHour = " + dayHour);
                return dayHour;  // ???
            }
            //string day = dayHour.Substring(0, 10) + " 00:00";  // ???
            string day = dayHour.Substring(0, 10);  // ???
            return day;
        }

        public static string[] GetThisTermRange(string termType)
        {
            var now = DateTimeUtil.CurrentUnixEpochMillis();
            var startDayHour = GetTermTime(termType, now);
            var endDayHour = GetTermTime(termType, now + 1L);
            var range = GetRange(termType, startDayHour, endDayHour);
            return range;
        }


        public static string[] GetRange(string termType, string startDayHour, string endDayHour)
        {
            if (TermTypes.TERM_HOURLY == termType) {
                return GetDayHourRange(startDayHour, endDayHour);
            } else if (TermTypes.TERM_DAILY == termType) {
                return GetDayRange(startDayHour, endDayHour);
            } else if (TermTypes.TERM_WEEKLY == termType) {
                return GetWeekRange(startDayHour, endDayHour);
            } else if (TermTypes.TERM_MONTHLY == termType) {
                return GetMonthRange(startDayHour, endDayHour);
            } else {
                return null;   // ????
            }
        }

        public static string[] GetMonthRange(string startDayHour, string endDayHour)
        {
            if (!IsValid(startDayHour)) {
                return null;
            }
            if (endDayHour == null || !IsValid(endDayHour)) {
                endDayHour = GetMonth();
                System.Diagnostics.Debug.WriteLine("Invalid endDayHour: New value = " + endDayHour);
            }

            // temporary.
            // Not the most efficient algo.
            var vals = new List<string>();
            string pointer = startDayHour;
            while (pointer.CompareTo(endDayHour) <= 0) {
                vals.Add(pointer);
                pointer = GetNextMonth(pointer);
            }

            string[] range = vals.ToArray();
            return range;
        }

        public static string[] GetWeekRange(string startDayHour, string endDayHour)
        {
            if (!IsValid(startDayHour)) {
                return null;
            }
            if (endDayHour == null || !IsValid(endDayHour)) {
                endDayHour = GetWeek();
                System.Diagnostics.Debug.WriteLine("Invalid endDayHour: New value = " + endDayHour);
            }

            // temporary.
            // Not the most efficient algo.
            List<string> vals = new List<string>();
            string pointer = startDayHour;
            while (pointer.CompareTo(endDayHour) <= 0) {
                vals.Add(pointer);
                pointer = GetNextWeek(pointer);
            }

            string[] range = vals.ToArray();
            return range;
        }

        public static string[] GetDayRange(string startDayHour, string endDayHour)
        {
            if (!IsValid(startDayHour)) {
                return null;
            }
            if (endDayHour == null || !IsValid(endDayHour)) {
                endDayHour = GetDay();
                System.Diagnostics.Debug.WriteLine("Invalid endDayHour: New value = " + endDayHour);
            }

            // temporary.
            // Not the most efficient algo.
            var vals = new List<string>();
            string pointer = startDayHour;
            while (pointer.CompareTo(endDayHour) <= 0) {
                vals.Add(pointer);
                pointer = GetNextDay(pointer);
            }

            string[] range = vals.ToArray();
            return range;
        }

        public static string[] GetDayHourRange(string startDayHour, string endDayHour)
        {
            if (!IsValid(startDayHour)) {
                return null;
            }
            if (endDayHour == null || !IsValid(endDayHour)) {
                endDayHour = GetDayHour();
                System.Diagnostics.Debug.WriteLine("Invalid endDayHour: New value = " + endDayHour);
            }

            // temporary.
            // Not the most efficient algo.
            List<string> vals = new List<string>();
            string pointer = startDayHour;
            while (pointer.CompareTo(endDayHour) <= 0) {
                vals.Add(pointer);
                pointer = GetNextDayHour(pointer);
            }

            string[] range = vals.ToArray();
            return range;
        }





        // temporary
        // Note that "past" range is a bit special.
        // All ranges start from low and end in high.
        // The range of past terms are reversed.

        // numDays == 0 means, today only.
        public static string[] GetRangeForPastDays(int numDays)
        {
            var now = DateTimeUtil.CurrentUnixEpochMillis();
            return GetRangeForPastDays(now, numDays);
        }
        public static string[] GetRangeForPastDays(long now, int numDays)
        {
            var end = now - (numDays - 1) * 24 * 3600 * 1000L;   // This should be good enough in most case where numDays is sufficiently small.
            var startDay = GetDay(now);
            var endDay = GetDay(end);
            var range = GetDayRange(endDay, startDay);
            return range.Reverse().ToArray();
        }

        public static string[] GetRangeForFutureDays(int numDays)
        {
            var now = DateTimeUtil.CurrentUnixEpochMillis();
            return GetRangeForFutureDays(now, numDays);
        }
        public static string[] GetRangeForFutureDays(long now, int numDays)
        {
            var end = now + (numDays - 1) * 24 * 3600 * 1000L;
            var startDay = GetDay(now);
            var endDay = GetDay(end);
            var range = GetDayRange(startDay, endDay);
            return range;
        }


        // numWeeks == 0 means, this week only.
        public static string[] GetRangeForPastWeeks(int numWeeks)
        {
            var now = DateTimeUtil.CurrentUnixEpochMillis();
            return GetRangeForPastWeeks(now, numWeeks);
        }
        public static string[] GetRangeForPastWeeks(long now, int numWeeks)
        {
            var end = now - (numWeeks - 1) * 7 * 24 * 3600 * 1000L;   // This should be good enough in most case where numWeeks is sufficiently small.
            var startWeek = GetWeek(now);
            var endWeek = GetWeek(end);
            var range = GetWeekRange(endWeek, startWeek);
            return range.Reverse().ToArray();
        }

        public static string[] GetRangeForFutureWeeks(int numWeeks)
        {
            var now = DateTimeUtil.CurrentUnixEpochMillis();
            return GetRangeForFutureWeeks(now, numWeeks);
        }
        public static string[] GetRangeForFutureWeeks(long now, int numWeeks)
        {
            var end = now + (numWeeks - 1) * 7 * 24 * 3600 * 1000L;
            var startWeek = GetWeek(now);
            var endWeek = GetWeek(end);
            var range = GetWeekRange(startWeek, endWeek);
            return range;
        }

        // tbd:
        // Month is a bit tricky.
        // We need to iterate through one month at a time using GetNextMonth() or GetPreviousMonth()
        // ....


        // numHours == 0 means, this hour only.
        public static string[] GetRangeForPastDayHours(int numHours)
        {
            var now = DateTimeUtil.CurrentUnixEpochMillis();
            return GetRangeForPastDayHours(now, numHours);
        }
        public static string[] GetRangeForPastDayHours(long now, int numHours)
        {
            var end = now - (numHours - 1) * 3600 * 1000L;
            var startDayHour = GetDayHour(now);
            var endDayHour = GetDayHour(end);
            var range = GetDayHourRange(endDayHour, startDayHour);
            return range.Reverse().ToArray();
        }

        public static string[] GetRangeForFutureDayHours(int numHours)
        {
            var now = DateTimeUtil.CurrentUnixEpochMillis();
            return GetRangeForFutureDayHours(now, numHours);
        }
        public static string[] GetRangeForFutureDayHours(long now, int numHours)
        {
            var end = now + (numHours - 1) * 3600 * 1000L;
            var startDayHour = GetDayHour(now);
            var endDayHour = GetDayHour(end);
            var range = GetDayHourRange(startDayHour, endDayHour);
            return range;
        }





        public static long GetMilli(string day)
        {
            long milli = 0L;
            try {
                var date1 = DateTime.ParseExact(day, "yyyy-MM-dd", null, DateTimeStyles.None);
                milli = date1.ToUnixEpochMillis();
            } catch (Exception ex1) {
                System.Diagnostics.Debug.WriteLine("Failed to parse the input string: day1 = " + day, ex1);
                try {
                    var date2 = DateTime.ParseExact(day, "yyyy-MM-dd HH:mm", null, DateTimeStyles.None);
                    milli = date2.ToUnixEpochMillis();
                } catch (Exception ex2) {
                    System.Diagnostics.Debug.WriteLine("Failed to parse the input string: day2 = " + day, ex2);
                }
            }
            return milli;
        }

        public static long[] GetMilliRange(string termType, string dayHour)
        {
            long[] range = new long[2];
            range[0] = GetMilli(dayHour);
            range[1] = GetMilli(GetNext(termType, dayHour));
            return range;
        }



        // TBD:
        // Methods to convert an arbitrary date string into a valid date/time based on termType...
        // ...




        public static string GetNext(string termType, string dayHour)
        {
            if (TermTypes.TERM_HOURLY == termType) {
                return GetNextDayHour(dayHour);
            } else if (TermTypes.TERM_DAILY == termType) {
                return GetNextDay(dayHour);
            } else if (TermTypes.TERM_WEEKLY == termType) {
                return GetNextWeek(dayHour);
            } else if (TermTypes.TERM_MONTHLY == termType) {
                return GetNextMonth(dayHour);
            } else {
                return null;   // ????
            }
        }
        public static string GetPrevious(string termType, string dayHour)
        {
            if (TermTypes.TERM_HOURLY == termType) {
                return GetPreviousDayHour(dayHour);
            } else if (TermTypes.TERM_DAILY == termType) {
                return GetPreviousDay(dayHour);
            } else if (TermTypes.TERM_WEEKLY == termType) {
                return GetPreviousWeek(dayHour);
            } else if (TermTypes.TERM_MONTHLY == termType) {
                return GetPreviousMonth(dayHour);
            } else {
                return null;   // ????
            }
        }


        public static string GetNextMonth(string dayHour)
        {
            if (dayHour == null || dayHour.Length < 10) {
                System.Diagnostics.Debug.WriteLine("Invalid input: dayHour = " + dayHour);
                return null;  // ???
            }
            string currentMonth = GetMonthFromDayHour(dayHour);
            int year = Convert.ToInt32(currentMonth.Substring(0, 4));
            int month = Convert.ToInt32(currentMonth.Substring(5, 7)) + 1;
            if (month > 12) {
                month = 1;
                year += 1;
            }
            string mo = "";
            if (month < 10) {
                mo = "0" + month;
            } else {
                mo = "" + month;
            }
            //string nextMonth = year + "-" + mo + "-01 00:00"; 
            string nextMonth = year + "-" + mo + "-01";
            return nextMonth;
        }
        public static string GetPreviousMonth(string dayHour)
        {
            if (dayHour == null || dayHour.Length < 10) {
                System.Diagnostics.Debug.WriteLine("Invalid input: dayHour = " + dayHour);
                return null;  // ???
            }
            string currentMonth = GetMonthFromDayHour(dayHour);
            int year = Convert.ToInt32(currentMonth.Substring(0, 4));
            int month = Convert.ToInt32(currentMonth.Substring(5, 7)) - 1;
            if (month < 1) {
                month = 12;
                year -= 1;
            }
            string mo = "";
            if (month < 10) {
                mo = "0" + month;
            } else {
                mo = "" + month;
            }
            //string prevMonth = year + "-" + mo + "-01 00:00"; 
            string prevMonth = year + "-" + mo + "-01";
            return prevMonth;
        }

        public static string GetNextWeek(string dayHour)
        {
            if (dayHour == null || dayHour.Length < 10) {
                System.Diagnostics.Debug.WriteLine("Invalid input: dayHour = " + dayHour);
                return null;  // ???
            }
            long milli = GetMilli(dayHour) + 7 * 3600 * 24 * 1000L;
            return GetWeek(milli);
        }
        public static string GetPreviousWeek(string dayHour)
        {
            if (dayHour == null || dayHour.Length < 10) {
                System.Diagnostics.Debug.WriteLine("Invalid input: dayHour = " + dayHour);
                return null;  // ???
            }
            long milli = GetMilli(dayHour) - 7 * 3600 * 24 * 1000L;
            return GetWeek(milli);
        }

        public static string GetNextDay(string dayHour)
        {
            if (dayHour == null || dayHour.Length < 10) {
                System.Diagnostics.Debug.WriteLine("Invalid input: dayHour = " + dayHour);
                return null;  // ???
            }
            long milli = GetMilli(dayHour) + 3600 * 24 * 1000L;
            return GetDay(milli);
        }
        public static string GetPreviousDay(string dayHour)
        {
            if (dayHour == null || dayHour.Length < 10) {
                System.Diagnostics.Debug.WriteLine("Invalid input: dayHour = " + dayHour);
                return null;  // ???
            }
            long milli = GetMilli(dayHour) - 3600 * 24 * 1000L;
            return GetDay(milli);
        }

        public static string GetNextDayHour(string dayHour)
        {
            if (dayHour == null || dayHour.Length < 10) {
                System.Diagnostics.Debug.WriteLine("Invalid input: dayHour = " + dayHour);
                return null;  // ???
            }
            long milli = GetMilli(dayHour) + 3600 * 1000L;
            return GetDayHour(milli);
        }
        public static string GetPreviousDayHour(string dayHour)
        {
            if (dayHour == null || dayHour.Length < 10) {
                System.Diagnostics.Debug.WriteLine("Invalid input: dayHour = " + dayHour);
                return null;  // ???
            }
            long milli = GetMilli(dayHour) - 3600 * 1000L;
            return GetDayHour(milli);
        }



        public static int CompareMonths(string dayL, string dayR)
        {
            return Compare(dayL, dayR);
        }
        public static int CompareWeeks(string dayL, string dayR)
        {
            return Compare(dayL, dayR);
        }
        public static int CompareDays(string dayL, string dayR)
        {
            return Compare(dayL, dayR);
        }
        public static int CompareHours(string dayL, string dayR)
        {
            return Compare(dayL, dayR);
        }


        // ???
        public static int CompareHourAndDay(string hourL, string dayR)
        {
            string dayL = GetDayFromDayHour(hourL);
            return Compare(dayL, dayR);
        }
        public static int CompareHourAndWeek(string hourL, string dayR)
        {
            string weekL = GetWeekFromDayHour(hourL);
            return Compare(weekL, dayR);
        }
        public static int CompareHourAndMonth(string hourL, string dayR)
        {
            string monthL = GetMonthFromDayHour(hourL);
            return Compare(monthL, dayR);
        }
        public static int CompareDayAndWeek(string hourL, string dayR)
        {
            string weekL = GetWeekFromDayHour(hourL);
            return Compare(weekL, dayR);
        }
        public static int CompareDayAndMonth(string hourL, string dayR)
        {
            string monthL = GetMonthFromDayHour(hourL);
            return Compare(monthL, dayR);
        }
        public static int CompareWeekAndMonth(string hourL, string dayR)
        {
            string monthL = GetMonthFromDayHour(hourL);
            return Compare(monthL, dayR);
        }


        public static int CompareTime(string termType, long time, string dayR)
        {
            if (TermTypes.TERM_HOURLY == termType) {
                return CompareTimeAndHour(time, dayR);
            } else if (TermTypes.TERM_DAILY == termType) {
                return CompareTimeAndDay(time, dayR);
            } else if (TermTypes.TERM_WEEKLY == termType) {
                return CompareTimeAndWeek(time, dayR);
            } else if (TermTypes.TERM_MONTHLY == termType) {
                return CompareTimeAndMonth(time, dayR);
            } else {
                return 0;   // ????
            }
        }

        public static int CompareTimeAndHour(long time, string dayR)
        {
            string dayL = GetDayHour(time);
            return Compare(dayL, dayR);
        }
        public static int CompareTimeAndDay(long time, string dayR)
        {
            string dayL = GetDay(time);
            return Compare(dayL, dayR);
        }
        public static int CompareTimeAndWeek(long time, string dayR)
        {
            string dayL = GetWeek(time);
            return Compare(dayL, dayR);
        }
        public static int CompareTimeAndMonth(long time, string dayR)
        {
            string dayL = GetMonth(time);
            return Compare(dayL, dayR);
        }


        // This has somewhat ambiguous semantics. Use the above, more specialized methods...
        private static int Compare(string dayL, string dayR)
        {
            if (dayL == null && dayR == null) {
                return 0;
            }
            if (dayL == null) {
                return -1;
            }
            if (dayR == null) {
                return 1;
            }
            return dayL.CompareTo(dayR);
        }


    }
}
