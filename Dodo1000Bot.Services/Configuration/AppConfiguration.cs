﻿namespace Dodo1000Bot.Services.Configuration
{
    public class AppConfiguration
    {
        public HttpLogConfiguration HttpLog { get; set; }

        public DialogflowConfiguration Dialogflow { get; set; }

        public RedisConfiguration Redis { get; set; }

        public UnitsJobConfiguration UnitsJob { get; set; }

        public StatisticsJobConfiguration StatisticsJob { get; set; }

        public string GlobalApiEndpoint { get; set; }

        public string RealtimeBoardApiClientEndpoint { get; set; }

        public string RestcountriesApiClientEndpoint { get; set; }

        public string MysqlConnectionString { get; set; }

        public PushNotificationsConfiguration PushNotifications { get; set; }

        public ManagementConfiguration Management { get; set; }

        public YoutubeConfiguration YouTube { get; set; }
    }
}
