﻿using System.Collections.Generic;
using System.Linq;

namespace Dodo1000Bot.Models
{
    public class Dialog
    {
        public Dialog()
        {
            Parameters = new Dictionary<string, ICollection<string>>();
        }

        public IDictionary<string, ICollection<string>> Parameters { get; set; }

        public bool EndConversation { get; set; }

        public bool AllRequiredParamsPresent { get; set; }

        public string Response { get; set; }

        public string Action { get; set; }

        public Button[] Buttons { get; set; }

        public Payload Payload { get; set; }

        public IEnumerable<string> GetParameters(string key)
        {
            return Parameters?
                .Where(p => string.Equals(p.Key, key) && p.Value != null)
                .SelectMany(p => p.Value);
        }
    }
}
