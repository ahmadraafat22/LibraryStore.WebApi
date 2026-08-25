using FluentValidation;
using MediatR;

namespace LibraryStore.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // check if validator has rules 
            if (_validators.Any())
            {
                // return validation context to validate 
                var validationContext = new ValidationContext<TRequest>(request);
                // this will return validation results 
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));
                // check if results has failures 
                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();
                // throw validation exception 
                if (failures.Any())
                {
                    throw new ValidationException(failures);
                }
            }
            return await next(cancellationToken);
        }
    }
}
