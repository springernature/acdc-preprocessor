using System;
using System.Collections.Generic;
using System.Text;

namespace Acdc.Preprocessor.Core
{
    public static class AuditLogHelper
    {
       public static List<AlertMessage> alert_message { get; set; } = null;

    }
    public class AlertMessage
    {
        public string code { get; set; }
        public string description { get; set; }
        public string elementref { get; set; }
    }
}
