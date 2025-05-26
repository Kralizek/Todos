using System.Diagnostics.CodeAnalysis;

using Microsoft.OpenApi.Models;

using Npgsql;

using Todos;
using Todos.Todo;

// Defines a name for a CORS (Cross-Origin Resource Sharing) policy.
// CORS policies are security features that control which external domains can access this API.
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

// This is the main setup for the web application.
// \'var\' is a C# keyword for implicitly typed local variables; the compiler infers the type.
var builder = WebApplication.CreateBuilder(args); // 'args' are command-line arguments.
                                                 // 'builder' is used to configure services and the application.

// Adds default services commonly used in .NET Aspire applications (e.g., health checks, telemetry).
builder.AddServiceDefaults();

// Configures CORS services.
builder.Services.AddCors(options =>
{
    // Adds a new CORS policy with the name defined above.
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy  =>
                      {
                          // This policy is very permissive: allows requests from any origin,
                          // with any HTTP header, and any HTTP method.
                          // In a production environment, this would be restricted to specific known origins.
                          policy.WithOrigins("*").AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });

});

// Adds services for problem details, which provide a standardized way to return error information from APIs.
builder.Services.AddProblemDetails();
// Adds services needed for API exploration, used by Swagger to generate API documentation.
builder.Services.AddEndpointsApiExplorer();
// Adds Swagger generation services. Swagger creates an interactive documentation UI for the API.
builder.Services.AddSwaggerGen(options =>
{
    // Configures a "document" for Swagger.
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Todo API", // Title of the API in the Swagger UI.
        Version = new Version(1, 0, 0).ToString() // Version of the API.
    });

    // Custom mapping for DateOnly type to ensure it's represented as a string in "date" format in Swagger.
    options.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date"
    });

    options.SupportNonNullableReferenceTypes(); // Enhances Swagger to better reflect C# nullable reference types.
    options.UseAllOfToExtendReferenceSchemas(); // Improves how Swagger handles inherited schemas.
});

// Registers AppDbContext (from AppDbContext.cs) with the dependency injection (DI) system.
// 'AddNpgsqlDbContext' configures AppDbContext to use PostgreSQL.
// "Database" is the name of the connection string that will be looked up in configuration (e.g., appsettings.json).
// '<AppDbContext>' specifies the type of the DbContext to register.
// This makes AppDbContext available to other parts of the application, like repositories.
builder.AddNpgsqlDbContext<AppDbContext>("Database");

// Adds and configures OpenTelemetry for distributed tracing.
// 'AddNpgsql()' includes tracing for database calls made with Npgsql.
builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddNpgsql());

// Registers ITodoRepository and its implementation EntityFrameworkTodoRepository with the DI system.
// 'AddTransient' means a new instance of EntityFrameworkTodoRepository is created each time ITodoRepository is requested.
// This is key for the Repository Pattern: other parts of the code will ask for ITodoRepository
// and DI will provide EntityFrameworkTodoRepository, decoupling the data access logic.
builder.Services.AddTransient<ITodoRepository, EntityFrameworkTodoRepository>();

// Builds the application instance using the configured services and settings.
var app = builder.Build();

// Configures the HTTP request processing pipeline.
// The order of middleware (app.Use...) is important here.

// Checks if the current environment is "Development".
if (app.Environment.IsDevelopment())
{
    // In development, use the permissive CORS policy defined earlier.
    app.UseCors(myAllowSpecificOrigins);
    // Enables the Swagger middleware, which serves the generated OpenAPI specification JSON file.
    // 'RouteTemplate' customizes the URL for the OpenAPI JSON.
    app.UseSwagger(o => o.RouteTemplate = "schema/{documentName}/openapi.json");
    
    // Enables the Swagger UI middleware, which provides an interactive HTML page to explore the API.
    app.UseSwaggerUI(o =>
    {
        // Points the Swagger UI to the OpenAPI JSON endpoint.
        o.SwaggerEndpoint("/schema/v1/openapi.json", "Todo API");
    });

    // For convenience in development, redirects the root path ("/") to the Swagger UI.
    app.MapGet("/", () => Results.LocalRedirect("/swagger"))
        .ExcludeFromDescription(); // Excludes this endpoint from the Swagger documentation itself.
}

// Calls an extension method (defined in Todo/Endpoints.cs) to map all Todo-related API endpoints.
// This helps keep Program.cs clean by organizing endpoint definitions elsewhere.
app.AddTodoEndpoints();

// This block ensures the database schema is created if it doesn't already exist when the app starts.
// 'app.Services.CreateScope()' creates a new DI scope to resolve services.
using (var scope = app.Services.CreateScope())
{
    // 'GetRequiredService<AppDbContext>()' retrieves an instance of AppDbContext from the DI container.
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // 'EnsureCreatedAsync()' creates the database and schema based on AppDbContext's configuration (entities, mappings).
    // This is useful for development and testing, but for production, migrations are usually preferred.
    await db.Database.EnsureCreatedAsync();
}

// Maps default endpoints provided by .NET Aspire (e.g., for health checks).
app.MapDefaultEndpoints();

// Runs the application, causing it to start listening for incoming HTTP requests.
app.Run();

// This partial class declaration is primarily for enabling tests to access Program.cs internals
// if needed, using 'InternalsVisibleTo' attribute in the .csproj file.
// '[ExcludeFromCodeCoverage]' attribute tells code coverage tools to ignore this part.
[ExcludeFromCodeCoverage(Justification = "Program")]
public partial class Program { }