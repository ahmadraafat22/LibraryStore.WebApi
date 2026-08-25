namespace LibraryStore.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoryQueryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

    }
}
