using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_StationRentalSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RentalOrders",
                columns: table => new
                {
                    RentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchStartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchEndId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalOrders", x => x.RentalId);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackRatings",
                columns: table => new
                {
                    FeedbackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackRatings", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_FeedbackRatings_RentalOrders_RentalId",
                        column: x => x.RentalId,
                        principalTable: "RentalOrders",
                        principalColumn: "RentalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionRef = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_RentalOrders_RentalId",
                        column: x => x.RentalId,
                        principalTable: "RentalOrders",
                        principalColumn: "RentalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PenaltyRecords",
                columns: table => new
                {
                    PenaltyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IssuedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenaltyRecords", x => x.PenaltyId);
                    table.ForeignKey(
                        name: "FK_PenaltyRecords_RentalOrders_RentalId",
                        column: x => x.RentalId,
                        principalTable: "RentalOrders",
                        principalColumn: "RentalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalContracts",
                columns: table => new
                {
                    ContractId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignedByStaff = table.Column<int>(type: "int", nullable: false),
                    SignedByRenter = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalContracts", x => x.ContractId);
                    table.ForeignKey(
                        name: "FK_RentalContracts_RentalOrders_RentalId",
                        column: x => x.RentalId,
                        principalTable: "RentalOrders",
                        principalColumn: "RentalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalOrderDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalOrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalOrderDetail_RentalOrders_RentalOrderId",
                        column: x => x.RentalOrderId,
                        principalTable: "RentalOrders",
                        principalColumn: "RentalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Checkins",
                columns: table => new
                {
                    CheckinId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalOrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Datetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdometerReading = table.Column<int>(type: "int", nullable: false),
                    BatteryLevel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RentalOrderRentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkins", x => x.CheckinId);
                    table.ForeignKey(
                        name: "FK_Checkins_RentalOrderDetail_RentalOrderDetailId",
                        column: x => x.RentalOrderDetailId,
                        principalTable: "RentalOrderDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checkins_RentalOrders_RentalOrderRentalId",
                        column: x => x.RentalOrderRentalId,
                        principalTable: "RentalOrders",
                        principalColumn: "RentalId");
                });

            migrationBuilder.CreateTable(
                name: "Checkouts",
                columns: table => new
                {
                    CheckoutId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalOrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Datetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdometerReading = table.Column<int>(type: "int", nullable: false),
                    BatteryLevel = table.Column<int>(type: "int", nullable: false),
                    ExtraFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RentalOrderRentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkouts", x => x.CheckoutId);
                    table.ForeignKey(
                        name: "FK_Checkouts_RentalOrderDetail_RentalOrderDetailId",
                        column: x => x.RentalOrderDetailId,
                        principalTable: "RentalOrderDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checkouts_RentalOrders_RentalOrderRentalId",
                        column: x => x.RentalOrderRentalId,
                        principalTable: "RentalOrders",
                        principalColumn: "RentalId");
                });

            migrationBuilder.CreateTable(
                name: "PhotoProofs",
                columns: table => new
                {
                    PhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckinId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CheckoutId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RentalOrderRentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CapturedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoProofs", x => x.PhotoId);
                    table.ForeignKey(
                        name: "FK_PhotoProofs_Checkins_CheckinId",
                        column: x => x.CheckinId,
                        principalTable: "Checkins",
                        principalColumn: "CheckinId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhotoProofs_Checkouts_CheckoutId",
                        column: x => x.CheckoutId,
                        principalTable: "Checkouts",
                        principalColumn: "CheckoutId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhotoProofs_RentalOrders_RentalOrderRentalId",
                        column: x => x.RentalOrderRentalId,
                        principalTable: "RentalOrders",
                        principalColumn: "RentalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checkins_RentalOrderDetailId",
                table: "Checkins",
                column: "RentalOrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkins_RentalOrderRentalId",
                table: "Checkins",
                column: "RentalOrderRentalId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_RentalOrderDetailId",
                table: "Checkouts",
                column: "RentalOrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_RentalOrderRentalId",
                table: "Checkouts",
                column: "RentalOrderRentalId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackRatings_RentalId",
                table: "FeedbackRatings",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RentalId",
                table: "Payments",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyRecords_RentalId",
                table: "PenaltyRecords",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoProofs_CheckinId",
                table: "PhotoProofs",
                column: "CheckinId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoProofs_CheckoutId",
                table: "PhotoProofs",
                column: "CheckoutId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoProofs_RentalOrderRentalId",
                table: "PhotoProofs",
                column: "RentalOrderRentalId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalContracts_RentalId",
                table: "RentalContracts",
                column: "RentalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentalOrderDetail_RentalOrderId",
                table: "RentalOrderDetail",
                column: "RentalOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedbackRatings");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PenaltyRecords");

            migrationBuilder.DropTable(
                name: "PhotoProofs");

            migrationBuilder.DropTable(
                name: "RentalContracts");

            migrationBuilder.DropTable(
                name: "Checkins");

            migrationBuilder.DropTable(
                name: "Checkouts");

            migrationBuilder.DropTable(
                name: "RentalOrderDetail");

            migrationBuilder.DropTable(
                name: "RentalOrders");
        }
    }
}
