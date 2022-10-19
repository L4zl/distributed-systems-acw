using Microsoft.EntityFrameworkCore.Migrations;

namespace DistSysAcw.Migrations
{
    public partial class Migration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivedLogs",
                columns: table => new
                {
                    LogID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LogString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogDateTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApiKey = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedLogs", x => x.LogID);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LogString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogDateTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserApiKey = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_Logs_Users_UserApiKey",
                        column: x => x.UserApiKey,
                        principalTable: "Users",
                        principalColumn: "ApiKey",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserApiKey",
                table: "Logs",
                column: "UserApiKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedLogs");

            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
