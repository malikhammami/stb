using Common.Logging;
using Cqrs.Hosts;
using FluentValidation;
using HealthChecks.UI.Client;
using MaxMind.GeoIP2;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Registration.API.Extensions;
using Registration.Application.Behaviours;
using Registration.Application.Contracts.Persistence;
using Registration.Application.Features.Commands;
using Registration.Infrastructure.Persistance;
using Registration.Infrastructure.Repositories;
using Serilog;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog(SeriLogger.Configure);



        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        

        builder.Services.AddSingleton<RabbitMQService>();
        builder.Services.AddSingleton<DatabaseReader>(provider =>
        {
            string geoIpDatabasePath = "C:\\GeoLite2-ASN_20240412\\GeoLite2-ASN.mmdb";
            return new DatabaseReader(geoIpDatabasePath);
        });

        builder.Services.AddMediatR(typeof(StartUp).Assembly);
        builder.Services.AddMediatR(typeof(ClientRegistrationCommand).GetTypeInfo().Assembly);
        builder.Services.AddScoped(typeof(IClientRepository), typeof(ClientRepository));
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());



        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        builder.Services.AddTransient<LoggingDelegatingHandler>();




        builder.Services.AddDbContext<RegistrationContext>(options =>
                        options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"), options => options.EnableRetryOnFailure()));
        builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
        builder.Services.AddScoped<IClientRepository, ClientRepository>();


        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddControllers();

        builder.Services.AddHealthChecks().AddDbContextCheck<RegistrationContext>();


        var app = builder.Build();
        app.MigrateDatabase<RegistrationContext>((context, services) =>
         {

         });



        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
            app.UseSwagger();
            app.UseSwaggerUI();
        //}
        app.UseRouting();
        app.UseAuthorization();
        

        app.MapControllers();

        app.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        }) ;

        app.Run();
    }



}