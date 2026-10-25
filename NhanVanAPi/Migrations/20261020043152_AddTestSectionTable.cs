using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhanVanAPi.Migrations
{
    /// <inheritdoc />
    public partial class AddTestSectionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestBankId = table.Column<int>(type: "int", nullable: false),
                    SectionNumber = table.Column<int>(type: "int", nullable: false),
                    AudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PassageText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSections_TestBank_TestBankId",
                        column: x => x.TestBankId,
                        principalTable: "TestBank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestSections_TestBankId_SectionNumber",
                table: "TestSections",
                columns: new[] { "TestBankId", "SectionNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestSections");
        }
    }
}
