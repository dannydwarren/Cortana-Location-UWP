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

		public string StardateString => $"{StarDate.ToString( "yyyy" )}.{StarDate.DayOfYear}{StarDate.ToString( "HHmmssff" )}";

		public string CurrentDateString => StarDate.ToLocalTime().ToString( "yyyy-MM-DD HH:mm:ss:ff tt zzz" );
	}
}