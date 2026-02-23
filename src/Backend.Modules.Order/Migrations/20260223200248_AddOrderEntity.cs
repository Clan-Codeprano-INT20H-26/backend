using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Modules.Order.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    KitId = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Longitude = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Taxes_StateRate = table.Column<decimal>(type: "numeric", nullable: true),
                    Taxes_CountryRate = table.Column<decimal>(type: "numeric", nullable: true),
                    Taxes_CityRate = table.Column<decimal>(type: "numeric", nullable: true),
                    Taxes_SpecialRates = table.Column<decimal>(type: "numeric", nullable: true),
                    Taxes_Jurisdictions = table.Column<List<string>>(type: "text[]", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
