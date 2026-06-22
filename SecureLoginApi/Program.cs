using SecureLoginApi.Repositories;
using SecureLoginApi.Services;

namespace SecureLoginApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Registrera repositories och services i DI-containern
            // AddSingleton = samma instans lever hela appens livstid (krävs för InMemory-lagring)
            builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();

            // AddScoped = ny instans per request (standard för services)
            builder.Services.AddScoped<AuthService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
