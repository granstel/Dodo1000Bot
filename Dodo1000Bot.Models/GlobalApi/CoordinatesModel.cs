namespace Dodo1000Bot.Models.GlobalApi
{
    public class CoordinatesModel
    {
        public float? Lat { get; set; }
        public float LatitudeOrDefault => Lat.GetValueOrDefault();

        public float? Long { get; set; }
        public float LongitudeOrDefault => Long.GetValueOrDefault();
    }
}