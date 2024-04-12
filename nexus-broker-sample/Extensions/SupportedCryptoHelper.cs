using Microsoft.Extensions.Options;
using Nexus.Samples.Broker.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Nexus.Samples.Broker.Extensions
{
    public class SupportedCryptoHelper
    {
        private IEnumerable<SupportedCrypto> _supportedCryptos;

        public SupportedCryptoHelper(IOptions<SupportedCryptoOption> options)
        {
            _supportedCryptos = options.Value.cryptos;
        }

        public SupportedCrypto GetSupportedCryptoFromRoute(string route)
        {
            return _supportedCryptos.First(crypto => string.Equals(crypto.Route.ToLower(), route.ToLower()));
        }

        public SupportedCrypto GetSupportedCrypto(string cryptoCode)
        {
            return _supportedCryptos.First(crypto => string.Equals(crypto.Crypto.ToLower(), cryptoCode.ToLower()));
        }

        public IEnumerable<SupportedCrypto> GetAllSupportedCryptos()
        {
            return _supportedCryptos;
        }
    }
}
