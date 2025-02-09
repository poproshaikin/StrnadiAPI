using DotNetEnv;
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

        Env.Load();
        
        builder.Configuration.AddEnvironmentVariables();
        
        builder.Services.AddControllers();
        builder.Services.AddDbContext<StrnadiDbContext>();
        builder.Services.AddScoped<IRecordingsRepository, RecordingsRepository>();
        builder.Services.AddScoped<IRecordingPartsRepository, RecordingPartsRepository>();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddScoped<IEmailSender, EmailSender>();
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
        
        WebApplication app = builder.Build();

        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}