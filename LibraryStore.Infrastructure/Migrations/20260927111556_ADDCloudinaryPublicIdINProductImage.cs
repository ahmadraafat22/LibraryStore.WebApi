using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ADDCloudinaryPublicIdINProductImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "ProductImages");
        }
    }
}
