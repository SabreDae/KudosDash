using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KudosDash.Migrations
{
	/// <inheritdoc />
	public partial class RestrictTeamNameLength : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<int>(
				name: "TeamName",
				table: "AccountVM",
				type: "INTEGER",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "TEXT",
				oldNullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "TeamName",
				table: "AccountVM",
				type: "TEXT",
				nullable: true,
				oldClrType: typeof(int),
				oldType: "INTEGER",
				oldNullable: true);
		}
	}
}