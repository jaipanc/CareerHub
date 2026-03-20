namespace CareerHub.API.Domain;

// ── Aggregate Root ────────────────────────────────────────────────────────────
public class ApplicantProfile
{
    public Guid     Id            { get; set; }
    public Guid     LoginId       { get; set; }   // DB column: Login
    public decimal? CurrentSalary { get; set; }
    public decimal? CurrentRate   { get; set; }
    public string?  Currency      { get; set; }
    public string?  CountryCode   { get; set; }   // DB column: Country_Code
    public string?  Province      { get; set; }   // DB column: State_Province_Code
    public string?  Street        { get; set; }   // DB column: Street_Address
    public string?  City          { get; set; }   // DB column: City_Town
    public string?  PostalCode    { get; set; }   // DB column: Zip_Postal_Code
    public byte[]?  TimeStamp     { get; set; }   // DB column: Time_Stamp (rowversion)

    // Navigation properties
    public virtual SecurityLogin?                          Login           { get; set; }
    public virtual SystemCountryCode?                      Country         { get; set; }
    public virtual ICollection<ApplicantEducation>         Educations      { get; set; } = [];
    public virtual ICollection<ApplicantJobApplication>    Applications    { get; set; } = [];
    public virtual ICollection<ApplicantSkill>             Skills          { get; set; } = [];
    public virtual ICollection<ApplicantWorkHistory>       WorkHistory     { get; set; } = [];
    public virtual ICollection<ApplicantResume>            Resumes         { get; set; } = [];
}

// ── Education ─────────────────────────────────────────────────────────────────
public class ApplicantEducation
{
    public Guid    Id                { get; set; }
    public Guid    ApplicantId       { get; set; }   // DB column: Applicant
    public string? Major             { get; set; }
    public string? CertificateDiploma{ get; set; }   // DB column: Certificate_Diploma
    public DateTime? StartDate       { get; set; }   // DB column: Start_Date
    public DateTime? CompletionDate  { get; set; }   // DB column: Completion_Date
    public byte?   CompletionPercent { get; set; }   // DB column: Completion_Percent
    public byte[]? TimeStamp         { get; set; }   // DB column: Time_Stamp (rowversion)

    public virtual ApplicantProfile? ApplicantProfile { get; set; }
}

// ── Skills ────────────────────────────────────────────────────────────────────
public class ApplicantSkill
{
    public Guid    Id          { get; set; }
    public Guid    ApplicantId { get; set; }   // DB column: Applicant
    public string? Skill       { get; set; }
    public string? SkillLevel  { get; set; }   // DB column: Skill_Level
    public byte    StartMonth  { get; set; }   // DB column: Start_Month  (byte, not string)
    public int     StartYear   { get; set; }   // DB column: Start_Year
    public byte    EndMonth    { get; set; }   // DB column: End_Month
    public int     EndYear     { get; set; }   // DB column: End_Year
    public byte[]? TimeStamp   { get; set; }   // DB column: Time_Stamp (rowversion)

    public virtual ApplicantProfile? ApplicantProfile { get; set; }
}

// ── Work History ──────────────────────────────────────────────────────────────
public class ApplicantWorkHistory
{
    public Guid    Id             { get; set; }
    public Guid    ApplicantId   { get; set; }   // DB column: Applicant
    public string? CompanyName   { get; set; }   // DB column: Company_Name
    public string? CountryCode   { get; set; }   // DB column: Country_Code
    public string? Location      { get; set; }
    public string? JobTitle      { get; set; }   // DB column: Job_Title
    public string? JobDescription{ get; set; }   // DB column: Job_Description
    public short   StartMonth    { get; set; }   // DB column: Start_Month
    public int     StartYear     { get; set; }   // DB column: Start_Year
    public short   EndMonth      { get; set; }   // DB column: End_Month
    public int     EndYear       { get; set; }   // DB column: End_Year
    public byte[]? TimeStamp     { get; set; }   // DB column: Time_Stamp (rowversion)

    public virtual ApplicantProfile?    ApplicantProfile { get; set; }
    public virtual SystemCountryCode?   Country          { get; set; }
}

// ── Resume ────────────────────────────────────────────────────────────────────
public class ApplicantResume
{
    public Guid      Id            { get; set; }
    public Guid      ApplicantId   { get; set; }   // DB column: Applicant
    public string?   ResumeContent { get; set; }   // DB column: Resume
    public DateTime? LastUpdated   { get; set; }   // DB column: Last_Updated

    public virtual ApplicantProfile? ApplicantProfile { get; set; }
}

// ── Job Application (linking table) ──────────────────────────────────────────
public class ApplicantJobApplication
{
    public Guid     Id              { get; set; }
    public Guid     ApplicantId     { get; set; }   // DB column: Applicant
    public Guid     JobId           { get; set; }   // DB column: Job
    public DateTime ApplicationDate { get; set; }   // DB column: Application_Date
    public byte[]?  TimeStamp       { get; set; }   // DB column: Time_Stamp (rowversion)

    public virtual ApplicantProfile? ApplicantProfile { get; set; }
    public virtual CompanyJob?       Job              { get; set; }
}
