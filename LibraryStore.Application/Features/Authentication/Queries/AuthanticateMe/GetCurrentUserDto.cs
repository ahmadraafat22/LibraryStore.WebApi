namespace LibraryStore.Application.Features.Authentication.Queries.AuthanticateMe
{
    public class GetCurrentUserDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
