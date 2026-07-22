using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UretimTakip.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCariTable : Migration
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

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Stoklar");

            migrationBuilder.DropColumn(
                name: "Adet",
                table: "Stoklar");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Depolar");

            migrationBuilder.DropColumn(
                name: "Ad",
                table: "Depolar");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Urunler",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stoklar",
                table: "Stoklar",
                column: "StokId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Depolar",
                table: "Depolar",
                column: "DepoId");

            migrationBuilder.CreateTable(
                name: "Cariler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CariKodu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CariTuru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cariler", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Depolar",
                columns: new[] { "DepoId", "Adres", "DepoAdi", "DepoKodu", "IrtibatBilgisi", "IsActive", "IsArchived", "Konum", "OlusturulmaTarihi", "SorumluKisi" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"), "", "Merkez Depo", "", "", true, false, "Eskişehir Tepebaşı", new DateTime(2026, 7, 21, 19, 8, 32, 604, DateTimeKind.Utc).AddTicks(5872), "" });

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "UrunId",
                keyValue: new Guid("f6e5d4c3-b2a1-0f9e-8d7c-6b5a4f3db1ba"),
                columns: new[] { "IsDeleted", "OlusturulmaTarihi" },
                values: new object[] { false, new DateTime(2026, 7, 21, 19, 8, 32, 604, DateTimeKind.Utc).AddTicks(6219) });

            migrationBuilder.InsertData(
                table: "Stoklar",
                columns: new[] { "StokId", "DepoId", "Miktar", "OlusturulmaTarihi", "SonGuncellenmeTarihi", "StokKodu", "UrunId" },
                values: new object[] { new Guid("11223344-5566-7788-9900-aabbccddeeff"), new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"), 50, new DateTime(2026, 7, 21, 19, 8, 32, 604, DateTimeKind.Utc).AddTicks(6263), new DateTime(2026, 7, 21, 19, 8, 32, 604, DateTimeKind.Utc).AddTicks(6258), "", new Guid("f6e5d4c3-b2a1-0f9e-8d7c-6b5a4f3db1ba") });

            migrationBuilder.AddForeignKey(
                name: "FK_Stoklar_Depolar_DepoId",
                table: "Stoklar",
                column: "DepoId",
                principalTable: "Depolar",
                principalColumn: "DepoId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stoklar_Depolar_DepoId",
                table: "Stoklar");

            migrationBuilder.DropTable(
                name: "Cariler");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stoklar",
                table: "Stoklar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Depolar",
                table: "Depolar");

            migrationBuilder.DeleteData(
                table: "Stoklar",
                keyColumn: "StokId",
                keyValue: new Guid("11223344-5566-7788-9900-aabbccddeeff"));

            migrationBuilder.DeleteData(
                table: "Depolar",
                keyColumn: "DepoId",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"));

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Urunler");

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

            migrationBuilder.UpdateData(
                table: "Urunler",
                keyColumn: "UrunId",
                keyValue: new Guid("f6e5d4c3-b2a1-0f9e-8d7c-6b5a4f3db1ba"),
                column: "OlusturulmaTarihi",
                value: new DateTime(2026, 7, 3, 15, 41, 56, 489, DateTimeKind.Utc).AddTicks(2173));

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
    }
}
