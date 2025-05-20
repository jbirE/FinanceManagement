using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class testbuget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Factures_RapportsDepenses_RapportDepenseId",
                table: "Factures");

            migrationBuilder.DropForeignKey(
                name: "FK_RapportsDepenses_AspNetUsers_UtilisateurId",
                table: "RapportsDepenses");

            migrationBuilder.DropForeignKey(
                name: "FK_RapportsDepenses_BudgetsProjets_BudgetProjetId",
                table: "RapportsDepenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RapportsDepenses",
                table: "RapportsDepenses");

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
                name: "BudgetAlloue",
                table: "Projets");

            migrationBuilder.RenameTable(
                name: "RapportsDepenses",
                newName: "RapportDepenses");

            migrationBuilder.RenameIndex(
                name: "IX_RapportsDepenses_UtilisateurId",
                table: "RapportDepenses",
                newName: "IX_RapportDepenses_UtilisateurId");

            migrationBuilder.RenameIndex(
                name: "IX_RapportsDepenses_BudgetProjetId",
                table: "RapportDepenses",
                newName: "IX_RapportDepenses_BudgetProjetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RapportDepenses",
                table: "RapportDepenses",
                column: "IdRpport");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Factures_RapportDepenses_RapportDepenseId",
                table: "Factures",
                column: "RapportDepenseId",
                principalTable: "RapportDepenses",
                principalColumn: "IdRpport",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RapportDepenses_AspNetUsers_UtilisateurId",
                table: "RapportDepenses",
                column: "UtilisateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RapportDepenses_BudgetsProjets_BudgetProjetId",
                table: "RapportDepenses",
                column: "BudgetProjetId",
                principalTable: "BudgetsProjets",
                principalColumn: "IdBudgetProjet",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Factures_RapportDepenses_RapportDepenseId",
                table: "Factures");

            migrationBuilder.DropForeignKey(
                name: "FK_RapportDepenses_AspNetUsers_UtilisateurId",
                table: "RapportDepenses");

            migrationBuilder.DropForeignKey(
                name: "FK_RapportDepenses_BudgetsProjets_BudgetProjetId",
                table: "RapportDepenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RapportDepenses",
                table: "RapportDepenses");

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

            migrationBuilder.RenameTable(
                name: "RapportDepenses",
                newName: "RapportsDepenses");

            migrationBuilder.RenameIndex(
                name: "IX_RapportDepenses_UtilisateurId",
                table: "RapportsDepenses",
                newName: "IX_RapportsDepenses_UtilisateurId");

            migrationBuilder.RenameIndex(
                name: "IX_RapportDepenses_BudgetProjetId",
                table: "RapportsDepenses",
                newName: "IX_RapportsDepenses_BudgetProjetId");

            migrationBuilder.AddColumn<double>(
                name: "BudgetAlloue",
                table: "Projets",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RapportsDepenses",
                table: "RapportsDepenses",
                column: "IdRpport");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Factures_RapportsDepenses_RapportDepenseId",
                table: "Factures",
                column: "RapportDepenseId",
                principalTable: "RapportsDepenses",
                principalColumn: "IdRpport",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RapportsDepenses_AspNetUsers_UtilisateurId",
                table: "RapportsDepenses",
                column: "UtilisateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RapportsDepenses_BudgetsProjets_BudgetProjetId",
                table: "RapportsDepenses",
                column: "BudgetProjetId",
                principalTable: "BudgetsProjets",
                principalColumn: "IdBudgetProjet",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
