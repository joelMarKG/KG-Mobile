using System;
using System.Collections.Generic;
using System.Text;

namespace KG.Mobile.Models
{
    public class PopupMessage
    {
        public string title { get; set; }
        public string component { get; set; }
        public string message { get; set; }
        public string buttonText { get; set; }

        public PopupMessage(string title, string component, string message, string buttonText)
        {
            this.title = title;
            this.component = component;
            this.message = message;
            this.buttonText = buttonText;
        }
    }
    public class PopupErrorMessage
    {
        public PopupMessage Popup { get; }

        public PopupErrorMessage(PopupMessage popup)
        {
            Popup = popup;
        }
    }

    public class PopupMessageRequest
    {
        public PopupMessage Popup { get; }

        public PopupMessageRequest(PopupMessage popup)
        {
            Popup = popup;
        }
    }
}
