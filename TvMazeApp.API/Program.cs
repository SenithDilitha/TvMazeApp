using Microsoft.EntityFrameworkCore;
using TvMazeApp.Application.Services;
using TvMazeApp.Domain.Interfaces;
using TvMazeApp.Infrastructure.Data;
using TvMazeApp.Infrastructure.Repositories;
using TvMazeApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IShowRepository, ShowRepository>();
builder.Services.AddScoped<ITvMazeService, TvMazeService>();
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
