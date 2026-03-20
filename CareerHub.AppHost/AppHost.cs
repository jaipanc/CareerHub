var builder = DistributedApplication.CreateBuilder(args);

// ── Option A (current — development against existing local JOB_PORTAL_DB) ────
//
// Use a connection string reference instead of spinning up a Docker container.
// This tells Aspire to inject the connection string from appsettings.json into
// the API project at runtime, WITHOUT replacing it with a new empty container.
//
// The connection string key "careercloud-db" must match:
//   - builder.AddSqlServerDbContext<CareerHubContext>("careercloud-db")  in Program.cs
//   - ConnectionStrings:careercloud-db  in CareerHub.API/appsettings.json

var existingDb = builder.AddConnectionString("careercloud-db");

builder.AddProject<Projects.CareerHub_API>("careerhub-api")
       .WithReference(existingDb);

// ── Option B (future — containerised SQL Server with fresh DB) ────────────────
// Uncomment below and comment out Option A when you are ready to provision a
// fresh Docker SQL Server (e.g. in CI or when deploying to a new environment).
// You will also need to run EF migrations against the new container.
//
// var sql = builder.AddSqlServer("sql")
//                  .AddDatabase("careercloud-db");
//
// builder.AddProject<Projects.CareerHub_API>("careerhub-api")
//        .WithReference(sql)
//        .WaitFor(sql);

builder.Build().Run();