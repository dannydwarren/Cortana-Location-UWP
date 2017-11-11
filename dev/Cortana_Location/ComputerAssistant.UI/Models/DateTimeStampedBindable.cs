using System;

namespace ComputerAssistant.UI.Models
{
    public class DateTimeStampedBindable : Bindable
    {
        public DateTimeStampedBindable()
        {
            StarDate = DateTime.UtcNow;
        }

        public DateTime StarDate { get; set; }

        public string StardateString => $"{StarDate.ToStarDate()}";

        public string CurrentDateString => StarDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss:ff tt zzz");
    }

    public static class Extensions
    {
        /// <summary>
        /// StarDate by ballance on GitHub: https://github.com/ballance/StarDate
        /// https://github.com/ballance/StarDate/blob/master/Ballance.StarDate/StarDateConverter.cs</summary>
        /// <param name="earthDateTime"></param>
        /// <returns></returns>
        public static double ToStarDate(this DateTime earthDateTime)
        {
            var starDateOrigin = new DateTime(2318, 7, 5, 12, 0, 0);
            var earthToStarDateDiff = earthDateTime - starDateOrigin;
            var millisecondConversion = earthToStarDateDiff.TotalMilliseconds / (34367056.4);
            var starDate = Math.Floor(millisecondConversion * 100) / 100;

            return Math.Round(starDate, 2, MidpointRounding.AwayFromZero);
        }
    }
}