using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class projetstatusAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1f3713b9-6cc3-48fb-ac19-106ae07fde53");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a0fcf625-a320-4cdc-b178-30e5004cad1e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a49de01d-c376-4d9e-b0f3-44c6262a93c1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec6113e2-4e3d-42e1-b0f4-64b453a42552");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Projets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<short>(
                name: "TypeEntityNotification",
                table: "Notifications",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5dd552b6-379e-4a1d-9d02-c768a7d448e6", null, "Employe", "Employe" },
                    { "ad724162-957d-4322-9057-93e981f99f0d", null, "Admin", "ADMIN" },
                    { "d32f0da1-9708-412b-a593-b30eb80e7f0c", null, "DepartementManager", "DepartementManager" },
                    { "fe1cd2f8-68ad-4e39-bce0-5667d88b991f", null, "Financier", "Financier" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5dd552b6-379e-4a1d-9d02-c768a7d448e6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ad724162-957d-4322-9057-93e981f99f0d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d32f0da1-9708-412b-a593-b30eb80e7f0c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fe1cd2f8-68ad-4e39-bce0-5667d88b991f");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Projets");

            migrationBuilder.AlterColumn<int>(
                name: "TypeEntityNotification",
                table: "Notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1f3713b9-6cc3-48fb-ac19-106ae07fde53", null, "Admin", "ADMIN" },
                    { "a0fcf625-a320-4cdc-b178-30e5004cad1e", null, "DepartementManager", "DepartementManager" },
                    { "a49de01d-c376-4d9e-b0f3-44c6262a93c1", null, "Employe", "Employe" },
                    { "ec6113e2-4e3d-42e1-b0f4-64b453a42552", null, "Financier", "Financier" }
                });
        }
    }
}
