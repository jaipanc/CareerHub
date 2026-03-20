namespace CareerHub.API.Domain;

// ── Country reference data ────────────────────────────────────────────────────
// PK is string Code (e.g. "CAN", "USA") — does NOT implement IEntity / have a Guid Id
public class SystemCountryCode
{
    public required string Code { get; set; }   // PK — DB column: Code
    public required string Name { get; set; }   // DB column: Name

    public virtual ICollection<ApplicantProfile>    ApplicantProfiles { get; set; } = [];
    public virtual ICollection<ApplicantWorkHistory> WorkHistories    { get; set; } = [];
}

// ── Language reference data ───────────────────────────────────────────────────
// PK is string LanguageID — does NOT implement IEntity / have a Guid Id
public class SystemLanguageCode
{
    public required string LanguageID  { get; set; }   // PK — DB column: LanguageID
    public required string Name        { get; set; }   // DB column: Name
    public required string NativeName  { get; set; }   // DB column: Native_Name

    public virtual ICollection<CompanyDescription> CompanyDescriptions { get; set; } = [];
}
