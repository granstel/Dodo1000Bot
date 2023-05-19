using System;

namespace Dodo1000Bot.Services.Configuration
{
    public abstract class Configuration
    {
        public string ExpandVariable(string variableName)
        {
            return Environment.ExpandEnvironmentVariables(variableName ?? string.Empty);
        }
    }
}
