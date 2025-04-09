using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using ShopTex;
using ShopTex.Models;

namespace ShopTex;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
}

/*var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddDbContext<DatabaseContext>(opt => opt.UseInMemoryDatabase("UserDB"));
builder.Services.AddDbContext<DatabaseContext>(opt =>
    opt.UseSqlServer(Configs.DbConnection));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(new ExceptionHandlerOptions()
{
    AllowStatusCode404Response = true,
    ExceptionHandlingPath = "/Error"
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();*/
