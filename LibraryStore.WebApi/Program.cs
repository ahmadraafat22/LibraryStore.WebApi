
using FluentValidation;
using LibraryStore.Application.Common.Behaviors;
using LibraryStore.Application.Common.Interfaces;
using LibraryStore.Application.Features.Categories.Commands.CreateCategory;
using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using LibraryStore.Infrastructure.Persistence;
using LibraryStore.Infrastructure.Services;
using LibraryStore.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Telegram.Bot;

namespace LibraryStore.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // application layer services 
            builder.Services.AddMediatR(
                config => config.RegisterServicesFromAssembly(typeof(CreateCategoryCommand).Assembly));

            builder.Services.AddValidatorsFromAssembly(
                typeof(CreateCategoryCommand).Assembly);

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


            // infrastructure layer sevices 

            builder.Services.AddDbContext<LibraryDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("cs")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }
            ).AddEntityFrameworkStores<LibraryDbContext>();

            builder.Services.AddScoped<ILibraryDbContext, LibraryDbContext>();

            builder.Services.AddScoped<IJwtService, JwtService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Iss"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Aud"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                    ValidateLifetime = true
                };
            });
            builder.Services.AddAuthorization();
            builder.Services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
            builder.Services.AddScoped<ITelegramOrderNotifier, TelegramOrderNotifier>();
            builder.Services.AddScoped<TelegramWebhookHandler>();
            // Add services to the container.

            builder.Services
                .AddOptions<TelegramOptions>()
                .Bind(builder.Configuration.GetSection(TelegramOptions.SectionName))
                .ValidateDataAnnotations()
                .Validate(options =>
                    options.AdminChatId != 0,
                    "Telegram:AdminChatId is required "
                ).Validate(
                    options => options.AdminUserId != 0,
                    "Telegram:AdminUserId is required.")
                .ValidateOnStart();
            //builder.Services


            builder.Services.AddHttpClient("TelegramBot")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler())
    .AddTypedClient<ITelegramBotClient>((httpClient, serviceProvider) =>
    {
        var telegramOptions = serviceProvider
            .GetRequiredService<IOptions<TelegramOptions>>()
            .Value;

        return new TelegramBotClient(
            telegramOptions.BotToken,
            httpClient);
    });
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // temp 
            builder.Logging.AddConsole();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole>>();
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();

                var telegramWebhookHandler = scope.ServiceProvider
                    .GetRequiredService<TelegramWebhookHandler>();

                await telegramWebhookHandler.RegisterWebhookAsync();

                app.UseSwagger();
                app.UseSwaggerUI();
            }
            //app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
