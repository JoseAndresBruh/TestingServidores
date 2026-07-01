using Microsoft.Extensions.Hosting;
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .AddDatabase("sqldata");

// USAR STRING DIRECTAMENTE evita depender de la generación automática de 'Projects.Api'
builder.AddProject<Projects.Api>("api")
       .WithReference(sql);

builder.Build().Run();