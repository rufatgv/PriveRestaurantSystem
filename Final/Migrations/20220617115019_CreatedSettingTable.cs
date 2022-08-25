using Microsoft.EntityFrameworkCore.Migrations;

namespace Final.Migrations
{
    public partial class CreatedSettingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "Settings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "Settings");
        }
    }
}
