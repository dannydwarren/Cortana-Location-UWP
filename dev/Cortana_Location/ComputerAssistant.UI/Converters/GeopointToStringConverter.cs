using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Data;

namespace ComputerAssistant.UI.Converters
{
	public class GeopointToStringConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, string language )
		{
			if ( value == null )
			{
				return string.Empty;
			}

			Geopoint point = (Geopoint)value;

			return $"Lat: {point.Position.Latitude}, Long: {point.Position.Longitude}, Alt: {point.Position.Altitude}";
		}

		public object ConvertBack( object value, Type targetType, object parameter, string language )
		{
			throw new NotImplementedException();
		}
	}
}