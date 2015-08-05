using System;

namespace ComputerAssistant.UI.Models
{
	public class CaptainsLogEntry : DateTimeStampedBindable
	{
		public CaptainsLogEntry(string entryText)
		{
			LogEntry = entryText;
		}

		private string _logEntry;
		public string LogEntry
		{
			get { return _logEntry; }
			set
			{
				if ( Equals( value, _logEntry ) )
				{
					return;
				}

				_logEntry = value;
				NotifyPropertyChanged();
			}
		}
	}
}