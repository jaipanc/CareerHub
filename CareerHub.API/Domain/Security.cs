namespace CareerHub.API.Domain;

// ── Security Login (Auth root) ────────────────────────────────────────────────
public class SecurityLogin
{
    public Guid      Id                  { get; set; }
    public string?   Login               { get; set; }
    public string?   Password            { get; set; }           // SHA-512 hash stored in DB
    public DateTime  Created             { get; set; }           // DB column: Created_Date
    public DateTime? PasswordUpdate      { get; set; }           // DB column: Password_Update_Date
    public DateTime? AgreementAccepted   { get; set; }           // DB column: Agreement_Accepted_Date
    public bool      IsLocked            { get; set; }           // DB column: Is_Locked
    public bool      IsInactive          { get; set; }           // DB column: Is_Inactive
    public string?   EmailAddress        { get; set; }           // DB column: Email_Address
    public string?   PhoneNumber         { get; set; }           // DB column: Phone_Number
    public string?   FullName            { get; set; }           // DB column: Full_Name
    public bool      ForceChangePassword { get; set; }           // DB column: Force_Change_Password
    // NOTE: "Prefferred_Language" is the real DB column name — double-f is a typo in the original
    // schema. DO NOT rename this property or the column mapping will break.
    public string?   PrefferredLanguage  { get; set; }           // DB column: Prefferred_Language
    public byte[]?   TimeStamp           { get; set; }           // DB column: Time_Stamp (rowversion)

    public virtual ICollection<SecurityLoginsLog>   LoginLogs        { get; set; } = [];
    public virtual ICollection<SecurityLoginsRole>  UserRoles        { get; set; } = [];
    public virtual ICollection<ApplicantProfile>    ApplicantProfiles{ get; set; } = [];
}

// ── Security Login Log (audit trail) ─────────────────────────────────────────
public class SecurityLoginsLog
{
    public Guid     Id         { get; set; }
    public Guid     LoginId    { get; set; }   // DB column: Login
    public string?  SourceIP   { get; set; }   // DB column: Source_IP
    public DateTime LogonDate  { get; set; }   // DB column: Logon_Date
    // NOTE: "Is_Succesful" is the real DB column name — single-s is a typo in the original schema.
    // DO NOT rename this property or the column mapping will break.
    public bool     IsSuccesful{ get; set; }   // DB column: Is_Succesful

    public virtual SecurityLogin? Login { get; set; }
}

// ── Security Role ─────────────────────────────────────────────────────────────
public class SecurityRole
{
    public Guid    Id         { get; set; }
    public string? Role       { get; set; }
    public bool    IsInactive { get; set; }   // DB column: Is_Inactive

    public virtual ICollection<SecurityLoginsRole> UserRoles { get; set; } = [];
}

// ── Login ↔ Role (linking table) ─────────────────────────────────────────────
public class SecurityLoginsRole
{
    public Guid    Id        { get; set; }
    public Guid    LoginId   { get; set; }   // DB column: Login
    public Guid    RoleId    { get; set; }   // DB column: Role
    public byte[]? TimeStamp { get; set; }   // DB column: Time_Stamp (rowversion)

    public virtual SecurityLogin? Login { get; set; }
    public virtual SecurityRole?  Role  { get; set; }
}
