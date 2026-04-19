using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: 
        "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddInterceptors(new AuditInterceptor()));

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(o =>
        o.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddHangfireServer();


builder.Services.AddScoped<ReconciliationService>();
builder.Services.AddScoped<RecordComparator>();
builder.Services.AddScoped<IExternalSystemClient, FakeExternalClientSystem>();

builder.Services.AddScoped<RetryJobs>();
builder.Services.AddScoped<PeriodScanJobs>();
builder.Services.AddScoped<StuckRecoveryJobs>();

builder.Services.AddControllers();

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();

app.UseHangfireDashboard();

using (var scope = app.Services.CreateScope())
{
    RecurringJob.AddOrUpdate<RetryJobs>(
        "retry-job",
        job => job.ExecuteAsync(),
        Cron.Minutely);

    RecurringJob.AddOrUpdate<PeriodScanJobs>(
        "periodic-scan",
        job => job.ExecuteAsync(),
        Cron.Minutely);
RecurringJob.AddOrUpdate<StuckRecoveryJobs>(
    "stuck-recovery",
    job => job.ExecuteAsync(),
    "*/5 * * * *");
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();

app.MapControllers();
app.Run();