using System;
using Nexus.Samples.Sdk.Models.Shared;

namespace Nexus.Samples.Sdk.Models.Response
{
    public class GetMailResponse
    {
        public string Code { get; set; }
        public DateTime Created { get; set; }
        public string Sent { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public MailEntityCodes References { get; set; }
        public MailContent Content { get; set; }
        public MailRecipient Recipient { get; set; }
    }
}
