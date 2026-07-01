var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .AddDatabase("sqldata");

builder.AddProject<Projects.Api>("api")
       .WithReference(sql);

builder.Build().Run();