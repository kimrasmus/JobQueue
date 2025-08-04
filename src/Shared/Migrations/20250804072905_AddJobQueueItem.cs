using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddJobQueueItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HardRetryCount",
                table: "JobQueueItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_HardRetryCount",
                table: "JobQueueItems",
                column: "HardRetryCount");

            migrationBuilder.CreateIndex(
                name: "IX_JobQueueItems_RetryCount",
                table: "JobQueueItems",
                column: "RetryCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobQueueItems_HardRetryCount",
                table: "JobQueueItems");

            migrationBuilder.DropIndex(
                name: "IX_JobQueueItems_RetryCount",
                table: "JobQueueItems");

            migrationBuilder.DropColumn(
                name: "HardRetryCount",
                table: "JobQueueItems");
        }
    }
}
