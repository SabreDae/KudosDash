using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KudosDash.Migrations
	{
	/// <inheritdoc />
	public partial class RemoveRoleFromUserModel : Migration
		{
		/// <inheritdoc />
		protected override void Up (MigrationBuilder migrationBuilder)
			{
			migrationBuilder.DropColumn(
				name: "Role",
				table: "AspNetUsers");
			}

		/// <inheritdoc />
		protected override void Down (MigrationBuilder migrationBuilder)
			{
			migrationBuilder.AddColumn<string>(
				name: "Role",
				table: "AspNetUsers",
				type: "TEXT",
				nullable: false,
				defaultValue: "");
			}
		}
	}
