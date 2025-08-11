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
    ALTER TABLE [dbo].[Properties] ADD [HasShareholder] bit NULL;
    UPDATE [dbo].[Properties] SET [HasShareholder] = 0 WHERE [HasShareholder] IS NULL;
    IF NOT EXISTS (
        SELECT 1 FROM sys.default_constraints dc
        JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
        WHERE OBJECT_NAME(dc.parent_object_id) = 'Properties' AND c.name = 'HasShareholder'
    )
    BEGIN
        ALTER TABLE [dbo].[Properties] ADD CONSTRAINT [DF_Properties_HasShareholder] DEFAULT (0) FOR [HasShareholder];
    END
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Idempotent: Varsa default constraint'i bulup kaldır, sonra kolonu kaldır
            migrationBuilder.Sql(@"
DECLARE @dfName nvarchar(128);
SELECT @dfName = dc.name
FROM sys.default_constraints dc
JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
WHERE OBJECT_NAME(dc.parent_object_id) = 'Properties' AND c.name = 'HasShareholder';

IF @dfName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE [dbo].[Properties] DROP CONSTRAINT [' + @dfName + ']');
END

IF COL_LENGTH('dbo.Properties', 'HasShareholder') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Properties] DROP COLUMN [HasShareholder];
END
");
        }
    }
}
