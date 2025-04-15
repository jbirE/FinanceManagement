using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "00c7281e-7365-4e47-b04c-84fd7c62e0bb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2110963f-9323-4c0c-b979-595454cd84de");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5d50aa4c-b616-477e-bda1-5661bdfb617a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6eb337f9-c76d-44a4-abfc-900e14be2337");

            migrationBuilder.AddColumn<string>(
                name: "Addresse",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cin",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nom",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prenom",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0d08bec8-536c-4bac-a17a-a7fc8447b2c7", null, "Employe", "Employe" },
                    { "46cd7dfe-0608-452d-92b0-ecc1a417e134", null, "Admin", "ADMIN" },
                    { "679224ad-b9fd-4eba-8b99-e3080da52e7a", null, "Financier", "Financier" },
                    { "d4c6b01b-00e1-4990-8171-05321582155c", null, "DepartementManger", "DepartementManger" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0d08bec8-536c-4bac-a17a-a7fc8447b2c7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "46cd7dfe-0608-452d-92b0-ecc1a417e134");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "679224ad-b9fd-4eba-8b99-e3080da52e7a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d4c6b01b-00e1-4990-8171-05321582155c");

            migrationBuilder.DropColumn(
                name: "Addresse",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Cin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nom",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Prenom",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "00c7281e-7365-4e47-b04c-84fd7c62e0bb", null, "DepartementManger", "DepartementManger" },
                    { "2110963f-9323-4c0c-b979-595454cd84de", null, "Financier", "Financier" },
                    { "5d50aa4c-b616-477e-bda1-5661bdfb617a", null, "Employe", "Employe" },
                    { "6eb337f9-c76d-44a4-abfc-900e14be2337", null, "Admin", "ADMIN" }
                });
        }
    }
}
