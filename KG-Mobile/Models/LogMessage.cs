using System;
using System.Collections.Generic;
using System.Text;

namespace KG.Mobile.Models
{
    public class LogMessage
    {
        public string type { get; set; }
        public string component { get; set; }
        public string comment { get; set; }

        public LogMessage(string type, string component, string comment)
        {
            this.type = type;
            this.component = component;
            this.comment = comment;
        }
    }

    public class LogMessageRequest
    {
        public LogMessage Log { get; }

        public LogMessageRequest(LogMessage log)
        {
            Log = log;
        }
    }
}
