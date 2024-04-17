using Microsoft.OpenApi.Models;

using Todos;
using Todos.Todo;
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("*").AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });

});

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

builder.Services.AddSqlite<AppDbContext>("Filename=.db/Todos.db");

builder.Services.AddTransient<ITodoRepository, EntityFrameworkTodoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors(MyAllowSpecificOrigins);
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
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.EnsureCreatedAsync();
}

app.Run();
