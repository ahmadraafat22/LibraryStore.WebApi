using LibraryStore.Application;
using LibraryStore.Infrastructure;
using LibraryStore.Infrastructure.Services;
using LibraryStore.WebApi.Middlwares;
using Microsoft.AspNetCore.Identity;

namespace LibraryStore.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddWebApi();


            builder.Logging.AddConsole();



            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    var allowedOrigins = builder.Configuration
                        .GetSection("Cors:AllowedOrigins")
                        .Get<string[]>() ?? Array.Empty<string>();

                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            var app = builder.Build();



            using (var scope = app.Services.CreateScope())
            {
                var roleManager =
                    scope.ServiceProvider
                        .GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole("Admin"));
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole("User"));
                }
            }



            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();

                var telegramWebhookHandler =
                    scope.ServiceProvider
                        .GetRequiredService<TelegramWebhookHandler>();

                await telegramWebhookHandler.RegisterWebhookAsync();

                app.UseSwagger();
                app.UseSwaggerUI();
            }



            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("FrontendPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}