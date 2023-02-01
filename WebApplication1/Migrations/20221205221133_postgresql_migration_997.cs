using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApplication1.Migrations
{
    public partial class postgresql_migration_997 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Clients_UserId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Countries_CountryInfoKey",
                table: "Clients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countries",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clients",
                table: "Clients");

            migrationBuilder.RenameTable(
                name: "Countries",
                newName: "UCountry");

            migrationBuilder.RenameTable(
                name: "Clients",
                newName: "Client");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_CountryInfoKey",
                table: "Client",
                newName: "IX_Client_CountryInfoKey");

            migrationBuilder.AddColumn<int>(
                name: "UserAuthBankId",
                table: "Client",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UCountry",
                table: "UCountry",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Client",
                table: "Client",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "BankOperationHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumberAccountRecepient = table.Column<string>(type: "text", nullable: true),
                    NumberAccountSending = table.Column<string>(type: "text", nullable: true),
                    operationType = table.Column<int>(type: "integer", nullable: false),
                    TransferSum = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankOperationHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAuthBank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    AccessLevel = table.Column<int>(type: "integer", nullable: false),
                    IsEmailValidate = table.Column<bool>(type: "boolean", nullable: false),
                    ValidateCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuthBank", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Client_UserId",
                table: "Accounts",
                column: "UserId",
                principalTable: "Client",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Client_UCountry_CountryInfoKey",
                table: "Client",
                column: "CountryInfoKey",
                principalTable: "UCountry",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Client_UserId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Client_UCountry_CountryInfoKey",
                table: "Client");

            migrationBuilder.DropTable(
                name: "BankOperationHistory");

            migrationBuilder.DropTable(
                name: "UserAuthBank");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UCountry",
                table: "UCountry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Client",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "UserAuthBankId",
                table: "Client");

            migrationBuilder.RenameTable(
                name: "UCountry",
                newName: "Countries");

            migrationBuilder.RenameTable(
                name: "Client",
                newName: "Clients");

            migrationBuilder.RenameIndex(
                name: "IX_Client_CountryInfoKey",
                table: "Clients",
                newName: "IX_Clients_CountryInfoKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Countries",
                table: "Countries",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clients",
                table: "Clients",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Clients_UserId",
                table: "Accounts",
                column: "UserId",
                principalTable: "Clients",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Countries_CountryInfoKey",
                table: "Clients",
                column: "CountryInfoKey",
                principalTable: "Countries",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
