using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
var postgres = builder.AddPostgres("postgres");
if (builder.Configuration.GetValue("UseVolumes", true))
{
    postgres.WithDataVolume();
}

var clinic = postgres.AddDatabase("clinic");
builder.AddProject<Projects.VeterinaryClinic_Api>("api")
    .WithEndpoint("http", endpoint => endpoint.Port = null)
    .WithReference(clinic)
    .WaitFor(clinic)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
