namespace HaryanaStatAbstract.API.Helpers
{
    /// <summary>
    /// Helper class for datetime operations
    /// Provides IST (Indian Standard Time) conversion utilities
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Gets the current IST (Indian Standard Time) datetime
        /// IST is UTC+5:30
        /// </summary>
        /// <returns>Current datetime in IST</returns>
        public static DateTime GetISTNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        }

        /// <summary>
        /// Converts UTC datetime to IST
        /// </summary>
        /// <param name="utcDateTime">UTC datetime to convert</param>
        /// <returns>IST datetime</returns>
        public static DateTime ConvertToIST(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        }

        /// <summary>
        /// Converts IST datetime to UTC
        /// </summary>
        /// <param name="istDateTime">IST datetime to convert</param>
        /// <returns>UTC datetime</returns>
        public static DateTime ConvertToUTC(DateTime istDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(istDateTime, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        }
    }
}
