using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_StationRentalSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserReports",
                columns: table => new
                {
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReporterId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ReporterEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReporterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReportType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelatedStationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedRentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttachmentUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AssignedToStaffId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    AssignedToStaffName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StaffNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserRating = table.Column<int>(type: "int", nullable: true),
                    UserFeedback = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReports", x => x.ReportId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserReports");
        }
    }
}
