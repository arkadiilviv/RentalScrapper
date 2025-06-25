using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImScoutAtWorker.Migrations
{
    /// <inheritdoc />
    public partial class AddHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hash",
                table: "flats",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_flats_hash",
                table: "flats",
                column: "hash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "ak_flats_hash",
                table: "flats");

            migrationBuilder.DropColumn(
                name: "hash",
                table: "flats");
        }
    }
}
