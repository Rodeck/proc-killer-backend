using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProcastinationKiller.Migrations
{
    public partial class RegistrationCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistartionCode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: false),
                    ConfirmationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistartionCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserState",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Points = table.Column<int>(nullable: false),
                    DailyLogins = table.Column<int>(nullable: false),
                    WeeklyLogins = table.Column<int>(nullable: false),
                    LongestLoginStreak = table.Column<int>(nullable: false),
                    CurrentLoginStreak = table.Column<int>(nullable: false),
                    TotalTodosCompleted = table.Column<int>(nullable: false),
                    LastLoginDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Regdate = table.Column<DateTime>(nullable: false),
                    UserStatus = table.Column<int>(nullable: false),
                    CodeId = table.Column<int>(nullable: true),
                    CurrentStateId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_RegistartionCode_CodeId",
                        column: x => x.CodeId,
                        principalTable: "RegistartionCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_UserState_CurrentStateId",
                        column: x => x.CurrentStateId,
                        principalTable: "UserState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Regdate = table.Column<DateTime>(nullable: false),
                    FinishTime = table.Column<DateTime>(nullable: true),
                    TargetDate = table.Column<DateTime>(nullable: false),
                    Completed = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Todos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaseEvent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(nullable: false),
                    Hidden = table.Column<bool>(nullable: false),
                    StateId = table.Column<int>(nullable: true),
                    Points = table.Column<int>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    CompletedItemId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseEvent_UserState_StateId",
                        column: x => x.StateId,
                        principalTable: "UserState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseEvent_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseEvent_Todos_CompletedItemId",
                        column: x => x.CompletedItemId,
                        principalTable: "Todos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseEvent_StateId",
                table: "BaseEvent",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseEvent_UserId",
                table: "BaseEvent",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseEvent_CompletedItemId",
                table: "BaseEvent",
                column: "CompletedItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_UserId",
                table: "Todos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CodeId",
                table: "Users",
                column: "CodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrentStateId",
                table: "Users",
                column: "CurrentStateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseEvent");

            migrationBuilder.DropTable(
                name: "Todos");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "RegistartionCode");

            migrationBuilder.DropTable(
                name: "UserState");
        }
    }
}
