using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddHasShareholderColumnToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // HasShareholder kolonu yoksa ekle, varsa güncelle
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Properties') AND name = 'HasShareholder')
                BEGIN
                    ALTER TABLE Properties ADD HasShareholder bit NOT NULL DEFAULT(0);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Properties') AND name = 'HasShareholder')
                BEGIN
                    ALTER TABLE Properties DROP COLUMN HasShareholder;
                END
            ");
        }
    }
}
