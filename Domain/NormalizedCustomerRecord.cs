namespace CustomerConsolidationFn.Domain
{
    public class NormalizedCustomerRecord
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string CountryOriginal { get; set; }
        public required string? CountryIso2 { get; set; }
        public DateTime LastModifiedUtc { get; set; }
    }
}