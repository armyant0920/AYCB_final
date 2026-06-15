using CorporateSite.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<NewsRepository>();

// CORS：只允許 Web 專案來源
builder.Services.AddCors(o => o.AddPolicy("WebOnly", p =>
    p.WithOrigins(builder.Configuration["WebOrigin"] ?? "https://localhost:7000")
     .AllowAnyHeader()
     .AllowAnyMethod()));

var app = builder.Build();

app.UseCors("WebOnly");
app.UseAuthorization();
app.MapControllers();

app.Run();
