using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QbQuestionsAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QbQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Level = table.Column<byte>(nullable: false),
                    Tournament = table.Column<string>(maxLength: 50, nullable: false),
                    Year = table.Column<int>(nullable: false),
                    Power = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: false),
                    Answer = table.Column<string>(maxLength: 50, nullable: false),
                    Notes = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QbQuestions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QbQuestions");
        }
    }
}
