
using FluentValidation;
using LibraryStore.Application.Common.Behaviors;
using LibraryStore.Application.Features.Categories.Commands.CreateCategory;
using LibraryStore.Domain.Interfaces;
using LibraryStore.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<LibraryDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("cs")));


            // application layer services 
            builder.Services.AddMediatR(
                config => config.RegisterServicesFromAssembly(typeof(CreateCategoryCommand).Assembly));
            builder.Services.AddValidatorsFromAssembly(
                typeof(CreateCategoryCommand).Assembly);
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


            // infrastructure layer sevices 
            builder.Services.AddScoped<ILibraryDbContext, LibraryDbContext>();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
