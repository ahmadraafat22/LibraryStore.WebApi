using LibraryStore.Domain.Common.Enums;

namespace LibraryStore.Domain.Interfaces
{
    public interface ICodeGeneratorService
    {
        Task<string> GenerateCodeAsync(CodeType type);
    }
}
