using LibraryStore.Application.Common.Exceptions;

namespace LibraryStore.WebApi.Middlwares
{
    public class ExceptionMiddleware
    {
        private RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        public static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = "not found ",
                errors = new List<string>()
            };

            switch (ex)
            {
                case FluentValidation.ValidationException validationException:
                    context.Response.StatusCode = 400;
                    response = new
                    {
                        message = "validation Exception",
                        errors = validationException.Errors
                        .Select(e => e.ErrorMessage)
                        .ToList()
                    };
                    break;
                case NotFoundException notFoundException:
                    context.Response.StatusCode = 404;
                    response = new
                    {
                        message = notFoundException.Message,
                        errors = new List<string>()
                    };
                    break;
                case UnauthorizedAccessException unauthorizedAccessException:
                    context.Response.StatusCode = 401;
                    response = new
                    {
                        message = "UnAuthorized !",
                        errors = new List<string>()
                    };
                    break;
                default:
                    context.Response.StatusCode = 500;
                    response = new
                    {
                        message = "Unexepected error ocurred !",
                        errors = new List<string>()
                    };
                    break;

            }

            await context.Response.WriteAsJsonAsync(response);
        }

    }
}
