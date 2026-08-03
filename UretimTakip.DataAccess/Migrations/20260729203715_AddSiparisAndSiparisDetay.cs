using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UretimTakip.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSiparisAndSiparisDetay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Siparis",
                columns: table => new
                {
                    SiparisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiparisNumarasi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CariId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiparisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToplamTutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siparis", x => x.SiparisId);
                });

            migrationBuilder.CreateTable(
                name: "SiparisDetay",
                columns: table => new
                {
                    SiparisDetayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiparisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UrunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Miktar = table.Column<int>(type: "int", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisDetay", x => x.SiparisDetayId);
                });

            migrationBuilder.UpdateData(
                table: "Depolar",
                keyColumn: "DepoId",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                column: "OlusturulmaTarihi",
                value: new DateTime(2026, 7, 29, 20, 37, 14, 602, DateTimeKind.Utc).AddTicks(845));

            migrationBuilder.UpdateData(
                table: "Stoklar",
                keyColumn: "StokId",
                keyValue: new Guid("11223344-5566-7788-9900-aabbccddeeff"),
                columns: new[] { "OlusturulmaTarihi", "SonGuncellenmeTarihi" },
                values: new object[] { new DateTime(2026, 7, 29, 20, 37, 14, 602, DateTimeKind.Utc).AddTicks(1329), new DateTime(2026, 7, 29, 20, 37, 14, 602, DateTimeKind.Utc).AddTicks(1323) });

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "UrunId",
                keyValue: new Guid("f6e5d4c3-b2a1-0f9e-8d7c-6b5a4f3db1ba"),
                column: "OlusturulmaTarihi",
                value: new DateTime(2026, 7, 29, 20, 37, 14, 602, DateTimeKind.Utc).AddTicks(1270));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Siparis");

            migrationBuilder.DropTable(
                name: "SiparisDetay");

            migrationBuilder.UpdateData(
                table: "Depolar",
                keyColumn: "DepoId",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                column: "OlusturulmaTarihi",
                value: new DateTime(2026, 7, 21, 19, 8, 32, 604, DateTimeKind.Utc).AddTicks(5872));

            migrationBuilder.UpdateData(
                table: "Stoklar",
                keyColumn: "StokId",
                keyValue: new Guid("11223344-5566-7788-9900-aabbccddeeff"),
                columns: new[] { "OlusturulmaTarihi", "SonGuncellenmeTarihi" },
                values: new object[] { new DateTime(2026, 7, 21, 19, 8, 32, 604, DateTimeKind.Utc).AddTicks(6263), new DateTime(2026, 7, 21, 19, 8, 32, 604, DateTimeKind.Utc).AddTicks(6258) });

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "UrunId",
                keyValue: new Guid("f6e5d4c3-b2a1-0f9e-8d7c-6b5a4f3db1ba"),
                column: "OlusturulmaTarihi",
                value: new DateTime(2026, 7, 21, 19, 8, 32, 604, DateTimeKind.Utc).AddTicks(6219));
        }
    }
}
