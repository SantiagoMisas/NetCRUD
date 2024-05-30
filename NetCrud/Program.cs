using Microsoft.EntityFrameworkCore;
using NetCrud.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDb>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();



// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();