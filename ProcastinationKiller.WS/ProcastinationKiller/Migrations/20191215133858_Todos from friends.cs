using Microsoft.EntityFrameworkCore.Migrations;

namespace ProcastinationKiller.Migrations
{
    public partial class Todosfromfriends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FriendId",
                table: "Todos",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FromFriend",
                table: "Todos",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "FromFriend",
                table: "Todos");
        }
    }
}
