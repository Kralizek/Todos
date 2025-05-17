var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("DatabasePassword")
    .ExcludeFromManifest();

var username = builder.AddParameter("DatabaseUsername")
    .ExcludeFromManifest();

var db = builder.AddPostgres("db", password: password, userName: username, port: 54000)
    .WithImageTag("16")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("todo-data")
    .WithPgAdmin()
    .AddDatabase("database", databaseName: "todo");

var api = builder.AddProject<Projects.API>("api")
    .WithReference(db)
    .WaitFor(db);

var web = builder.AddNpmApp("web", "../../client")
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT", port: 3182)
    .WithEnvironment("BROWSER", "none");

builder.Build().Run();
