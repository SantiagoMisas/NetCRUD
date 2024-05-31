using Microsoft.EntityFrameworkCore;
using NetCrud.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; 
    });

builder.Services.AddDbContext<ApplicationDb>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


app.UseAuthorization();

app.MapControllers();

app.Run();
