using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UretimTakip.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddGoalsAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bildirimler",
                columns: table => new
                {
                    BildirimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mesaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OkunduMu = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OkunmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirimler", x => x.BildirimId);
                });

            migrationBuilder.CreateTable(
                name: "Hedefler",
                columns: table => new
                {
                    HedefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HedeflenenTutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MevcutTutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TamamlandiMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hedefler", x => x.HedefId);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bildirimler");

            migrationBuilder.DropTable(
                name: "Hedefler");

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
    }
}
