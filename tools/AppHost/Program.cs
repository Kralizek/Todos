var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.API>("api");

var web = builder.AddNpmApp("web", "../../client")
    .WithHttpEndpoint(env: "PORT", port: 3182)
    .WithEnvironment("BROWSER", "none");

builder.Build().Run();
