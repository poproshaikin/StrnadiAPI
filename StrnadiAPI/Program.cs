namespace StrnadiAPI;

public class Program
{
    private const string CORS_POLICY_NAME = "defaultPolicy";
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(corsOptions =>
        {
            corsOptions.AddPolicy(CORS_POLICY_NAME, policyBuilder =>
            {
                policyBuilder
                    .AllowAnyOrigin() // docasne povoluji vsechny origins, ale po zahostovani front-endu to zmenim
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        
        
        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        app.Run();
    }
}