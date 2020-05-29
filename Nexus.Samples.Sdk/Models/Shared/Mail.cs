using System;
using System.Collections.Generic;
using System.Text;

namespace Nexus.Samples.Sdk.Models.Shared
{
    public class MailEntityCodes
    {
        public string AccountCode { get; set; }
        public string CustomerCode { get; set; }
        public string TransactionCode { get; set; }
    }

    public class MailContent
    {
        public string Subject { get; set; }
        public string Html { get; set; }
        public string Text { get; set; }
    }

    public class MailRecipient
    {
        public string Email { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
    }
}
