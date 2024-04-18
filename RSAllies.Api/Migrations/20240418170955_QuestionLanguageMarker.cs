using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RSAllies.Api.Migrations
{
    /// <inheritdoc />
    public partial class QuestionLanguageMarker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnglish",
                table: "Questions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnglish",
                table: "Questions");
        }
    }
}
