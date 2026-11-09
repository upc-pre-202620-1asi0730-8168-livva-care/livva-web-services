using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddInsuranceOfferingAndApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "beneficiaries",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    policy_id = table.Column<long>(type: "bigint", nullable: false),
                    full_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    document_type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    document_number = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    relationship = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    birth_date = table.Column<DateTime>(type: "date", nullable: true),
                    participation_percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_beneficiaries", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "insurers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    external_reference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_insurers", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    license_plate = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    brand = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    model = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    manufacture_year = table.Column<int>(type: "int", nullable: false),
                    estimated_value = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_vehicles", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "insurance_products",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    insurer_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    insurance_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    coverage_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    premium_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_insurance_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_insurance_products_insurer",
                        column: x => x.insurer_id,
                        principalTable: "insurers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "insurance_applications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    requested_coverage = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_insurance_applications", x => x.id);
                    table.ForeignKey(
                        name: "fk_insurance_applications_product",
                        column: x => x.product_id,
                        principalTable: "insurance_products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "life_insurance_applications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    application_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_life_insurance_applications", x => x.id);
                    table.ForeignKey(
                        name: "fk_life_insurance_applications_application",
                        column: x => x.application_id,
                        principalTable: "insurance_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vehicle_insurance_applications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    application_id = table.Column<long>(type: "bigint", nullable: false),
                    vehicle_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_vehicle_insurance_applications", x => x.id);
                    table.ForeignKey(
                        name: "fk_vehicle_insurance_applications_application",
                        column: x => x.application_id,
                        principalTable: "insurance_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_vehicle_insurance_applications_vehicle",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "insurers",
                columns: new[] { "id", "active", "created_at", "external_reference", "name", "updated_at" },
                values: new object[,]
                {
                    { 1L, true, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "INS-ANDINA", "Protección Andina", new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2L, true, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "INS-HORIZONTE", "Horizonte Seguros", new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3L, true, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "INS-CONFIANZA", "Confianza Vida", new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "insurance_products",
                columns: new[] { "id", "active", "coverage_amount", "created_at", "insurance_type", "insurer_id", "name", "premium_amount", "updated_at" },
                values: new object[,]
                {
                    { 1L, true, 35000m, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "VEHICLE", 1L, "Auto Esencial", 650m, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2L, true, 70000m, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "VEHICLE", 1L, "Auto Integral", 1100m, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3L, true, 100000m, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "VEHICLE", 2L, "Vehículo Total", 1450m, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 4L, true, 80000m, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "LIFE", 2L, "Vida Familiar", 780m, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 5L, true, 120000m, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "LIFE", 3L, "Vida Protección", 1050m, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 6L, true, 200000m, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "LIFE", 3L, "Vida Plus", 1600m, new DateTimeOffset(new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "i_x_insurance_applications_product_id",
                table: "insurance_applications",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "i_x_insurance_products_insurer_id",
                table: "insurance_products",
                column: "insurer_id");

            migrationBuilder.CreateIndex(
                name: "i_x_insurers_external_reference",
                table: "insurers",
                column: "external_reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_life_insurance_applications_application_id",
                table: "life_insurance_applications",
                column: "application_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_vehicle_insurance_applications_application_id",
                table: "vehicle_insurance_applications",
                column: "application_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_vehicle_insurance_applications_vehicle_id",
                table: "vehicle_insurance_applications",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "i_x_vehicles_license_plate",
                table: "vehicles",
                column: "license_plate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "beneficiaries");

            migrationBuilder.DropTable(
                name: "life_insurance_applications");

            migrationBuilder.DropTable(
                name: "vehicle_insurance_applications");

            migrationBuilder.DropTable(
                name: "insurance_applications");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropTable(
                name: "insurance_products");

            migrationBuilder.DropTable(
                name: "insurers");
        }
    }
}
