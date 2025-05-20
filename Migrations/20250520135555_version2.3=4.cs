using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class version234 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "162a97e2-a9dc-46df-ab8f-94a1f4e45607");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4ed50648-4672-43b8-bec3-594c842369e5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a0208ead-9beb-4c71-9339-d1639085e8c5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b265e783-f9e1-460f-abbd-c0a122822496");

            migrationBuilder.AddColumn<int>(
                name: "EntityId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeEntityNotification",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TypeEntityNotification",
                table: "Notifications");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "162a97e2-a9dc-46df-ab8f-94a1f4e45607", null, "Admin", "ADMIN" },
                    { "4ed50648-4672-43b8-bec3-594c842369e5", null, "Financier", "Financier" },
                    { "a0208ead-9beb-4c71-9339-d1639085e8c5", null, "DepartementManager", "DepartementManager" },
                    { "b265e783-f9e1-460f-abbd-c0a122822496", null, "Employe", "Employe" }
                });
        }
    }
}
