using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPanelBack.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedbackAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedAdminId",
                table: "Feedbacks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedAdminName",
                table: "Feedbacks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedAdminId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "AssignedAdminName",
                table: "Feedbacks");
        }
    }
}
