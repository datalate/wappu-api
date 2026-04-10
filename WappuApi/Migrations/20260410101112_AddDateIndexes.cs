using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WappuApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDateIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tracks_PlayedAt",
                table: "Tracks",
                column: "PlayedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_EndAt",
                table: "Programs",
                column: "EndAt");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_StartAt",
                table: "Programs",
                column: "StartAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tracks_PlayedAt",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Programs_EndAt",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_Programs_StartAt",
                table: "Programs");
        }
    }
}
