using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KudosDash.Migrations
{
	/// <inheritdoc />
	public partial class AddApprovalToFeedbackModel : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "ManagerApproved",
				table: "Feedback",
				type: "INTEGER",
				nullable: false,
				defaultValue: false);

			migrationBuilder.CreateTable(
				name: "AccountVM",
				columns: table => new
				{
					Id = table.Column<string>(type: "TEXT", nullable: false),
					FirstName = table.Column<string>(type: "TEXT", nullable: false),
					LastName = table.Column<string>(type: "TEXT", nullable: false),
					Email = table.Column<string>(type: "TEXT", nullable: false),
					Username = table.Column<string>(type: "TEXT", nullable: true),
					TeamName = table.Column<string>(type: "TEXT", nullable: true),
					OldPassword = table.Column<string>(type: "TEXT", nullable: true),
					NewPassword = table.Column<string>(type: "TEXT", nullable: true),
					ConfirmNewPassword = table.Column<string>(type: "TEXT", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AccountVM", x => x.Id);
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "AccountVM");

			migrationBuilder.DropColumn(
				name: "ManagerApproved",
				table: "Feedback");
		}
	}
}