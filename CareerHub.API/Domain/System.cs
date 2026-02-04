namespace CareerHub.API.Domain
{
    public class SystemCountryCode
    {
        // The "Code" is the Primary Key (e.g., "CAN", "USA")
        public required string Code { get; set; }
        public required string Name { get; set; }

        // Reverse Navigation (Optional, but useful for OData/GraphQL)
        public virtual ICollection<ApplicantProfile> ApplicantProfiles { get; set; } = [];
        public virtual ICollection<ApplicantWorkHistory> WorkHistories { get; set; } = [];
    }

    public class SystemLanguageCode
    {
        public required string LanguageID { get; set; }
        public required string Name { get; set; }
        public required string NativeName { get; set; }
    }
}
