using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ProcastinationKiller.Migrations
{
    public partial class Friends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendsInvitation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    InviterId = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    InvitationDate = table.Column<DateTime>(nullable: false),
                    Accepted = table.Column<bool>(nullable: false),
                    Rejected = table.Column<bool>(nullable: false),
                    AcceptedDate = table.Column<DateTime>(nullable: false),
                    RejectedDate = table.Column<DateTime>(nullable: false),
                    InviterName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendsInvitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendsInvitation_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MyInvitation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    InvitedId = table.Column<string>(nullable: true),
                    InvitationDate = table.Column<DateTime>(nullable: false),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsRejected = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyInvitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyInvitation_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendsInvitation_UserId",
                table: "FriendsInvitation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MyInvitation_UserId",
                table: "MyInvitation",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendsInvitation");

            migrationBuilder.DropTable(
                name: "MyInvitation");
        }
    }
}
