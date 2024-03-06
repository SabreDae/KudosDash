using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KudosDash.Migrations
{
	/// <inheritdoc />
	public partial class UpdateFeedbackModelReqFields : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Author",
				table: "Feedback",
				type: "TEXT",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "TEXT");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Author",
				table: "Feedback",
				type: "TEXT",
				nullable: false,
				defaultValue: "",
				oldClrType: typeof(string),
				oldType: "TEXT",
				oldNullable: true);
		}
	}
}