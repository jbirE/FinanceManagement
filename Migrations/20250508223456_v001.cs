using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class v001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "96e024e0-c843-4f39-92f1-fbaf9486de0e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2e1b3c5-c840-4890-a492-f7dbf6985f07");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aae6857e-795a-47d9-be59-3a9beb9a00d3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d067ef9b-47a8-46fc-9287-9e767c92d621");

            migrationBuilder.DropColumn(
                name: "ExpenseCategory",
                table: "RapportsDepenses");

            migrationBuilder.DropColumn(
                name: "BudgetTotal",
                table: "Departements");

            migrationBuilder.RenameColumn(
                name: "DateEnvoi",
                table: "Notifications",
                newName: "DateCreation");

            migrationBuilder.RenameColumn(
                name: "dateEmbauche",
                table: "AspNetUsers",
                newName: "DateEmbauche");

            migrationBuilder.AddColumn<bool>(
                name: "IsReaded",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Titre",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Departements",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1f39e196-82fd-4e38-a957-48acf6c3531b", null, "Financier", "Financier" },
                    { "72d135df-d12d-47f8-ac2e-4c2b38bf3cfe", null, "Employe", "Employe" },
                    { "79035f86-9ba5-4fe1-9737-a7c91c5073b0", null, "DepartementManager", "DepartementManager" },
                    { "a44dd78f-8f07-4a0b-8c38-87ce2ddfd11c", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1f39e196-82fd-4e38-a957-48acf6c3531b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "72d135df-d12d-47f8-ac2e-4c2b38bf3cfe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "79035f86-9ba5-4fe1-9737-a7c91c5073b0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a44dd78f-8f07-4a0b-8c38-87ce2ddfd11c");

            migrationBuilder.DropColumn(
                name: "IsReaded",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Titre",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "DateCreation",
                table: "Notifications",
                newName: "DateEnvoi");

            migrationBuilder.RenameColumn(
                name: "DateEmbauche",
                table: "AspNetUsers",
                newName: "dateEmbauche");

            migrationBuilder.AddColumn<string>(
                name: "ExpenseCategory",
                table: "RapportsDepenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Departements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BudgetTotal",
                table: "Departements",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "96e024e0-c843-4f39-92f1-fbaf9486de0e", null, "DepartementManager", "DepartementManager" },
                    { "a2e1b3c5-c840-4890-a492-f7dbf6985f07", null, "Employe", "Employe" },
                    { "aae6857e-795a-47d9-be59-3a9beb9a00d3", null, "Financier", "Financier" },
                    { "d067ef9b-47a8-46fc-9287-9e767c92d621", null, "Admin", "ADMIN" }
                });
        }
    }
}
