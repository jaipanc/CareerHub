var builder = DistributedApplication.CreateBuilder(args);

// SQL Server container — name must match builder.AddSqlServerDbContext<CareerHubContext>("careercloud-db") in API
var sql = builder.AddSqlServer("sql")
                 .AddDatabase("careercloud-db");

builder.AddProject<Projects.CareerHub_API>("careerhub-api")
       .WithReference(sql)
       .WaitFor(sql);

builder.Build().Run();
