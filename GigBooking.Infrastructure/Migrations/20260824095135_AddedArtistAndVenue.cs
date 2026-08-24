using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GigBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedArtistAndVenue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Gigs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldCollation: "NOCASE");

            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gigs_ArtistId",
                table: "Gigs",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Gigs_VenueId",
                table: "Gigs",
                column: "VenueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gigs_Artists_ArtistId",
                table: "Gigs",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gigs_Venues_VenueId",
                table: "Gigs",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gigs_Artists_ArtistId",
                table: "Gigs");

            migrationBuilder.DropForeignKey(
                name: "FK_Gigs_Venues_VenueId",
                table: "Gigs");

            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.DropIndex(
                name: "IX_Gigs_ArtistId",
                table: "Gigs");

            migrationBuilder.DropIndex(
                name: "IX_Gigs_VenueId",
                table: "Gigs");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Gigs",
                type: "TEXT",
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(Guid),
                oldType: "TEXT");
        }
    }
}
