using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImportWizard.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSectionColumnColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            // drop the old columns
            mb.DropColumn(name: "IsRequired", schema: "imp", table: "SectionColumn");
            mb.DropColumn(name: "Format", schema: "imp", table: "SectionColumn");

            // add the new ones
            mb.AddColumn<string>(
                name: "DbColumnName",
                schema: "imp",
                table: "SectionColumn",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            mb.AddColumn<bool>(
                name: "IsIdentifier",
                schema: "imp",
                table: "SectionColumn",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder mb)
        {
            // undo the fix
            mb.DropColumn(name: "IsIdentifier", schema: "imp", table: "SectionColumn");
            mb.DropColumn(name: "DbColumnName", schema: "imp", table: "SectionColumn");

            mb.AddColumn<bool>(
                name: "IsRequired",
                schema: "imp",
                table: "SectionColumn",
                type: "bit",
                nullable: false,
                defaultValue: false);

            mb.AddColumn<string>(
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
