using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KudosDash.Migrations
{
    /// <inheritdoc />
    public partial class updateFeedbackTable4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_AspNetUsers_UserId",
                table: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_Feedback_UserId",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Feedback");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Feedback",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_UserId",
                table: "Feedback",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_AspNetUsers_UserId",
                table: "Feedback",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
