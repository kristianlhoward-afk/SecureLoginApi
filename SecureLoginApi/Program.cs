using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SecureLoginApi.Data;
using SecureLoginApi.Repositories;
using SecureLoginApi.Services;
using System.Threading.RateLimiting;

namespace SecureLoginApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Connection string hämtas från user-secrets (lokalt) eller miljövariabler (produktion)
            // Sparas ALDRIG i kod eller appsettings.json - skyddar mot läckta databasuppgifter (OWASP A05)
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Registrera repositories och services i DI-containern
            // AddScoped = ny instans per request - rätt mönster för databas-kopplingar
            builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
            builder.Services.AddScoped<AuthService>();

            // Rate limiting - max 5 inloggningsförsök per minut per klient
            // Skyddar mot brute force-attacker mot login-endpointen (OWASP A07)
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("login", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueLimit = 0;
                });
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Swagger tillgängligt i alla miljöer - för att möjliggöra testning av deployad version
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Måste ligga innan MapControllers för att gälla
            app.UseRateLimiter();

            app.MapControllers();
            app.Run();
        }
    }
}
