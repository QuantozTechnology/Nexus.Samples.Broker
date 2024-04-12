﻿using System.Collections.Generic;

namespace Nexus.Samples.Broker.Configuration
{
    public class SupportedCryptoOption
    {
        public IEnumerable<SupportedCrypto> cryptos { get; set; }
    }

    public class SupportedCrypto
    {
        public string Route { get; set; }
        public string Crypto {  get; set; }
        public string Name { get; set; }
        public bool IsNative { get; set; }
    }
}