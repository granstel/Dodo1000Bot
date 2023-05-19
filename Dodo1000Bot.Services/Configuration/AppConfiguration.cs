﻿namespace Dodo1000Bot.Services.Configuration
{
    public class AppConfiguration
    {
        public HttpLogConfiguration HttpLog { get; set; }

        public DialogflowConfiguration Dialogflow { get; set; }

        public RedisConfiguration Redis { get; set; }

        public UnitsConfiguration Units { get; set; }
    }
}
