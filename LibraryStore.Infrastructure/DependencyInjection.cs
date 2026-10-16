using LibraryStore.Application.Common.Interfaces;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using LibraryStore.Infrastructure.Persistence;
using LibraryStore.Infrastructure.Services;
using LibraryStore.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Telegram.Bot;

namespace LibraryStore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("cs")));

            services.AddScoped<ILibraryDbContext, LibraryDbContext>();


            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<LibraryDbContext>();


            services.AddScoped<IJwtService, JwtService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = config["JWT:Iss"],

                        ValidateAudience = true,
                        ValidAudience = config["JWT:Aud"],

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    config["JWT:Key"])),

                        ValidateLifetime = true
                    };
            });


            services.AddAuthorization();


            services.AddScoped<
                ICodeGeneratorService,
                CodeGeneratorService>();


            services.Configure<CloudinaryOptions>(
                config.GetSection("Cloudinary"));

            services.AddScoped<
                ICloudinaryService,
                CloudinaryService>();


            services.AddScoped<
                ITelegramOrderNotifier,
                TelegramOrderNotifier>();

            services.AddScoped<TelegramWebhookHandler>();

            services
                .AddOptions<TelegramOptions>()
                .Bind(config.GetSection(
                    TelegramOptions.SectionName))
                .ValidateDataAnnotations()
                .Validate(
                    options => options.AdminChatId != 0,
                    "Telegram:AdminChatId is required")
                .Validate(
                    options => options.AdminUserId != 0,
                    "Telegram:AdminUserId is required")
                .ValidateOnStart();

            services
                .AddHttpClient("TelegramBot")
                .ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler())
                .AddTypedClient<ITelegramBotClient>(
                    (httpClient, serviceProvider) =>
                    {
                        var telegramOptions =
                            serviceProvider
                                .GetRequiredService<
                                    IOptions<TelegramOptions>>()
                                .Value;

                        return new TelegramBotClient(
                            telegramOptions.BotToken,
                            httpClient);
                    });

            return services;

        }
    }
}
