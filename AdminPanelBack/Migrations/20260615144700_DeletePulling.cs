using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPanelBack.Migrations
{
    /// <inheritdoc />
    public partial class DeletePulling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSentToOperator",
                table: "Feedbacks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSentToOperator",
                table: "Feedbacks",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
