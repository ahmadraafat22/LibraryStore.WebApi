using FluentValidation;

namespace LibraryStore.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersValidator : AbstractValidator<GetAllOrdersQuery>
    {
        public GetAllOrdersValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue);
        }
    }
}
