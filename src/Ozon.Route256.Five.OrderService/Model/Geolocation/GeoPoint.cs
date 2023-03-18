namespace Ozon.Route256.Five.OrderService.Model.Geolocation;

public record GeoPoint(double Latitude, double Longitude)
{
    public double DistanceTo(GeoPoint point)
    {
        var d1 = Latitude * (Math.PI / 180.0);
        var num1 = Longitude * (Math.PI / 180.0);
        var d2 = point.Latitude * (Math.PI / 180.0);
        var num2 = point.Longitude * (Math.PI / 180.0) - num1;
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                 Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
        return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
    }
}