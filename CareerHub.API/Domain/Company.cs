namespace CareerHub.API.Domain;

// ── Company Profile (Aggregate Root) ─────────────────────────────────────────
public class CompanyProfile
{
    public Guid     Id               { get; set; }
    public DateTime RegistrationDate { get; set; }   // DB column: Registration_Date
    public string?  CompanyWebsite   { get; set; }   // DB column: Company_Website
    public string?  ContactPhone     { get; set; }   // DB column: Contact_Phone
    public string?  ContactName      { get; set; }   // DB column: Contact_Name
    public byte[]?  CompanyLogo      { get; set; }   // DB column: Company_Logo
    public byte[]?  TimeStamp        { get; set; }   // DB column: Time_Stamp (rowversion)

    public virtual ICollection<CompanyDescription> Descriptions { get; set; } = [];
    public virtual ICollection<CompanyLocation>    Locations    { get; set; } = [];
    public virtual ICollection<CompanyJob>         Jobs         { get; set; } = [];
}

// ── Company Description (localized per language) ──────────────────────────────
public class CompanyDescription
{
    public Guid    Id          { get; set; }
    public Guid    CompanyId   { get; set; }   // DB column: Company
    public string? LanguageId  { get; set; }   // DB column: LanguageId  → FK to SystemLanguageCode
    public string? CompanyName { get; set; }   // DB column: Company_Name
    public string? Description { get; set; }   // DB column: Company_Description
    public byte[]? TimeStamp   { get; set; }   // DB column: Time_Stamp (rowversion)

    public virtual CompanyProfile?      CompanyProfile { get; set; }
    public virtual SystemLanguageCode?  LanguageCode   { get; set; }
}

// ── Company Location ──────────────────────────────────────────────────────────
public class CompanyLocation
{
    public Guid    Id          { get; set; }
    public Guid    CompanyId   { get; set; }   // DB column: Company
    public string? CountryCode { get; set; }   // DB column: Country_Code
    public string? Province    { get; set; }   // DB column: State_Province_Code
    public string? Street      { get; set; }   // DB column: Street_Address
    public string? City        { get; set; }   // DB column: City_Town
    public string? PostalCode  { get; set; }   // DB column: Zip_Postal_Code
    public byte[]? TimeStamp   { get; set; }   // DB column: Time_Stamp (rowversion)

    public virtual CompanyProfile? CompanyProfile { get; set; }
}

// ── Company Job ───────────────────────────────────────────────────────────────
public class CompanyJob
{
    public Guid     Id              { get; set; }
    public Guid     CompanyId       { get; set; }   // DB column: Company
    public DateTime ProfileCreated  { get; set; }   // DB column: Profile_Created
    public bool     IsInactive      { get; set; }   // DB column: Is_Inactive
    public bool     IsCompanyHidden { get; set; }   // DB column: Is_Company_Hidden
    public byte[]?  TimeStamp       { get; set; }   // DB column: Time_Stamp (rowversion)

    public virtual CompanyProfile?                         CompanyProfile { get; set; }
    public virtual ICollection<CompanyJobDescription>      Descriptions   { get; set; } = [];
    public virtual ICollection<CompanyJobEducation>        Educations     { get; set; } = [];
    public virtual ICollection<CompanyJobSkill>            Skills         { get; set; } = [];
    public virtual ICollection<ApplicantJobApplication>    Applications   { get; set; } = [];
}

// ── Job Description ───────────────────────────────────────────────────────────
public class CompanyJobDescription
{
    public Guid    Id          { get; set; }
    public Guid    JobId       { get; set; }   // DB column: Job
    public string? JobName     { get; set; }   // DB column: Job_Name
    public string? Description { get; set; }   // DB column: Job_Descriptions
    public byte[]? TimeStamp   { get; set; }   // DB column: Time_Stamp (rowversion)

    public virtual CompanyJob? Job { get; set; }
}

// ── Required Education for the Job ───────────────────────────────────────────
public class CompanyJobEducation
{
    public Guid    Id         { get; set; }
    public Guid    JobId      { get; set; }   // DB column: Job
    public string? Major      { get; set; }
    public short   Importance { get; set; }
    public byte[]? TimeStamp  { get; set; }   // DB column: Time_Stamp (rowversion)

    public virtual CompanyJob? Job { get; set; }
}

// ── Required Skills for the Job ───────────────────────────────────────────────
public class CompanyJobSkill
{
    public Guid    Id         { get; set; }
    public Guid    JobId      { get; set; }   // DB column: Job
    public string? Skill      { get; set; }
    public string? SkillLevel { get; set; }   // DB column: Skill_Level
    public int     Importance { get; set; }
    public byte[]? TimeStamp  { get; set; }   // DB column: Time_Stamp (rowversion)

    public virtual CompanyJob? Job { get; set; }
}
