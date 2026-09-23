using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdersequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "OrderSequence");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "OrderSequence");
        }
    }
}
