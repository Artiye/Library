using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addto3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "adminuser123412903847192311234",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "dfcc1f6e-eb55-416b-9b79-fd89ed641e05", "ARTINJOBRO@GMAIL.COM", "AQAAAAIAAYagAAAAEGIv0PYIsao0TAb5k5FsQbp9vO+3sGQ4KrSP6qcVFNdyPx7czSAT4lxROYd4vUq7GQ==", "837e6343-8868-4a0a-923d-10138a49eeed", "artinjobro@gmail.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "adminuser123412903847192311234",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "a2228774-2d43-4daf-bbd1-2ab33ce09d16", null, "AQAAAAIAAYagAAAAEK4r5yq3EK4jeJIFyPVCRYZemdqw+eOgS/qVpEx8Ysu6SnHAvn2SOmA2nGSGpizrQQ==", "76943c82-8930-45e7-879d-33e7bfea96e6", "Art Morina" });
        }
    }
}
