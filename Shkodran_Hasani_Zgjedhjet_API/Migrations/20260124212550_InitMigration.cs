using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shkodran_Hasani_Zgjedhjet_API.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Zgjedhjet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kategoria = table.Column<int>(type: "int", nullable: false),
                    Komuna = table.Column<int>(type: "int", nullable: false),
                    QendraVotimit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VendVotimi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Partia = table.Column<int>(type: "int", nullable: false),
                    Vota = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zgjedhjet", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zgjedhjet");
        }
    }
}
