namespace CareerHub.API.Domain
{
    // The Aggregate Root
    public class ApplicantProfile
    {
        public Guid Id { get; set; }

        // 1. The Foreign Key (Raw Data)
        public Guid LoginId { get; set; }

        // 2. The Navigation Property (The Object) <-- THIS WAS MISSING
        public virtual SecurityLogin? Login { get; set; }

        public decimal? CurrentSalary { get; set; }
        public decimal? CurrentRate { get; set; }
        public string? Currency { get; set; }

        // Address Info
        public string? CountryCode { get; set; }
        public string? Province { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }

        // Other Navigations
        public virtual ICollection<ApplicantEducation> Educations { get; set; } = [];
        public virtual ICollection<ApplicantJobApplication> Applications { get; set; } = [];
        public virtual ICollection<ApplicantSkill> Skills { get; set; } = [];
        public virtual ICollection<ApplicantWorkHistory> WorkHistory { get; set; } = [];
        public virtual ICollection<ApplicantResume> Resumes { get; set; } = [];

        public virtual SystemCountryCode? Country { get; set; }
    }

    // Child Entity: Skills
    public class ApplicantSkill
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }

        public required string Skill { get; set; }
        public required string SkillLevel { get; set; }
        public string? StartMonth { get; set; } // Changed to string or byte based on your pref (Poco was byte)
        public int StartYear { get; set; }
        public string? EndMonth { get; set; }
        public int EndYear { get; set; }

        // Navigation Back
        public virtual ApplicantProfile? ApplicantProfile { get; set; }
    }

    // Child Entity: Resume
    public class ApplicantResume
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }

        public required string ResumeContent { get; set; } // Renamed from 'Resume' to avoid confusion
        public DateTime? LastUpdated { get; set; }

        public virtual ApplicantProfile? ApplicantProfile { get; set; }
    }

    // Child Entity: Job Application (Linking Table)
    public class ApplicantJobApplication
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }
        public Guid JobId { get; set; } // Links to CompanyJob

        public DateTime ApplicationDate { get; set; }

        public virtual ApplicantProfile? ApplicantProfile { get; set; }
        public virtual CompanyJob? Job { get; set; }
    }

    // Child Entity: Work History
    public class ApplicantWorkHistory
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }

        public required string CompanyName { get; set; }
        public required string CountryCode { get; set; } // FK
        public required string Location { get; set; }
        public required string JobTitle { get; set; }
        public required string JobDescription { get; set; }

        public short StartMonth { get; set; }
        public int StartYear { get; set; }
        public short EndMonth { get; set; }
        public int EndYear { get; set; }

        public virtual ApplicantProfile? ApplicantProfile { get; set; }
        public virtual SystemCountryCode? Country { get; set; }
    }

    // Child Entity: Education (Assuming from previous context)
    public class ApplicantEducation
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }

        public required string Major { get; set; }
        public string? CertificateDiploma { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public byte? CompletionPercent { get; set; }

        public virtual ApplicantProfile? ApplicantProfile { get; set; }
    }
}
