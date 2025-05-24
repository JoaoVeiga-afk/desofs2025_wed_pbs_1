using System.Text;
using DDDSample1.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using ShopTex.Domain.Shared;
using ShopTex.Domain.Users;
using ShopTex.Infrastructure.Shared;
using ShopTex.Infrastructure.Users;
using ShopTex.Models;
using ShopTex.Services;

namespace ShopTex;

public class Startup
{
    public IConfiguration Configuration { get; }
    
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>(opt =>
            //opt.UseSqlServer(Configs.DbConnection).ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>());
            opt.UseMySQL(Configs.DbConnection)
                .ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>());
        ConfigureMyServices(services);
        
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", builder =>
            {
                builder.WithOrigins("http://localhost:4200","http://localhost:4000","http://uvm003.dei.isep.ipp.pt")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        services.AddControllers().AddNewtonsoftJson();
        services.AddSwaggerGen();

        var jwtIssuer = Configuration.GetSection("Jwt:Issuer").Get<string>();
        var jwtKey = Configuration.GetSection("Jwt:Key").Get<string>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseExceptionHandler(new ExceptionHandlerOptions()
        {
            AllowStatusCode404Response = true,
            ExceptionHandlingPath = "/Error"
        });

        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");

        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints => endpoints.MapControllers());
        
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            dbContext.Database.Migrate(); // This applies migrations and creates tables
        }
    }

    public void ConfigureMyServices(IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<UserService>();
    }
}