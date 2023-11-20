namespace Dodo1000Bot.Models.PublicApi
{
    public class Location
    {
        public float? Latitude { get; set; }
        public float LatitudeOrDefault => Latitude.GetValueOrDefault();

        public float? Longitude { get; set; }
        public float LongitudeOrDefault => Longitude.GetValueOrDefault();
    }
}