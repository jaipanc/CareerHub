using CareerHub.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.API.Infrastructure;

public class CareerHubContext(DbContextOptions<CareerHubContext> options) : DbContext(options)
{
    // =========================================================================
    // DbSets
    // =========================================================================

    // System reference data
    public DbSet<SystemCountryCode>   SystemCountryCodes   { get; set; }
    public DbSet<SystemLanguageCode>  SystemLanguageCodes  { get; set; }

    // Applicant domain
    public DbSet<ApplicantProfile>        ApplicantProfiles        { get; set; }
    public DbSet<ApplicantEducation>      ApplicantEducations      { get; set; }
    public DbSet<ApplicantJobApplication> ApplicantJobApplications { get; set; }
    public DbSet<ApplicantResume>         ApplicantResumes         { get; set; }
    public DbSet<ApplicantSkill>          ApplicantSkills          { get; set; }
    public DbSet<ApplicantWorkHistory>    ApplicantWorkHistory     { get; set; }

    // Company domain
    public DbSet<CompanyProfile>        CompanyProfiles        { get; set; }
    public DbSet<CompanyDescription>    CompanyDescriptions    { get; set; }
    public DbSet<CompanyJob>            CompanyJobs            { get; set; }
    public DbSet<CompanyJobDescription> CompanyJobDescriptions { get; set; }
    public DbSet<CompanyJobEducation>   CompanyJobEducations   { get; set; }
    public DbSet<CompanyJobSkill>       CompanyJobSkills       { get; set; }
    public DbSet<CompanyLocation>       CompanyLocations       { get; set; }

    // Security domain
    public DbSet<SecurityLogin>       SecurityLogins       { get; set; }
    public DbSet<SecurityLoginsLog>   SecurityLoginsLogs   { get; set; }
    public DbSet<SecurityRole>        SecurityRoles        { get; set; }
    public DbSet<SecurityLoginsRole>  SecurityLoginsRoles  { get; set; }

