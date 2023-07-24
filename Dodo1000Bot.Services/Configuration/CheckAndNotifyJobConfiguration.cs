using System;

namespace Dodo1000Bot.Services.Configuration;

public abstract class CheckAndNotifyJobConfiguration
{
    public TimeSpan RefreshEveryTime { get; set; }
}