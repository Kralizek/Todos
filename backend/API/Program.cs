using Microsoft.OpenApi.Models;

using Todos;
using Todos.Todo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Todo API",
        Version = new Version(1, 0, 0).ToString()
    });

    options.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date"
    });

    options.SupportNonNullableReferenceTypes();
    options.UseAllOfToExtendReferenceSchemas();
});

builder.Services.AddSqlite<TodoDbContext>("Filename=.db/Todos.db");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(o => o.RouteTemplate = "schema/{documentName}/openapi.json");
    
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/schema/v1/openapi.json", "Todo API");
    });

    app.MapGet("/", () => Results.LocalRedirect("/swagger"))
        .ExcludeFromDescription();
}

app.AddTodoEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

    await db.Database.EnsureCreatedAsync();
}

app.Run();