    // =========================================================================
    // Model configuration — all column names map to the EXISTING DB schema.
    // DO NOT rename columns even where there are typos (e.g. "Prefferred_Language",
    // "Is_Succesful") — they are the real column names in JOB_PORTAL_DB.
    // =========================================================================
    protected override void OnModelCreating(ModelBuilder m)
    {
        base.OnModelCreating(m);

        // ── SYSTEM ──────────────────────────────────────────────────────────
        m.Entity<SystemCountryCode>(e =>
        {
            e.ToTable("System_Country_Codes");
            e.HasKey(p => p.Code);
        });

        m.Entity<SystemLanguageCode>(e =>
        {
            e.ToTable("System_Language_Codes");
            e.HasKey(p => p.LanguageID);
            e.Property(p => p.NativeName).HasColumnName("Native_Name");
        });

        // ── SECURITY ────────────────────────────────────────────────────────
        m.Entity<SecurityRole>(e =>
        {
            e.ToTable("Security_Roles");
            e.Property(p => p.IsInactive).HasColumnName("Is_Inactive");
        });

        m.Entity<SecurityLogin>(e =>
        {
            e.ToTable("Security_Logins");
            e.Property(p => p.Created).HasColumnName("Created_Date");
            e.Property(p => p.PasswordUpdate).HasColumnName("Password_Update_Date");
            e.Property(p => p.AgreementAccepted).HasColumnName("Agreement_Accepted_Date");
            e.Property(p => p.IsLocked).HasColumnName("Is_Locked");
            e.Property(p => p.IsInactive).HasColumnName("Is_Inactive");
            e.Property(p => p.EmailAddress).HasColumnName("Email_Address");
            e.Property(p => p.PhoneNumber).HasColumnName("Phone_Number");
            e.Property(p => p.FullName).HasColumnName("Full_Name");
            e.Property(p => p.ForceChangePassword).HasColumnName("Force_Change_Password");
            // NOTE: "Prefferred_Language" is the real DB column name (typo with double-f). Do not fix.
            e.Property(p => p.PrefferredLanguage).HasColumnName("Prefferred_Language");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();
        });

        m.Entity<SecurityLoginsLog>(e =>
        {
            e.ToTable("Security_Logins_Log");
            e.Property(p => p.LoginId).HasColumnName("Login");
            e.Property(p => p.SourceIP).HasColumnName("Source_IP");
            e.Property(p => p.LogonDate).HasColumnName("Logon_Date");
            // NOTE: "Is_Succesful" is the real DB column name (typo, single-s). Do not fix.
            e.Property(p => p.IsSuccesful).HasColumnName("Is_Succesful");

            e.HasOne(p => p.Login)
             .WithMany(l => l.LoginLogs)
             .HasForeignKey(p => p.LoginId)
             .IsRequired();
        });

        m.Entity<SecurityLoginsRole>(e =>
        {
            e.ToTable("Security_Logins_Roles");
            e.Property(p => p.LoginId).HasColumnName("Login");
            e.Property(p => p.RoleId).HasColumnName("Role");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();

            e.HasOne(p => p.Login)
             .WithMany(l => l.UserRoles)
             .HasForeignKey(p => p.LoginId)
             .IsRequired();

            e.HasOne(p => p.Role)
             .WithMany(r => r.UserRoles)
             .HasForeignKey(p => p.RoleId)
             .IsRequired();
        });

        // ── APPLICANT ───────────────────────────────────────────────────────
        m.Entity<ApplicantProfile>(e =>
        {
            e.ToTable("Applicant_Profiles");
            e.Property(p => p.LoginId).HasColumnName("Login");
            e.Property(p => p.CurrentSalary).HasColumnName("Current_Salary");
            e.Property(p => p.CurrentRate).HasColumnName("Current_Rate");
            e.Property(p => p.CountryCode).HasColumnName("Country_Code");
            e.Property(p => p.Province).HasColumnName("State_Province_Code");
            e.Property(p => p.Street).HasColumnName("Street_Address");
            e.Property(p => p.City).HasColumnName("City_Town");
            e.Property(p => p.PostalCode).HasColumnName("Zip_Postal_Code");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();

            e.HasOne(p => p.Login)
             .WithMany(l => l.ApplicantProfiles)
             .HasForeignKey(p => p.LoginId)
             .IsRequired();

            e.HasOne(p => p.Country)
             .WithMany(c => c.ApplicantProfiles)
             .HasForeignKey(p => p.CountryCode);
        });

        m.Entity<ApplicantEducation>(e =>
        {
            e.ToTable("Applicant_Educations");
            e.Property(p => p.ApplicantId).HasColumnName("Applicant");
            e.Property(p => p.CertificateDiploma).HasColumnName("Certificate_Diploma");
            e.Property(p => p.StartDate).HasColumnName("Start_Date");
            e.Property(p => p.CompletionDate).HasColumnName("Completion_Date");
            e.Property(p => p.CompletionPercent).HasColumnName("Completion_Percent");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();

            e.HasOne(p => p.ApplicantProfile)
             .WithMany(a => a.Educations)
             .HasForeignKey(p => p.ApplicantId)
             .IsRequired();
        });

        m.Entity<ApplicantSkill>(e =>
        {
            e.ToTable("Applicant_Skills");
            e.Property(p => p.ApplicantId).HasColumnName("Applicant");
            e.Property(p => p.SkillLevel).HasColumnName("Skill_Level");
            e.Property(p => p.StartMonth).HasColumnName("Start_Month");
            e.Property(p => p.StartYear).HasColumnName("Start_Year");
            e.Property(p => p.EndMonth).HasColumnName("End_Month");
            e.Property(p => p.EndYear).HasColumnName("End_Year");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();

            e.HasOne(p => p.ApplicantProfile)
             .WithMany(a => a.Skills)
             .HasForeignKey(p => p.ApplicantId)
             .IsRequired();
        });

        m.Entity<ApplicantResume>(e =>
        {
            e.ToTable("Applicant_Resumes");
            e.Property(p => p.ApplicantId).HasColumnName("Applicant");
            // DB column name is "Resume" — C# property is ResumeContent to avoid class name clash
            e.Property(p => p.ResumeContent).HasColumnName("Resume");
            e.Property(p => p.LastUpdated).HasColumnName("Last_Updated");

            e.HasOne(p => p.ApplicantProfile)
             .WithMany(a => a.Resumes)
             .HasForeignKey(p => p.ApplicantId)
             .IsRequired();
        });

        m.Entity<ApplicantJobApplication>(e =>
        {
            e.ToTable("Applicant_Job_Applications");
            e.Property(p => p.ApplicantId).HasColumnName("Applicant");
            e.Property(p => p.JobId).HasColumnName("Job");
            e.Property(p => p.ApplicationDate).HasColumnName("Application_Date");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();

            e.HasOne(p => p.ApplicantProfile)
             .WithMany(a => a.Applications)
             .HasForeignKey(p => p.ApplicantId)
             .IsRequired();

            e.HasOne(p => p.Job)
             .WithMany(j => j.Applications)
             .HasForeignKey(p => p.JobId)
             .IsRequired();
        });

        m.Entity<ApplicantWorkHistory>(e =>
        {
            e.ToTable("Applicant_Work_History");
            e.Property(p => p.ApplicantId).HasColumnName("Applicant");
            e.Property(p => p.CompanyName).HasColumnName("Company_Name");
            e.Property(p => p.CountryCode).HasColumnName("Country_Code");
            e.Property(p => p.JobTitle).HasColumnName("Job_Title");
            e.Property(p => p.JobDescription).HasColumnName("Job_Description");
            e.Property(p => p.StartMonth).HasColumnName("Start_Month");
            e.Property(p => p.StartYear).HasColumnName("Start_Year");
            e.Property(p => p.EndMonth).HasColumnName("End_Month");
            e.Property(p => p.EndYear).HasColumnName("End_Year");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();

            e.HasOne(p => p.ApplicantProfile)
             .WithMany(a => a.WorkHistory)
             .HasForeignKey(p => p.ApplicantId)
             .IsRequired();

            e.HasOne(p => p.Country)
             .WithMany(c => c.WorkHistories)
             .HasForeignKey(p => p.CountryCode)
             .IsRequired();
        });

        // ── COMPANY ─────────────────────────────────────────────────────────
        m.Entity<CompanyProfile>(e =>
        {
            e.ToTable("Company_Profiles");
            e.Property(p => p.RegistrationDate).HasColumnName("Registration_Date");
            e.Property(p => p.CompanyWebsite).HasColumnName("Company_Website");
            e.Property(p => p.ContactPhone).HasColumnName("Contact_Phone");
            e.Property(p => p.ContactName).HasColumnName("Contact_Name");
            e.Property(p => p.CompanyLogo).HasColumnName("Company_Logo");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();
        });

        m.Entity<CompanyDescription>(e =>
        {
            e.ToTable("Company_Descriptions");
            e.Property(p => p.CompanyId).HasColumnName("Company");
            e.Property(p => p.LanguageId).HasColumnName("LanguageId");
            e.Property(p => p.CompanyName).HasColumnName("Company_Name");
            // DB column is "Company_Description" — C# property is Description to avoid class name clash
            e.Property(p => p.Description).HasColumnName("Company_Description");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();

            e.HasOne(p => p.CompanyProfile)
             .WithMany(c => c.Descriptions)
             .HasForeignKey(p => p.CompanyId)
             .IsRequired();

            e.HasOne(p => p.LanguageCode)
             .WithMany()
             .HasForeignKey(p => p.LanguageId);
        });

        m.Entity<CompanyJob>(e =>
        {
            e.ToTable("Company_Jobs");
            e.Property(p => p.CompanyId).HasColumnName("Company");
            e.Property(p => p.ProfileCreated).HasColumnName("Profile_Created");
            e.Property(p => p.IsInactive).HasColumnName("Is_Inactive");
            e.Property(p => p.IsCompanyHidden).HasColumnName("Is_Company_Hidden");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();

            e.HasOne(p => p.CompanyProfile)
             .WithMany(c => c.Jobs)
             .HasForeignKey(p => p.CompanyId)
             .IsRequired();
        });

        m.Entity<CompanyJobDescription>(e =>
        {
            e.ToTable("Company_Jobs_Descriptions");
            e.Property(p => p.JobId).HasColumnName("Job");
            e.Property(p => p.JobName).HasColumnName("Job_Name");
            // DB column is "Job_Descriptions" — C# property is Description to avoid plural confusion
            e.Property(p => p.Description).HasColumnName("Job_Descriptions");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();

            e.HasOne(p => p.Job)
             .WithMany(j => j.Descriptions)
             .HasForeignKey(p => p.JobId)
             .IsRequired();
        });

        m.Entity<CompanyJobEducation>(e =>
        {
            e.ToTable("Company_Job_Educations");
            e.Property(p => p.JobId).HasColumnName("Job");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();

            e.HasOne(p => p.Job)
             .WithMany(j => j.Educations)
             .HasForeignKey(p => p.JobId)
             .IsRequired();
        });

        m.Entity<CompanyJobSkill>(e =>
        {
            e.ToTable("Company_Job_Skills");
            e.Property(p => p.JobId).HasColumnName("Job");
            e.Property(p => p.SkillLevel).HasColumnName("Skill_Level");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();

            e.HasOne(p => p.Job)
             .WithMany(j => j.Skills)
             .HasForeignKey(p => p.JobId)
             .IsRequired();
        });

        m.Entity<CompanyLocation>(e =>
        {
            e.ToTable("Company_Locations");
            e.Property(p => p.CompanyId).HasColumnName("Company");
            e.Property(p => p.CountryCode).HasColumnName("Country_Code");
            e.Property(p => p.Province).HasColumnName("State_Province_Code");
            e.Property(p => p.Street).HasColumnName("Street_Address");
            e.Property(p => p.City).HasColumnName("City_Town");
            e.Property(p => p.PostalCode).HasColumnName("Zip_Postal_Code");
            e.Property(p => p.TimeStamp).HasColumnName("Time_Stamp").IsRowVersion();

            e.HasOne(p => p.CompanyProfile)
             .WithMany(c => c.Locations)
             .HasForeignKey(p => p.CompanyId)
             .IsRequired();
        });
    }
}
