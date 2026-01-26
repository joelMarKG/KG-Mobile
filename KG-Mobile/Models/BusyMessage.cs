using System;
using System.Collections.Generic;
using System.Text;

namespace KG.Mobile.Models
{
    public class BusyMessage
    {
        public bool visible { get; set; }
        public string message { get; set; }

        public BusyMessage(bool visible, string message)
        {
            this.visible = visible;
            this.message = message;
        }
    }
}
