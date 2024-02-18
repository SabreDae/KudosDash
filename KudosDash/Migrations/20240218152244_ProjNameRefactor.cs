using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KudosDash.Migrations
{
	/// <inheritdoc />
	public partial class ProjNameRefactor : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "TeamName",
				table: "AspNetUsers");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "TeamName",
				table: "AspNetUsers",
				type: "TEXT",
				nullable: false,
				defaultValue: "");
		}
	}
}