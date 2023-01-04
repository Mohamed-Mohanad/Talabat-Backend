using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.API.Extensions;
using Talabat.API.Helper;
using Talabat.API.Middlewares;
using Talabat.BLL.Interfaces;
using Talabat.BLL.Repositories;
using Talabat.DAL.Data;
using Talabat.DAL.Entities.Identity;
using Talabat.DAL.Identity;

namespace Talabat.API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<StoreDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(x =>
            {
                var connection = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Radis"), true);
                return ConnectionMultiplexer.Connect(connection);
            });

            builder.Services.AddApplicationServices();
            builder.Services.AddIdentityServices(builder.Configuration);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerDocumentation();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExciptionMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            seedData();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();

            async void seedData()
            {
                using(var scope = app.Services.CreateScope())
                {
                    var servicesProvider = scope.ServiceProvider;
                    var loggerFactory = servicesProvider.GetRequiredService<ILoggerFactory>();

                    try
                    {
                        var context = servicesProvider.GetRequiredService<StoreDBContext>();
                        await context.Database.MigrateAsync();

                        await StoreContextSeed.SeedAsync(context, loggerFactory);

                        var identityContext = servicesProvider.GetRequiredService<AppIdentityDbContext>();
                        await identityContext.Database.MigrateAsync();

                        var userManage = servicesProvider.GetRequiredService<UserManager<AppUser>>();
                        await AppIdentityDbContextSeed.SeedUserAsync(userManage);
                    }
                    catch (Exception ex)
                    {
                        var logger = loggerFactory.CreateLogger<Program>();
                        logger.LogError(ex.Message);
                    }
                }
            }
        }
    }
}