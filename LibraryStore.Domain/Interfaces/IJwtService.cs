using LibraryStore.Domain.Entities;

namespace LibraryStore.Domain.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateToken(ApplicationUser user);
    }
}
