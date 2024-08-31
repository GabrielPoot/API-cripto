using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_cripto.Migrations
{
    public partial class AddApiIdToCryptocurrencies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiId",
                table: "Cryptocurrencies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiId",
                table: "Cryptocurrencies");
        }
    }
}
