using System;
using System.Collections.Generic;
using System.Text;

namespace KG.Mobile.Models
{
    public enum FocusTarget
    {
        MoveToLocationName,
        ItemBarcode
    }

    public sealed class FocusRequestMessage
    {
        public FocusTarget Target { get; }

        public FocusRequestMessage(FocusTarget target)
        {
            Target = target;
        }
    }

}
