namespace Dodo1000Bot.Services.Configuration
{
    public class YoutubeConfiguration : CheckAndNotifyJobConfiguration
    {
        public string ApiKey { get; set; }

        public string Endpoint { get; set; }

        public string[] Channels { get; set; }
    }
}