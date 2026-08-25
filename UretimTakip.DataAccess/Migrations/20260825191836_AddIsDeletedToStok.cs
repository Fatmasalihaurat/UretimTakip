using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UretimTakip.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToStok : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Stoklar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Depolar",
                keyColumn: "DepoId",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                column: "OlusturulmaTarihi",
                value: new DateTime(2026, 8, 25, 19, 18, 35, 498, DateTimeKind.Utc).AddTicks(2775));

            migrationBuilder.UpdateData(
                table: "Stoklar",
                keyColumn: "StokId",
                keyValue: new Guid("11223344-5566-7788-9900-aabbccddeeff"),
                columns: new[] { "IsDeleted", "OlusturulmaTarihi", "SonGuncellenmeTarihi" },
                values: new object[] { false, new DateTime(2026, 8, 25, 19, 18, 35, 498, DateTimeKind.Utc).AddTicks(3327), new DateTime(2026, 8, 25, 19, 18, 35, 498, DateTimeKind.Utc).AddTicks(3321) });

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "UrunId",
                keyValue: new Guid("f6e5d4c3-b2a1-0f9e-8d7c-6b5a4f3db1ba"),
                column: "OlusturulmaTarihi",
                value: new DateTime(2026, 8, 25, 19, 18, 35, 498, DateTimeKind.Utc).AddTicks(3242));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stoklar");

            migrationBuilder.UpdateData(
                table: "Depolar",
                keyColumn: "DepoId",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                column: "OlusturulmaTarihi",
                value: new DateTime(2026, 8, 25, 18, 57, 48, 803, DateTimeKind.Utc).AddTicks(2527));

            migrationBuilder.UpdateData(
                table: "Stoklar",
                keyColumn: "StokId",
                keyValue: new Guid("11223344-5566-7788-9900-aabbccddeeff"),
                columns: new[] { "OlusturulmaTarihi", "SonGuncellenmeTarihi" },
                values: new object[] { new DateTime(2026, 8, 25, 18, 57, 48, 803, DateTimeKind.Utc).AddTicks(2947), new DateTime(2026, 8, 25, 18, 57, 48, 803, DateTimeKind.Utc).AddTicks(2942) });

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "UrunId",
                keyValue: new Guid("f6e5d4c3-b2a1-0f9e-8d7c-6b5a4f3db1ba"),
                column: "OlusturulmaTarihi",
                value: new DateTime(2026, 8, 25, 18, 57, 48, 803, DateTimeKind.Utc).AddTicks(2874));
        }
    }
}
