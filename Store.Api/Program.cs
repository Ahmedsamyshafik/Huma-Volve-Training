using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Store.Api.Swagger;
using Store.Business.Services;
using Store.Data;
using Store.Data.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Serilog is configured entirely from the "Serilog" section of appsettings.json
// (ReadFrom.Configuration): a single File sink, minimum level Information, no console sink.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Replaces the default Microsoft.Extensions.Logging providers with Serilog, so everything
// logged through ILogger<T> - framework and application alike - lands in the log file.
builder.Host.UseSerilog();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Concrete, entity-specific repositories and services - deliberately no generic
// repository, no unit of work and no extra abstractions.
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();

builder.Services.AddControllers();

// URL-segment API versioning: /api/v1/..., /api/v2/...
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1.0);
        options.AssumeDefaultVersionWhenUnspecified = false;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// One Swagger document per API version (discovered through Asp.Versioning.Mvc.ApiExplorer).
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

var app = builder.Build();

// One log entry per HTTP request (method, path, status, elapsed time) written by Serilog.
app.UseSerilogRequestLogging();

// Apply pending migrations (schema + seed data) on startup so a fresh clone only needs
// `dotnet run`. Manual alternative: `dotnet ef database update --project Store.Data --startup-project Store.Api`.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.Services.GetRequiredService<IApiVersionDescriptionProvider>().ApiVersionDescriptions;
        foreach (var description in descriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Store API {description.GroupName}");
        }
    });
}

app.MapControllers();

try
{
    Log.Information("Starting Store API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Store API terminated unexpectedly");
}
finally
{
    // Make sure buffered file writes are flushed before the process exits.
    Log.CloseAndFlush();
}
