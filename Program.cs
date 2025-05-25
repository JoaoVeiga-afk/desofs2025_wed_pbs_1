using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.AspNetCore;

namespace ShopTex;

public class Program
{
    public static void Main(string[] args)
    {
        // Enable Serilog self-logging for diagnostics
        Serilog.Debugging.SelfLog.Enable(msg => Console.Error.WriteLine(msg));

        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.MySQL(
               connectionString: builder.Configuration.GetConnectionString("MySqlLogs"),
               tableName:"Logs")
            .CreateLogger();

        builder.Host.UseSerilog();

        var startup = new ShopTex.Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        var app = builder.Build();

        startup.Configure(app, app.Environment);

        Log.Information("Application started");

        app.Run();
    }
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
