namespace CareerHub.API.Domain
{
    public class CompanyProfile
    {
        public Guid Id { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactName { get; set; }
        public byte[]? CompanyLogo { get; set; }

        // Navigation Properties
        public virtual ICollection<CompanyDescription> Descriptions { get; set; } = [];
        public virtual ICollection<CompanyLocation> Locations { get; set; } = [];
        public virtual ICollection<CompanyJob> Jobs { get; set; } = [];
    }

    // Localized Description (supports multiple languages per company)
    public class CompanyDescription
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public required string LanguageId { get; set; } // FK to SystemLanguageCode

        public required string CompanyName { get; set; }
        public required string Description { get; set; } // Renamed from 'CompanyDescription' to avoid class name clash

        public virtual CompanyProfile? CompanyProfile { get; set; }
        public virtual SystemLanguageCode? LanguageCode { get; set; }
    }

    // A Job Posted by the Company
    public class CompanyJob
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }

        public DateTime ProfileCreated { get; set; }
        public bool IsInactive { get; set; }
        public bool IsCompanyHidden { get; set; }

        // Job Details (One-to-Many)
        public virtual ICollection<CompanyJobDescription> Descriptions { get; set; } = [];
        public virtual ICollection<CompanyJobEducation> Educations { get; set; } = [];
        public virtual ICollection<CompanyJobSkill> Skills { get; set; } = [];
        public virtual ICollection<ApplicantJobApplication> Applications { get; set; } = [];

        public virtual CompanyProfile? CompanyProfile { get; set; }
    }

    // Job Description (Localized)
    public class CompanyJobDescription
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }

        public required string JobName { get; set; }
        public required string Description { get; set; } // Renamed from 'JobDescriptions'

        public virtual CompanyJob? Job { get; set; }
    }

    // Required Education for the Job
    public class CompanyJobEducation
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }

        public required string Major { get; set; }
        public short Importance { get; set; }

        public virtual CompanyJob? Job { get; set; }
    }

    // Required Skills for the Job
    public class CompanyJobSkill
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }

        public required string Skill { get; set; }
        public required string SkillLevel { get; set; }
        public int Importance { get; set; }

        public virtual CompanyJob? Job { get; set; }
    }

    // Company Locations
    public class CompanyLocation
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }

        public required string CountryCode { get; set; }
        public string? Province { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }

        public virtual CompanyProfile? CompanyProfile { get; set; }
        // Optional: Navigation to SystemCountryCode if you want strict validation
    }
}
