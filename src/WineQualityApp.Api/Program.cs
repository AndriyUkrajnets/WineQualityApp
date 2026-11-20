using WineQualityApp.Application.Abstractions;
using WineQualityApp.Application.Services;
using WineQualityApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
var frontendOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy(
    "Frontend",
    policy => policy
        .WithOrigins(frontendOrigins)
        .WithMethods("GET", "POST")
        .WithHeaders("Content-Type")));
builder.Services.AddScoped<IPredictionService, PredictionService>();
builder.Services.AddPythonMlClient(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.MapControllers();

app.Run();
