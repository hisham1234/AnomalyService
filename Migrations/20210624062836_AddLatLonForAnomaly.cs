using Microsoft.EntityFrameworkCore.Migrations;

namespace AnomalyService.Migrations
{
    public partial class AddLatLonForAnomaly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Anomalys",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Anomalys",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Anomalys");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Anomalys");
        }
    }
}
