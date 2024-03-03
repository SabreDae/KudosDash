using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KudosDash.Migrations
{
    /// <inheritdoc />
    public partial class updateFeedbackPK : Migration
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

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Feedback",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "FeedbackId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Feedback_FeedbackId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FeedbackId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FeedbackId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Feedback",
                newName: "ID");

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
