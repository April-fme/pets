using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<PetsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PetsDatabase")));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Controllers
builder.Services.AddControllers();

// Add services
builder.Services.AddScoped<IAlertService, AlertService>();

// Add Hosted Service for collar simulator
builder.Services.AddHostedService<CollarSimulatorService>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Pets Health Monitoring API");

app.Run();

