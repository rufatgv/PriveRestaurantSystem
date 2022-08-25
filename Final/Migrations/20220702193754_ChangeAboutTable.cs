using Microsoft.EntityFrameworkCore.Migrations;

namespace Final.Migrations
{
    public partial class ChangeAboutTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTitle",
                table: "AboutIntros");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "AboutIntros");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubTitle",
                table: "AboutIntros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AboutIntros",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
