namespace CustomerConsolidationFn.Domain
{
    public class CustomerRecord
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Country { get; set; }
        public DateTime LastModifiedUtc { get; set; }
    }
}