using System.Text;
using CareerHub.API.Features.Auth;
using CareerHub.API.Infrastructure;
using CareerHub.API.Infrastructure.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

namespace CareerHub.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // =====================================================================
        // 1. ASPIRE — telemetry, health checks, service discovery
        // =====================================================================
        builder.AddServiceDefaults();

        // =====================================================================
        // 2. DATABASE — EF Core via Aspire (reads "careercloud-db" from config)
        // =====================================================================
        builder.AddSqlServerDbContext<CareerHubContext>("careercloud-db");

        // =====================================================================
        // 3. AUTH — JWT bearer + strongly-typed config
        // =====================================================================
        builder.Services
               .AddOptions<JwtSettings>()
               .BindConfiguration(JwtSettings.SectionName)
               .ValidateDataAnnotations()
               .ValidateOnStart();

        var jwtSettings = builder.Configuration
                                 .GetSection(JwtSettings.SectionName)
                                 .Get<JwtSettings>()
                          ?? throw new InvalidOperationException(
                                 "Jwt config section is missing from appsettings.");

        builder.Services
               .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(opts =>
               {
                   opts.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer           = true,
                       ValidateAudience         = true,
                       ValidateLifetime         = true,
                       ValidateIssuerSigningKey  = true,
                       ValidIssuer              = jwtSettings.Issuer,
                       ValidAudience            = jwtSettings.Audience,
                       IssuerSigningKey         = new SymmetricSecurityKey(
                                                      Encoding.UTF8.GetBytes(jwtSettings.Key)),
                       ClockSkew                = TimeSpan.FromSeconds(30),
                   };
               });

        builder.Services.AddAuthorization();

        // Register auth service
        builder.Services.AddScoped<IAuthService, AuthService>();

        // =====================================================================
        // 4. VALIDATION — FluentValidation (scans this assembly)
        // =====================================================================
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        // =====================================================================
        // 5. API DOCS — OpenAPI (built-in .NET 10) + Scalar UI
        // =====================================================================
        builder.Services.AddOpenApi(opts =>
        {
            opts.AddDocumentTransformer((doc, _, _) =>
            {
                doc.Info.Title   = "CareerCloud API";
                doc.Info.Version = "v1";
                doc.Info.Description =
                    "CareerCloud job portal — migrated from .NET Framework 4.8 to .NET 10. " +
                    "Authenticate via POST /api/auth/login to receive a JWT bearer token.";
                return Task.CompletedTask;
            });
        });

        // =====================================================================
        // 6. BUILD
        // =====================================================================
        var app = builder.Build();

        // =====================================================================
        // 7. PIPELINE
        // =====================================================================
        app.MapDefaultEndpoints();   // /health  /alive  (Aspire)

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();                // /openapi/v1.json
            app.MapScalarApiReference();     // /scalar/v1
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        // =====================================================================
        // 8. ROUTE GROUPS
        // =====================================================================

        var api = app.MapGroup("/api");

        // ── Auth (public — no [Authorize] on group) ───────────────────────────
        var auth = api.MapGroup("/auth")
                      .WithTags("Auth");
        AuthEndpoints.Map(auth);

        // ── Applicant ─────────────────────────────────────────────────────────
        var applicants = api.MapGroup("/careercloud/applicant/v1")
                            .WithTags("Applicant")
                            .RequireAuthorization();

        // Phase 4 — register slice endpoints here:
        // EducationEndpoints.Map(applicants);
        // SkillEndpoints.Map(applicants);
        // ProfileEndpoints.Map(applicants);
        // ResumeEndpoints.Map(applicants);
        // WorkHistoryEndpoints.Map(applicants);
        // JobApplicationEndpoints.Map(applicants);

        // ── Company ───────────────────────────────────────────────────────────
        var company = api.MapGroup("/careercloud/company/v1")
                         .WithTags("Company")
                         .RequireAuthorization();

        // Phase 4:
        // CompanyProfileEndpoints.Map(company);
        // CompanyDescriptionEndpoints.Map(company);
        // CompanyLocationEndpoints.Map(company);
        // CompanyJobEndpoints.Map(company);
        // CompanyJobDescriptionEndpoints.Map(company);
        // CompanyJobEducationEndpoints.Map(company);
        // CompanyJobSkillEndpoints.Map(company);

        // ── Security ──────────────────────────────────────────────────────────
        var security = api.MapGroup("/careercloud/security/v1")
                          .WithTags("Security")
                          .RequireAuthorization();

        // Phase 4:
        // SecurityLoginEndpoints.Map(security);
        // SecurityLoginsLogEndpoints.Map(security);
        // SecurityLoginsRoleEndpoints.Map(security);
        // SecurityRoleEndpoints.Map(security);

        // ── System ────────────────────────────────────────────────────────────
        var system = api.MapGroup("/careercloud/system/v1")
                        .WithTags("System")
                        .RequireAuthorization();

        // Phase 4:
        // CountryCodeEndpoints.Map(system);
        // LanguageCodeEndpoints.Map(system);

        // =====================================================================
        app.Run();
    }
}
