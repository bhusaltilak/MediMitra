using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediMitra.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "registerModels",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$D/INkLVcmcE8DnmINsTp0ec79zCZtmlxdC9kxPjtfRcdGN3f18yTi");

            migrationBuilder.UpdateData(
                table: "registerModels",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$J9vJkNRb7xFixSKVEiD7TuYeVftFDZfZzZd9LPWkM3a1nRSOEnPKO");

            migrationBuilder.UpdateData(
                table: "registerModels",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$59LbetCZhg00AsRR7LotfehWvxBiQTcvJngKm4Cfc35TdkUOM2gsW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "registerModels",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$rvRNkvhCu/NIncm/vyLWmOmbEPiC.RVUYQIp7I6lT8ApTFYycgLcS");

            migrationBuilder.UpdateData(
                table: "registerModels",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$No09Dq2h44/BNomdkxNFdOU8ij9oF1cj9eYqTxDR8n0egS3Sq0GR2");

            migrationBuilder.UpdateData(
                table: "registerModels",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$lPDoekBf5BkaYhyhlGOlFuCy3XC1rZ5Yn8Mk6WoobYbg6jJQFKBze");
        }
    }
}
