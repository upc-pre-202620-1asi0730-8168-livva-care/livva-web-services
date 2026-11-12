using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicyManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "policies",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    application_id = table.Column<long>(type: "bigint", nullable: false),
                    policy_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    insurance_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    expiration_date = table.Column<DateTime>(type: "date", nullable: false),
                    premium_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    coverage_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_policies", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "policy_coverages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    coverage_limit = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_policy_coverages", x => x.id);
                    table.ForeignKey(
                        name: "fk_policy_coverages_policy",
                        column: x => x.policy_id,
                        principalTable: "policies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "policy_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    document_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    file_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_policy_documents", x => x.id);
                    table.ForeignKey(
                        name: "fk_policy_documents_policy",
                        column: x => x.policy_id,
                        principalTable: "policies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "renewals",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    proposed_expiration_date = table.Column<DateTime>(type: "date", nullable: true),
                    proposed_premium = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    resolved_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_renewals", x => x.id);
                    table.ForeignKey(
                        name: "fk_renewals_policy",
                        column: x => x.policy_id,
                        principalTable: "policies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "policies",
                columns: new[] { "id", "application_id", "coverage_amount", "created_at", "expiration_date", "insurance_type", "policy_number", "premium_amount", "start_date", "status", "updated_at", "user_id" },
                values: new object[,]
                {
                    { 1L, 1L, 60000m, new DateTimeOffset(new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTime(2027, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "VEHICLE", "LIV-VEH-0001", 1200m, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "ACTIVE", new DateTimeOffset(new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 2L, 2L, 100000m, new DateTimeOffset(new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTime(2027, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "LIFE", "LIV-LIF-0002", 960m, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ACTIVE", new DateTimeOffset(new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 3L, 3L, 45000m, new DateTimeOffset(new DateTime(2025, 8, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTime(2026, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "VEHICLE", "LIV-VEH-0003", 1080m, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "EXPIRED", new DateTimeOffset(new DateTime(2026, 7, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.InsertData(
                table: "policy_coverages",
                columns: new[] { "id", "coverage_limit", "created_at", "description", "name", "policy_id", "updated_at" },
                values: new object[,]
                {
                    { 1L, 60000m, new DateTimeOffset(new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Coverage for damage to the insured vehicle.", "Vehicle damage", 1L, new DateTimeOffset(new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2L, 30000m, new DateTimeOffset(new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Coverage for damage caused to third parties.", "Third-party liability", 1L, new DateTimeOffset(new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3L, 100000m, new DateTimeOffset(new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Financial protection for registered beneficiaries.", "Life coverage", 2L, new DateTimeOffset(new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "policy_documents",
                columns: new[] { "id", "created_at", "document_type", "file_url", "name", "policy_id", "updated_at" },
                values: new object[,]
                {
                    { 1L, new DateTimeOffset(new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "policy", "", "Vehicle policy document", 1L, new DateTimeOffset(new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2L, new DateTimeOffset(new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "policy", "", "Life policy document", 2L, new DateTimeOffset(new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "i_x_policies_policy_number",
                table: "policies",
                column: "policy_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_policy_coverages_policy_id",
                table: "policy_coverages",
                column: "policy_id");

            migrationBuilder.CreateIndex(
                name: "i_x_policy_documents_policy_id",
                table: "policy_documents",
                column: "policy_id");

            migrationBuilder.CreateIndex(
                name: "i_x_renewals_policy_id",
                table: "renewals",
                column: "policy_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "policy_coverages");

            migrationBuilder.DropTable(
                name: "policy_documents");

            migrationBuilder.DropTable(
                name: "renewals");

            migrationBuilder.DropTable(
                name: "policies");
        }
    }
}
