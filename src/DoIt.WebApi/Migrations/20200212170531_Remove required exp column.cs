using Microsoft.EntityFrameworkCore.Migrations;

namespace ProcastinationKiller.Migrations
{
    public partial class Removerequiredexpcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredExp",
                table: "Level");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequiredExp",
                table: "Level",
                nullable: false,
                defaultValue: 0);
        }
    }
}
