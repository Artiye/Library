using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleSeeding2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "adminuser123412903847192311234", 0, "a2228774-2d43-4daf-bbd1-2ab33ce09d16", "artinjobro@gmail.com", false, false, null, "ARTINJOBRO@GMAIL.COM", null, "AQAAAAIAAYagAAAAEK4r5yq3EK4jeJIFyPVCRYZemdqw+eOgS/qVpEx8Ysu6SnHAvn2SOmA2nGSGpizrQQ==", null, false, "76943c82-8930-45e7-879d-33e7bfea96e6", false, "Art Morina" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "adminRoleId1293931239438254523", "adminuser123412903847192311234" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "adminRoleId1293931239438254523", "adminuser123412903847192311234" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "adminuser123412903847192311234");
        }
    }
}
