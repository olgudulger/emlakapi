using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddHasShareholderPropertyNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // HasShareholder kolonu ekle (VERİ GÜVENLİ - mevcut kayıtlar korunur)
            migrationBuilder.AddColumn<bool>(
                name: "HasShareholder",
                table: "Properties",
                type: "bit",
                nullable: true,
                defaultValue: false);
                
            // Mevcut kayıtların NULL değerlerini false yap (VERİ GÜVENLİ)
            migrationBuilder.Sql(
                "UPDATE Properties SET HasShareholder = 0 WHERE HasShareholder IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasShareholder",
                table: "Properties");
        }
    }
}
