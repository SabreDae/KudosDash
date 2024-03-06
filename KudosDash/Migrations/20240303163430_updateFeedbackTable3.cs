using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KudosDash.Migrations
{
	/// <inheritdoc />
	public partial class updateFeedbackTable3 : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Feedback_AspNetUsers_UsersId",
				table: "Feedback");

			migrationBuilder.RenameColumn(
				name: "UsersId",
				table: "Feedback",
				newName: "UserId");

			migrationBuilder.RenameIndex(
				name: "IX_Feedback_UsersId",
				table: "Feedback",
				newName: "IX_Feedback_UserId");

			migrationBuilder.AddForeignKey(
				name: "FK_Feedback_AspNetUsers_UserId",
				table: "Feedback",
				column: "UserId",
				principalTable: "AspNetUsers",
				principalColumn: "Id");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Feedback_AspNetUsers_UserId",
				table: "Feedback");

			migrationBuilder.RenameColumn(
				name: "UserId",
				table: "Feedback",
				newName: "UsersId");

			migrationBuilder.RenameIndex(
				name: "IX_Feedback_UserId",
				table: "Feedback",
				newName: "IX_Feedback_UsersId");

			migrationBuilder.AddForeignKey(
				name: "FK_Feedback_AspNetUsers_UsersId",
				table: "Feedback",
				column: "UsersId",
				principalTable: "AspNetUsers",
				principalColumn: "Id");
		}
	}
}