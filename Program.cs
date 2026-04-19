using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddInterceptors(new AuditInterceptor()));

var app = builder.Build();


app.MapControllers();
app.UseHttpsRedirection();

app.Run();

