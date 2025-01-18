using StrnadiAPI.Data;
using StrnadiAPI.Data.Repositories;
using StrnadiAPI.Services;

namespace StrnadiAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.AddControllers();
        builder.Services.AddDbContext<StrnadiDbContext>();
        builder.Services.AddScoped<IRecordingsRepository, RecordingsRepository>();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddScoped<IStrnadiEmailVerifier, StrnadiEmailVerifier>();
        builder.Services.AddCors(corsOptions =>
        {
            corsOptions.AddPolicy(configuration["Cors:PolicyName"], policyBuilder =>
            {
                policyBuilder
                    .AllowAnyOrigin() 
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        var app = builder.Build();

        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}