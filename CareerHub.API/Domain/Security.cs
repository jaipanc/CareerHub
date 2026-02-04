namespace CareerHub.API.Domain
{
    public class SecurityLogin
    {
        public Guid Id { get; set; }
        public required string Login { get; set; }
        public required string Password { get; set; }
        public DateTime Created { get; set; }
        public DateTime? PasswordUpdate { get; set; }
        public DateTime? AgreementAccepted { get; set; }
        public bool IsLocked { get; set; }
        public bool IsInactive { get; set; }
        public required string EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public bool ForceChangePassword { get; set; }
        public string? PrefferredLanguage { get; set; }

        // Navigation
        public virtual ICollection<SecurityLoginsLog> LoginLogs { get; set; } = [];
        public virtual ICollection<SecurityLoginsRole> UserRoles { get; set; } = [];
        public virtual ICollection<ApplicantProfile> ApplicantProfiles { get; set; } = [];
    }

    public class SecurityLoginsLog
    {
        public Guid Id { get; set; }
        public Guid LoginId { get; set; }
        public required string SourceIP { get; set; }
        public DateTime LogonDate { get; set; }
        public bool IsSuccesful { get; set; }

        public virtual SecurityLogin? Login { get; set; }
    }

    public class SecurityRole
    {
        public Guid Id { get; set; }
        public required string Role { get; set; }
        public bool IsInactive { get; set; }

        public virtual ICollection<SecurityLoginsRole> UserRoles { get; set; } = [];
    }

    // Linking Table (User <-> Role)
    public class SecurityLoginsRole
    {
        public Guid Id { get; set; }
        public Guid LoginId { get; set; }
        public Guid RoleId { get; set; }

        public virtual SecurityLogin? Login { get; set; }
        public virtual SecurityRole? Role { get; set; }
    }
}
