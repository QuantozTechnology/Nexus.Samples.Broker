using System.Collections.Generic;

namespace Nexus.Samples.Sdk.Models.Response
{
    public class GetPaymentMethodResponseList
    {
        public IEnumerable<GetPaymentMethodResponse> PaymentMethods { get; set; }
    }
    public class GetPaymentMethodResponse
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string? CryptoCode { get; set; }
        public string? CurrencyCode { get; set; }
        public PaymentMethodFees Fees { get; set; }
        public PaymentType PaymentType { get; set; }
    }
    public class PaymentMethodFees
    {
        public BankFees Bank { get; set; }
        public ServiceFees Service { get; set; }
    }
    public class BankFees
    {
        public decimal Fixed { get; set; }
        public decimal Relative { get; set; }
    }

    public class ServiceFees
    {
        public decimal Relative { get; set; }
    }

    public class PaymentType
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}


