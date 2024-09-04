using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.PostgresMigrations.Migrations
{
    /// <inheritdoc />
    public partial class CreateRolePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                schema: "Account",
                table: "Permissions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "Account",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    PermissionId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Account",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "Account",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId",
                schema: "Account",
                table: "Permissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                schema: "Account",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_AspNetRoles_RoleId",
                schema: "Account",
                table: "Permissions",
                column: "RoleId",
                principalSchema: "Account",
                principalTable: "AspNetRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_AspNetRoles_RoleId",
                schema: "Account",
                table: "Permissions");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_RoleId",
                schema: "Account",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "Account",
                table: "Permissions");
        }
    }
}
