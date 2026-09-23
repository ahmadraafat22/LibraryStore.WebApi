using LibraryStore.Domain.Common.Enums;
using LibraryStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Infrastructure.Services
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ILibraryDbContext _context;
        public CodeGeneratorService(ILibraryDbContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateCodeAsync(CodeType type)
        {
            var sequenceName = GetSequenceName(type);
            var prefix = GetPrefix(type);

            await using var command = _context.Database.GetDbConnection().CreateCommand();

            command.CommandText = $"SELECT NEXT VALUE FOR dbo.{sequenceName}";

            if (command.Connection.State != System.Data.ConnectionState.Open)
            {
                await command.Connection.OpenAsync();
            }
            var result = await command.ExecuteScalarAsync();
            var number = Convert.ToInt64(result);
            return $"{prefix}-{number:D6}";
        }
        private static string GetSequenceName(CodeType type)
        {
            return $"{type}Sequence";
        }
        private static string GetPrefix(CodeType type)
        {
            switch (type)
            {
                case CodeType.Order:
                    return "ORD";
                default:
                    throw new ArgumentException("not Supported type");
            }

        }
    }
}
