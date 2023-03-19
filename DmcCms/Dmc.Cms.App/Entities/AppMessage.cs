using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class AppMessage
    {
        public AppMessage(MessageType type, string message)
        {
            MessageType = type;
            Message = message;
        }

        public MessageType MessageType
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }
    }
}
