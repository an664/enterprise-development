using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VeterinaryClinic.Infrastructure.PostgreSQL.Migrations;

/// <inheritdoc />
public partial class InitialSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "breeds",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                species = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_breeds", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "owners",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_owners", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "specializations",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                species = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_specializations", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "pets",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                breed_id = table.Column<int>(type: "integer", nullable: false),
                owner_id = table.Column<int>(type: "integer", nullable: false),
                birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                weight = table.Column<decimal>(type: "numeric", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_pets", x => x.id);
                table.ForeignKey(
                    name: "FK_pets_breeds_breed_id",
                    column: x => x.breed_id,
                    principalTable: "breeds",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_pets_owners_owner_id",
                    column: x => x.owner_id,
                    principalTable: "owners",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "veterinarians",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                passport_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                birth_year = table.Column<int>(type: "integer", nullable: false),
                specialization_id = table.Column<int>(type: "integer", nullable: false),
                experience_years = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_veterinarians", x => x.id);
                table.ForeignKey(
                    name: "FK_veterinarians_specializations_specialization_id",
                    column: x => x.specialization_id,
                    principalTable: "specializations",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "appointments",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                pet_id = table.Column<int>(type: "integer", nullable: false),
                veterinarian_id = table.Column<int>(type: "integer", nullable: false),
                starts_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                room_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                is_follow_up = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_appointments", x => x.id);
                table.ForeignKey(
                    name: "FK_appointments_pets_pet_id",
                    column: x => x.pet_id,
                    principalTable: "pets",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_appointments_veterinarians_veterinarian_id",
                    column: x => x.veterinarian_id,
                    principalTable: "veterinarians",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_appointments_pet_id",
            table: "appointments",
            column: "pet_id");

        migrationBuilder.CreateIndex(
            name: "IX_appointments_veterinarian_id",
            table: "appointments",
            column: "veterinarian_id");

        migrationBuilder.CreateIndex(
            name: "ux_appointments_room_start",
            table: "appointments",
            columns: new[] { "room_number", "starts_at" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_pets_breed_id",
            table: "pets",
            column: "breed_id");

        migrationBuilder.CreateIndex(
            name: "IX_pets_owner_id",
            table: "pets",
            column: "owner_id");

        migrationBuilder.CreateIndex(
            name: "IX_veterinarians_specialization_id",
            table: "veterinarians",
            column: "specialization_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "appointments");

        migrationBuilder.DropTable(
            name: "pets");

        migrationBuilder.DropTable(
            name: "veterinarians");

        migrationBuilder.DropTable(
            name: "breeds");

        migrationBuilder.DropTable(
            name: "owners");

        migrationBuilder.DropTable(
            name: "specializations");
    }
}
