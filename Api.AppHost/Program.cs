var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .AddDatabase("sqldata");

builder.AddProject("api", "../Api/Api.csproj")
       .WithReference(sql);

builder.Build().Run();