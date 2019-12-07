using Microsoft.EntityFrameworkCore.Migrations;

namespace ProcastinationKiller.Migrations
{
    public partial class Changeuidonbadges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Badge",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Badge",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
