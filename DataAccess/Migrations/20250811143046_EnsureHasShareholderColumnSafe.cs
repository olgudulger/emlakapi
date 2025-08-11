using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EnsureHasShareholderColumnSafe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent: Kolon yoksa ekle, varsa dokunma
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Properties', 'HasShareholder') IS NULL
BEGIN
    ALTER TABLE [dbo].[Properties] ADD [HasShareholder] bit NOT NULL DEFAULT(0);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Idempotent: Varsa default constraint'i bulup kaldır, sonra kolonu kaldır
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Properties', 'HasShareholder') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Properties] DROP COLUMN [HasShareholder];
END
");
        }
    }
}
