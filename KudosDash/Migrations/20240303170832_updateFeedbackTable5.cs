using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KudosDash.Migrations
{
    /// <inheritdoc />
    public partial class updateFeedbackTable5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
