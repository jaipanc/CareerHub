var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var sql = builder.AddSqlServer("sql")
                 .AddDatabase("career-db");

builder.AddProject<Projects.CareerHub_API>("api")
       .WithReference(sql);

builder.Build().Run();
