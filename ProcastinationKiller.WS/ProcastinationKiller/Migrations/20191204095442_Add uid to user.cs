using Microsoft.EntityFrameworkCore.Migrations;

namespace ProcastinationKiller.Migrations
{
    public partial class Adduidtouser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UId",
                table: "Users");
        }
    }
}
