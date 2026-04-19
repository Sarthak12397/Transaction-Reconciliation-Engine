using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionReconcillationEngine.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InternalRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReconciliationRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InternalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    NextRetryAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastAttemptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaxRetryCount = table.Column<int>(type: "integer", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconciliationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReconciliationRecords_ExternalRecords_ExternalRecordId",
                        column: x => x.ExternalRecordId,
                        principalTable: "ExternalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReconciliationRecords_InternalRecords_InternalRecordId",
                        column: x => x.InternalRecordId,
                        principalTable: "InternalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InternalRecords_TransactionId",
                table: "InternalRecords",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationRecords_ExternalRecordId",
                table: "ReconciliationRecords",
                column: "ExternalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationRecords_InternalRecordId",
                table: "ReconciliationRecords",
                column: "InternalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationRecords_NextRetryAt",
                table: "ReconciliationRecords",
                column: "NextRetryAt");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationRecords_Status",
                table: "ReconciliationRecords",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReconciliationRecords");

            migrationBuilder.DropTable(
                name: "ExternalRecords");

            migrationBuilder.DropTable(
                name: "InternalRecords");
        }
    }
}
