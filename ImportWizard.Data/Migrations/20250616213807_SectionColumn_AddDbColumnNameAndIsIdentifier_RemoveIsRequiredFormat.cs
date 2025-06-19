using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImportWizard.Data.Migrations
{
    /// <inheritdoc />
    public partial class SectionColumn_AddDbColumnNameAndIsIdentifier_RemoveIsRequiredFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // drop the old columns
            migrationBuilder.DropColumn(
                name: "IsRequired",
                schema: "imp",
                table: "SectionColumn");

            migrationBuilder.DropColumn(
                name: "Format",
                schema: "imp",
                table: "SectionColumn");

            // add the new ones
            migrationBuilder.AddColumn<string>(
                name: "DbColumnName",
                schema: "imp",
                table: "SectionColumn",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsIdentifier",
                schema: "imp",
                table: "SectionColumn",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // roll back the schema change
            migrationBuilder.DropColumn(
                name: "IsIdentifier",
                schema: "imp",
                table: "SectionColumn");

            migrationBuilder.DropColumn(
                name: "DbColumnName",
                schema: "imp",
                table: "SectionColumn");

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                schema: "imp",
                table: "SectionColumn",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Format",
                schema: "imp",
                table: "SectionColumn",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

    }
}
