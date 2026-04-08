var builder = WebApplication.CreateBuilder(args);



var app = builder.Build();

app.MapControllers();
app.UseHttpsRedirection();

app.Run();

