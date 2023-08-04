namespace Dodo1000Bot.Models.GlobalApi
{
    public class CoordinatesModel
    {
        public float? Lat { get; set; }
        public float Latitude => Lat.GetValueOrDefault();

        public float? Long { get; set; }
        public float Longitude => Long.GetValueOrDefault();
    }
}