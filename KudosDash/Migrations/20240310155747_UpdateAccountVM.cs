using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KudosDash.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountVM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Feedback_FeedbackId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AccountVM");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FeedbackId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FeedbackId",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FeedbackId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountVM",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ConfirmNewPassword = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    NewPassword = table.Column<string>(type: "TEXT", nullable: true),
                    OldPassword = table.Column<string>(type: "TEXT", nullable: true),
                    TeamName = table.Column<int>(type: "INTEGER", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountVM", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FeedbackId",
                table: "AspNetUsers",
                column: "FeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Feedback_FeedbackId",
                table: "AspNetUsers",
                column: "FeedbackId",
                principalTable: "Feedback",
                principalColumn: "Id");
        }
    }
}
