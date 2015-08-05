using Windows.Devices.Geolocation;

namespace ComputerAssistant.UI.Models
{
	public class BeamedLocation : DateTimeStampedBindable
	{

		public BeamedLocation( Geoposition geoposition )
		{
			Point = geoposition.Coordinate.Point;
			Latitude = geoposition.Coordinate.Point.Position.Latitude;
			Longitude = geoposition.Coordinate.Point.Position.Longitude;
		}

		private Geopoint _point;
		public Geopoint Point
		{
			get
			{
				return _point
					   ?? ( _point =
						   new Geopoint(
							   new BasicGeoposition
							   {
								   Latitude = Latitude,
								   Longitude = Longitude
							   } ) );
			}
			set { _point = value; }
		}

		public double Longitude { get; set; }

		public double Latitude { get; set; }
	}
}