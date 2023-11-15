using Microsoft.EntityFrameworkCore;
using TecEnergy.Database;
using TecEnergy.Database.DataModels;
using TecEnergy.Database.Repositories;
using TecEnergy.Database.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TecEnergy.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //var cs = builder.Configuration.GetConnectionString("SqlServer");
        
       //builder.WebHost.UseUrls("https://*:7141");

        //// Add services to the container.
        builder.Services.AddDbContext<DatabaseContext>(
                 o => o.UseSqlServer("Data Source=10.233.134.112,1433;Initial Catalog=TecEnergyDB;User ID=TecAdmin;Password=Tec420;TrustServerCertificate=True"));


        //builder.Services.AddDbContext<DatabaseContext>(options =>
        //{
        //    options.UseSqlServer(cs, b => b.MigrationsAssembly("TecEnergy.Database")); // Update the assembly name accordingly
        //});

        builder.Services.AddScoped<IBuildingRepository, BuildingRepository>();
        builder.Services.AddScoped<IRoomRepository, RoomRepository>();
        builder.Services.AddScoped<IEnergyMeterRepository, EnergyMeterRepository>();
        builder.Services.AddScoped<IEnergyDataRepository, EnergyDataRepository>();

        //Ensures that many to many models does not loop into each other lists.
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null; // Preserve property names as-is
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; // Handle reference loops

            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

       
        // Configure the HTTP request pipeline.

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
