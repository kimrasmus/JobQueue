using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobQueueItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentJobId = table.Column<int>(type: "int", nullable: true),
                    RunId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    QueueId = table.Column<int>(type: "int", nullable: false),
                    JobType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    JobState = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    InputPayload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingContext = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutionResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    JobStatus = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    StatusDetail = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CreatedByUser = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobQueueItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_JobStatus",
                table: "JobQueueItems",
                column: "JobStatus");

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_JobStatus_ScheduledAt_JobType",
                table: "JobQueueItems",
                columns: new[] { "JobStatus", "ScheduledAt", "JobType" });

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_JobStatus_ScheduledAt_JobType_Id",
                table: "JobQueueItems",
                columns: new[] { "JobStatus", "ScheduledAt", "JobType", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_JobType",
                table: "JobQueueItems",
                column: "JobType");

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_ParentJobId",
                table: "JobQueueItems",
                column: "ParentJobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_RunId",
                table: "JobQueueItems",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_RunId_JobStatus_ScheduledAt",
                table: "JobQueueItems",
                columns: new[] { "RunId", "JobStatus", "ScheduledAt" });

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_ScheduledAt",
                table: "JobQueueItems",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_StartedAt",
                table: "JobQueueItems",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_StatusDetail",
                table: "JobQueueItems",
                column: "StatusDetail");

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_UpdatedAt",
                table: "JobQueueItems",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobQueueItems");
        }
    }
}
