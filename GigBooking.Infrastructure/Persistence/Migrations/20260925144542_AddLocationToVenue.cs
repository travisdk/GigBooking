using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GigBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToVenue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location_City",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location_CountryCode",
                table: "Venues",
                type: "nchar(2)",
                fixedLength: true,
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location_PostalCode",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location_Street",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location_City",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "Location_CountryCode",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "Location_PostalCode",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "Location_Street",
                table: "Venues");
        }
    }
}
