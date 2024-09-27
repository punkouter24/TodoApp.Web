using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Serilog;
using TodoApp.Web.Data;
using TodoApp.Web.Services;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Microsoft.ApplicationInsights.Extensibility;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
 .WriteTo.ApplicationInsights(new TelemetryConfiguration { InstrumentationKey = builder.Configuration["ApplicationInsights:InstrumentationKey"] }, TelemetryConverter.Traces)
    .CreateLogger();

builder.Host.UseSerilog();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.InstrumentationKey = builder.Configuration["ApplicationInsights:InstrumentationKey"];
});

//builder.Services.AddApplicationInsightsTelemetry();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<TelemetryService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");




using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        // or if you prefer to use migrations:
        // context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}








app.Run();








//structuered

//traces
//| where message startswith "Todo item"
//| order by timestamp desc
//| take 100


// dotnet publish -c Release -o ./publish
// Compress-Archive -Path ./publish/* -DestinationPath ./publish.zip -Force
// az webapp deployment source config-zip --resource-group TodoAppRG --name todoapp-nny4gymly32cm --src ./publish.zip

//az webapp config connection-string set --resource-group TodoAppRG --name todoapp-nny4gymly32cm --settings DefaultConnection="Server=tcp:sqlserver-nny4gymly32cm.database.windows.net,1433;Initial Catalog=TodoAppDb;Persist Security Info=False;User ID=sqladmin;Password=W4yc00l69!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"


//traces
//| where message startswith "Todo item created"
//| extend TodoTitle = tostring(customDimensions['Todo'].Title)
//| project timestamp, TodoTitle
//| order by timestamp desc
//| take 10traces
//| where message startswith "Todo item created"
//| extend TodoTitle = tostring(customDimensions['Todo'].Title)
//| project timestamp, TodoTitle
//| order by timestamp desc
//| take 10









