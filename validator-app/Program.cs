var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient("kosit", client => {
    client.BaseAddress = new Uri(
        Environment.GetEnvironmentVariable("KOSIT_URL") 
        ?? "http://localhost:8080"
    );
    client.Timeout = TimeSpan.FromSeconds(60);
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();