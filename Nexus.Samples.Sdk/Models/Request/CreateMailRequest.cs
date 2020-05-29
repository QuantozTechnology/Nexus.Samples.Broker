using Nexus.Samples.Sdk.Models.Shared;

namespace Nexus.Samples.Sdk.Models.Request
{
    public class CreateMailRequest
    {
        public string Type { get; set; }
        public MailEntityCodes References { get; set; }
        public MailContent Content { get; set; }
        public MailRecipient Recipient { get; set; }
    }
}
