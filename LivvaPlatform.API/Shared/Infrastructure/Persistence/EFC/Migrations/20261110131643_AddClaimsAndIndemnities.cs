using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimsAndIndemnities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "indemnity_requests",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    beneficiary_id = table.Column<long>(type: "bigint", nullable: false),
                    request_date = table.Column<DateTime>(type: "date", nullable: false),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    indemnity_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    resolved_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_indemnity_requests", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vehicle_claims",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    claim_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    incident_date = table.Column<DateTime>(type: "date", nullable: false),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    compensation_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    resolved_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_vehicle_claims", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "indemnity_status_histories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    indemnity_request_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    changed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_indemnity_status_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_indemnity_status_histories_request",
                        column: x => x.indemnity_request_id,
                        principalTable: "indemnity_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "claim_status_histories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    claim_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    changed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_claim_status_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_claim_status_histories_vehicle_claim",
                        column: x => x.claim_id,
                        principalTable: "vehicle_claims",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_claim_status_histories_claim_id",
                table: "claim_status_histories",
                column: "claim_id");

            migrationBuilder.CreateIndex(
                name: "i_x_indemnity_requests_beneficiary_id",
                table: "indemnity_requests",
                column: "beneficiary_id");

            migrationBuilder.CreateIndex(
                name: "i_x_indemnity_requests_policy_id",
                table: "indemnity_requests",
                column: "policy_id");

            migrationBuilder.CreateIndex(
                name: "i_x_indemnity_status_histories_indemnity_request_id",
                table: "indemnity_status_histories",
                column: "indemnity_request_id");

            migrationBuilder.CreateIndex(
                name: "i_x_vehicle_claims_claim_number",
                table: "vehicle_claims",
                column: "claim_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_vehicle_claims_policy_id",
                table: "vehicle_claims",
                column: "policy_id");

            migrationBuilder.CreateIndex(
                name: "i_x_vehicle_claims_user_id",
                table: "vehicle_claims",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "claim_status_histories");

            migrationBuilder.DropTable(
                name: "indemnity_status_histories");

            migrationBuilder.DropTable(
                name: "vehicle_claims");

            migrationBuilder.DropTable(
                name: "indemnity_requests");
        }
    }
}
