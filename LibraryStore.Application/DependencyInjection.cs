using FluentValidation;
using LibraryStore.Application.Common.Behaviors;
using LibraryStore.Application.Features.Categories.Commands.CreateCategory;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryStore.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(
                config => config.RegisterServicesFromAssembly(
                typeof(CreateCategoryCommand).Assembly));

            services.AddValidatorsFromAssembly(
                typeof(CreateCategoryCommand).Assembly);

            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
