
using CareerHub.API.Infrastructure;
using FluentValidation;
using Scalar.AspNetCore;

namespace CareerHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =========================================================================
            // 1. ADD SERVICES
            // =========================================================================

            // Add Aspire Service Defaults (Telemetry, Health Checks)
            builder.AddServiceDefaults();

            // Add Database
            // This automatically reads the connection string from Aspire or appsettings.json
            builder.AddSqlServerDbContext<CareerHubContext>("careercloud-db");

            // Add OpenAPI / Documentation (Replaces Swagger)
            builder.Services.AddOpenApi();

            // Add Validation
            // Scans the current assembly for any class inheriting from AbstractValidator<>
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // Add Authorization (Optional, but good to have ready)
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // =========================================================================
            // 2. CONFIGURE PIPELINE
            // =========================================================================

            // Enable Aspire endpoints (/health, /alive)
            app.MapDefaultEndpoints();

            // Configure OpenAPI / Scalar UI
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi(); // Generates the JSON document
                app.MapScalarApiReference(); // The UI at /scalar/v1
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // =========================================================================
            // 3. MAP ENDPOINTS (Vertical Slices)
            // =========================================================================

            // Create a versioned group for all API endpoints
            var api = app.MapGroup("/api");

            // --- Applicants Feature ---
            var applicants = api.MapGroup("/applicants").WithTags("Applicants");

            // Register your Slices here
            // Note: Ensure you have created these classes, or comment them out until you do
            // RegisterEndpoint.Map(applicants);
            // AddEducationEndpoint.Map(applicants);


            // --- System Feature (Reference Data) ---
            var system = api.MapGroup("/system").WithTags("System");
            //GetCountriesEndpoint.Map(system);


            // --- Company Feature ---
            var company = api.MapGroup("/company").WithTags("Company");
            // AddCompanyEndpoint.Map(company);

            app.Run();
        }
    }
}
