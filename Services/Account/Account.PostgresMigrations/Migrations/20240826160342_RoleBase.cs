using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.PostgresMigrations.Migrations
{
    /// <inheritdoc />
    public partial class RoleBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Account",
                table: "AspNetRoles",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Account",
                table: "AspNetRoles");
        }
    }
}
