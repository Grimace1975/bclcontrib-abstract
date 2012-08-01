using System;
using System.ComponentModel;

namespace Rhino.ServiceBus.FileMessaging
{
    [AttributeUsage(AttributeTargets.All)]
    public class MessagingDescriptionAttribute : DescriptionAttribute
    {
        private bool replaced;

        public MessagingDescriptionAttribute(string description)
            : base(description) { }

        public override string Description
        {
            get
            {
                if (!this.replaced)
                {
                    this.replaced = true;
                    base.DescriptionValue = "#" + base.Description;
                }
                return base.Description;
            }
        }
    }
}
