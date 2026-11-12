using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AlignClaimsWithReportAndFrontend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_vehicle_claims_claim_number",
                table: "vehicle_claims");

            migrationBuilder.DropColumn(
                name: "claim_number",
                table: "vehicle_claims");

            migrationBuilder.AddColumn<decimal>(
                name: "estimated_damage_amount",
                table: "vehicle_claims",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "incident_location",
                table: "vehicle_claims",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "reported_at",
                table: "vehicle_claims",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "submitted_at",
                table: "indemnity_requests",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estimated_damage_amount",
                table: "vehicle_claims");

            migrationBuilder.DropColumn(
                name: "incident_location",
                table: "vehicle_claims");

            migrationBuilder.DropColumn(
                name: "reported_at",
                table: "vehicle_claims");

            migrationBuilder.DropColumn(
                name: "submitted_at",
                table: "indemnity_requests");

            migrationBuilder.AddColumn<string>(
                name: "claim_number",
                table: "vehicle_claims",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "i_x_vehicle_claims_claim_number",
                table: "vehicle_claims",
                column: "claim_number",
                unique: true);
        }
    }
}
