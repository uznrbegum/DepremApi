using DepremApi.Services;
using Scalar.AspNetCore;
using DepremApi.Data; //DepremDbContext sınıfını kullanabilmek için.
using Microsoft.EntityFrameworkCore;
using DepremApi.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<DepremDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();

builder.Services.AddHttpClient<DepremService>();

builder.Services.AddScoped<AnalyticsService>();

builder.Services.AddHostedService<DepremBackgroundService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("NextJsPolicy");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();