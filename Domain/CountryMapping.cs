using System.Collections.Generic;

namespace CustomerConsolidationFn.Domain
{
    public static class CountryMapping
    {
        public static readonly Dictionary<string, string> NameToIso2 = new()
        {
            { "United States", "US" },
            { "Canada", "CA" },
            { "United Kingdom", "GB" },
            { "Germany", "DE" },
            { "France", "FR" }
        };
    }
}