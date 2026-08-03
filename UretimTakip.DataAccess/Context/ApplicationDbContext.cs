using Microsoft.EntityFrameworkCore;
using UretimTakip.core.Entities;
using UretimTakip.core.Models;

namespace UretimTakip.DataAccess.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Depo> Depolar { get; set; }
        public DbSet<Stok> Stoklar { get; set; }
        public DbSet<Cari> Cariler { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisDetay> SiparislerDetaylar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Urun>().ToTable("Urunler");
            modelBuilder.Entity<Urun>().Property(u => u.Fiyat).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Depo>().ToTable("Depolar");
            modelBuilder.Entity<Stok>().ToTable("Stoklar");
            modelBuilder.Entity<Cari>().ToTable("Cariler");
            modelBuilder.Entity<Siparis>().ToTable("Siparis");
            modelBuilder.Entity<Siparis>().Property(s => s.ToplamTutar).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SiparisDetay>().ToTable("SiparisDetay");
            modelBuilder.Entity<SiparisDetay>().Property(s => s.BirimFiyat).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Stok>()
                .HasOne<Depo>()
                .WithMany()
                .HasForeignKey(s => s.DepoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stok>()
                .HasOne<Urun>()
                .WithMany()
                .HasForeignKey(s => s.UrunId)
                .OnDelete(DeleteBehavior.Restrict);

            var merkezDepoId = Guid.Parse("A1B2C3D4-E5F6-7A8B-9C0D-1E2F3A4B5C6D");
            var telefonUrunId = Guid.Parse("F6E5D4C3-B2A1-0F9E-8D7C-6B5A4F3DB1BA");
            var stokId = Guid.Parse("11223344-5566-7788-9900-AABBCCDDEEFF");

            modelBuilder.Entity<Depo>().HasData(new Depo
            {
                DepoId = merkezDepoId,
                DepoAdi = "Merkez Depo",
                Konum = "Eskişehir Tepebaşı",
                OlusturulmaTarihi = DateTime.UtcNow
            });

            modelBuilder.Entity<Urun>().HasData(new Urun
            {
                UrunId = telefonUrunId,
                UrunAdi = "Akıllı Telefon",
                SistemUrunKodu = "TEL-001",
                Marka = "Apple",
                Model = "iPhone 15",
                Fiyat = 45000,
                OlusturulmaTarihi = DateTime.UtcNow
            });

            modelBuilder.Entity<Stok>().HasData(new Stok
            {
                StokId = stokId,
                DepoId = merkezDepoId,
                UrunId = telefonUrunId,
                Miktar = 50,
                OlusturulmaTarihi = DateTime.UtcNow
            });
        }
    }
}