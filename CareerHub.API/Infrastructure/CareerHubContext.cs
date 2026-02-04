using CareerHub.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.API.Infrastructure
{
    public class CareerHubContext(DbContextOptions<CareerHubContext> options) : DbContext(options)
    {
        // =========================================================
        // 1. Register the Domain Entities (formerly POCOs)
        // =========================================================

        // System
        public DbSet<SystemCountryCode> SystemCountryCodes { get; set; }
        public DbSet<SystemLanguageCode> SystemLanguageCodes { get; set; }

        // Applicant
        public DbSet<ApplicantProfile> ApplicantProfiles { get; set; }
        public DbSet<ApplicantEducation> ApplicantEducations { get; set; }
        public DbSet<ApplicantJobApplication> ApplicantJobApplications { get; set; }
        public DbSet<ApplicantResume> ApplicantResumes { get; set; }
        public DbSet<ApplicantSkill> ApplicantSkills { get; set; }
        public DbSet<ApplicantWorkHistory> ApplicantWorkHistory { get; set; }

        // Company
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<CompanyDescription> CompanyDescriptions { get; set; }
        public DbSet<CompanyJob> CompanyJobs { get; set; }
        public DbSet<CompanyJobDescription> CompanyJobDescriptions { get; set; }
        public DbSet<CompanyJobEducation> CompanyJobEducations { get; set; }
        public DbSet<CompanyJobSkill> CompanyJobSkills { get; set; }
        public DbSet<CompanyLocation> CompanyLocations { get; set; }

        // Security (Legacy Tables)
        public DbSet<SecurityLogin> SecurityLogins { get; set; }
        public DbSet<SecurityLoginsLog> SecurityLoginsLogs { get; set; }
        public DbSet<SecurityRole> SecurityRoles { get; set; }
        public DbSet<SecurityLoginsRole> SecurityLoginsRoles { get; set; }


        // =========================================================
        // 2. Configure the Mapping (Fluent API)
        // =========================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- SYSTEM ---
            modelBuilder.Entity<SystemCountryCode>().HasKey(e => e.Code);
            modelBuilder.Entity<SystemLanguageCode>().HasKey(e => e.LanguageID);

            // --- APPLICANT ---
            modelBuilder.Entity<ApplicantProfile>(e => {
                e.ToTable("Applicant_Profiles");
                e.Property(p => p.CountryCode).HasColumnName("Country_Code");
                e.Property(p => p.CurrentRate).HasColumnName("Current_Rate");
                e.Property(p => p.CurrentSalary).HasColumnName("Current_Salary");
                e.Property(p => p.PostalCode).HasColumnName("Zip_Postal_Code");
                // Map the FK 'LoginId' (C#) to the column 'Login' (DB)
                e.Property(p => p.LoginId).HasColumnName("Login");

                // Relationships
                e.HasOne(p => p.Country).WithMany(c => c.ApplicantProfiles)
                 .HasForeignKey(p => p.CountryCode);
                e.HasOne(p => p.Login).WithMany(l => l.ApplicantProfiles)
                 .HasForeignKey(p => p.LoginId);
            });

            modelBuilder.Entity<ApplicantEducation>(e => {
                e.ToTable("Applicant_Educations");
                e.Property(p => p.ApplicantId).HasColumnName("Applicant");
                e.Property(p => p.StartDate).HasColumnName("Start_Date");
                e.Property(p => p.CompletionDate).HasColumnName("Completion_Date");
                e.Property(p => p.CertificateDiploma).HasColumnName("Certificate_Diploma");

                e.HasOne(p => p.ApplicantProfile).WithMany(a => a.Educations)
                 .HasForeignKey(p => p.ApplicantId);
            });

            modelBuilder.Entity<ApplicantJobApplication>(e => {
                e.ToTable("Applicant_Job_Applications");
                e.Property(p => p.ApplicantId).HasColumnName("Applicant");
                e.Property(p => p.JobId).HasColumnName("Job");
                e.Property(p => p.ApplicationDate).HasColumnName("Application_Date");
            });

            modelBuilder.Entity<ApplicantResume>(e => {
                e.ToTable("Applicant_Resumes");
                e.Property(p => p.ApplicantId).HasColumnName("Applicant");
                e.Property(p => p.ResumeContent).HasColumnName("Resume"); // Mapped C# 'ResumeContent' to DB 'Resume'
                e.Property(p => p.LastUpdated).HasColumnName("Last_Updated");
            });

            modelBuilder.Entity<ApplicantSkill>(e => {
                e.ToTable("Applicant_Skills");
                e.Property(p => p.ApplicantId).HasColumnName("Applicant");
                e.Property(p => p.SkillLevel).HasColumnName("Skill_Level");
                e.Property(p => p.StartMonth).HasColumnName("Start_Month");
                e.Property(p => p.StartYear).HasColumnName("Start_Year");
                e.Property(p => p.EndMonth).HasColumnName("End_Month");
                e.Property(p => p.EndYear).HasColumnName("End_Year");
            });

            modelBuilder.Entity<ApplicantWorkHistory>(e => {
                e.ToTable("Applicant_Work_History");
                e.Property(p => p.ApplicantId).HasColumnName("Applicant");
                e.Property(p => p.CountryCode).HasColumnName("Country_Code");
                e.Property(p => p.CompanyName).HasColumnName("Company_Name");
                e.Property(p => p.JobTitle).HasColumnName("Job_Title");
                e.Property(p => p.JobDescription).HasColumnName("Job_Description");
                e.Property(p => p.StartMonth).HasColumnName("Start_Month");
                e.Property(p => p.EndMonth).HasColumnName("End_Month");
            });

            // --- COMPANY ---
            modelBuilder.Entity<CompanyProfile>(e => {
                e.ToTable("Company_Profiles");
                e.Property(p => p.RegistrationDate).HasColumnName("Registration_Date");
                e.Property(p => p.CompanyWebsite).HasColumnName("Company_Website");
                e.Property(p => p.ContactPhone).HasColumnName("Contact_Phone");
            });

            modelBuilder.Entity<CompanyJob>(e => {
                e.ToTable("Company_Jobs");
                e.Property(p => p.CompanyId).HasColumnName("Company");
                e.Property(p => p.IsInactive).HasColumnName("Is_Inactive");
                e.Property(p => p.IsCompanyHidden).HasColumnName("Is_Company_Hidden");
                e.Property(p => p.ProfileCreated).HasColumnName("Profile_Created");
            });

            // --- SECURITY ---
            modelBuilder.Entity<SecurityLogin>(e => {
                e.ToTable("Security_Logins");
                e.Property(p => p.Created).HasColumnName("Created_Date");
                e.Property(p => p.EmailAddress).HasColumnName("Email_Address");
                e.Property(p => p.PhoneNumber).HasColumnName("Phone_Number");
                e.Property(p => p.FullName).HasColumnName("Full_Name");
                e.Property(p => p.ForceChangePassword).HasColumnName("Force_Change_Password");
                e.Property(p => p.PrefferredLanguage).HasColumnName("Prefferred_Language");
            });
        }
    }
}
