using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class PerformanceOptimizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserActivities_UserId",
                table: "UserActivities");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_PropertyPriceHistories_PropertyId",
                table: "PropertyPriceHistories");

            migrationBuilder.DropIndex(
                name: "IX_Properties_CustomerId",
                table: "Properties");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Sales",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Properties",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Properties",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "InterestType",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerType",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivity_UserId_ActivityDate",
                table: "UserActivities",
                columns: new[] { "UserId", "ActivityDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Sale_Status_SaleDate",
                table: "Sales",
                columns: new[] { "Status", "SaleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Sale_Status_UserId",
                table: "Sales",
                columns: new[] { "Status", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sale_UserId_SaleDate",
                table: "Sales",
                columns: new[] { "UserId", "SaleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Token_Unique",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId_ExpiresAt",
                table: "RefreshTokens",
                columns: new[] { "UserId", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyPriceHistory_PropertyId_Date",
                table: "PropertyPriceHistories",
                columns: new[] { "PropertyId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Property_CustomerId_Status",
                table: "Properties",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Property_Location_Status",
                table: "Properties",
                columns: new[] { "ProvinceId", "DistrictId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Property_Status_CreatedAt",
                table: "Properties",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Property_Title",
                table: "Properties",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Property_Type_Status",
                table: "Properties",
                columns: new[] { "PropertyType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_FullName",
                table: "Customers",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerType",
                table: "Customers",
                column: "CustomerType");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_InterestType",
                table: "Customers",
                column: "InterestType");

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_IsOnline",
                table: "AspNetUsers",
                column: "IsOnline");

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_IsOnline_LastActivityDate",
                table: "AspNetUsers",
                columns: new[] { "IsOnline", "LastActivityDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserActivity_UserId_ActivityDate",
                table: "UserActivities");

            migrationBuilder.DropIndex(
                name: "IX_Sale_Status_SaleDate",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sale_Status_UserId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sale_UserId_SaleDate",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_Token_Unique",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_UserId_ExpiresAt",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_PropertyPriceHistory_PropertyId_Date",
                table: "PropertyPriceHistories");

            migrationBuilder.DropIndex(
                name: "IX_Property_CustomerId_Status",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Property_Location_Status",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Property_Status_CreatedAt",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Property_Title",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Property_Type_Status",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Customer_FullName",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CustomerType",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_InterestType",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_AppUser_IsOnline",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AppUser_IsOnline_LastActivityDate",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Sales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "InterestType",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerType",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_UserId",
                table: "UserActivities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyPriceHistories_PropertyId",
                table: "PropertyPriceHistories",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_CustomerId",
                table: "Properties",
                column: "CustomerId");
        }
    }
}
