using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UretimTakip.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stoklar_Depolar_DepoId",
                table: "Stoklar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stoklar",
                table: "Stoklar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Depolar",
                table: "Depolar");

            migrationBuilder.AddColumn<decimal>(
                name: "Fiyat",
                table: "Urunler",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Stoklar",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Adet",
                table: "Stoklar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturulmaTarihi",
                table: "Stoklar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Depolar",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Ad",
                table: "Depolar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Konum",
                table: "Depolar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturulmaTarihi",
                table: "Depolar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stoklar",
                table: "Stoklar",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Depolar",
                table: "Depolar",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Depolar",
                columns: new[] { "Id", "Ad", "Adres", "DepoAdi", "DepoId", "DepoKodu", "IrtibatBilgisi", "IsActive", "IsArchived", "Konum", "OlusturulmaTarihi", "SorumluKisi" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"), "Merkez Depo", "", "", new Guid("a60c5b51-6d53-426c-b6a7-2e5648a154e5"), "", "", true, false, "Eskişehir Tepebaşı", new DateTime(2026, 7, 3, 15, 41, 56, 489, DateTimeKind.Utc).AddTicks(1975), "" });

            migrationBuilder.InsertData(
                table: "Urunler",
                columns: new[] { "UrunId", "Agirlik", "Akim", "Desi", "Fiyat", "Genislik", "Guc", "GuncellenmeTarihi", "HariciKodu", "Kategori", "Marka", "Model", "OlcuBirimi", "OlusturulmaTarihi", "SistemUrunKodu", "UrunAdi", "Uzunluk", "Voltaj", "Yukseklik" },
                values: new object[] { new Guid("f6e5d4c3-b2a1-0f9e-8d7c-6b5a4f3db1ba"), 0.0, "", 0.0, 45000m, 0.0, "", null, "", "", "Apple", "iPhone 15", "", new DateTime(2026, 7, 3, 15, 41, 56, 489, DateTimeKind.Utc).AddTicks(2173), "TEL-001", "Akıllı Telefon", 0.0, "", 0.0 });

            migrationBuilder.InsertData(
                table: "Stoklar",
                columns: new[] { "Id", "Adet", "DepoId", "Miktar", "OlusturulmaTarihi", "SonGuncellenmeTarihi", "StokId", "StokKodu", "UrunId" },
                values: new object[] { new Guid("11223344-5566-7788-9900-aabbccddeeff"), 50, new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"), 0, new DateTime(2026, 7, 3, 15, 41, 56, 489, DateTimeKind.Utc).AddTicks(2198), new DateTime(2026, 7, 3, 15, 41, 56, 489, DateTimeKind.Utc).AddTicks(2195), new Guid("1ff0741f-3591-432c-ac47-6a7fe404f5cb"), "", new Guid("f6e5d4c3-b2a1-0f9e-8d7c-6b5a4f3db1ba") });

            migrationBuilder.AddForeignKey(
                name: "FK_Stoklar_Depolar_DepoId",
                table: "Stoklar",
                column: "DepoId",
                principalTable: "Depolar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stoklar_Depolar_DepoId",
                table: "Stoklar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stoklar",
                table: "Stoklar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Depolar",
                table: "Depolar");

            migrationBuilder.DeleteData(
                table: "Stoklar",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("11223344-5566-7788-9900-aabbccddeeff"));

            migrationBuilder.DeleteData(
                table: "Depolar",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"));

            migrationBuilder.DeleteData(
                table: "Urunler",
                keyColumn: "UrunId",
                keyValue: new Guid("f6e5d4c3-b2a1-0f9e-8d7c-6b5a4f3db1ba"));

            migrationBuilder.DropColumn(
                name: "Fiyat",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Stoklar");

            migrationBuilder.DropColumn(
                name: "Adet",
                table: "Stoklar");

            migrationBuilder.DropColumn(
                name: "OlusturulmaTarihi",
                table: "Stoklar");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Depolar");

            migrationBuilder.DropColumn(
                name: "Ad",
                table: "Depolar");

            migrationBuilder.DropColumn(
                name: "Konum",
                table: "Depolar");

            migrationBuilder.DropColumn(
                name: "OlusturulmaTarihi",
                table: "Depolar");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stoklar",
                table: "Stoklar",
                column: "StokId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Depolar",
                table: "Depolar",
                column: "DepoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stoklar_Depolar_DepoId",
                table: "Stoklar",
                column: "DepoId",
                principalTable: "Depolar",
                principalColumn: "DepoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
