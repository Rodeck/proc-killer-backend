using Microsoft.EntityFrameworkCore.Migrations;

namespace ProcastinationKiller.Migrations
{
    public partial class AddTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TagString",
                table: "Todos",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagString",
                table: "Todos");
        }
    }
}
