using Microsoft.EntityFrameworkCore;
using UretimTakip.core.Entities;

namespace UretimTakip.DataAccess.Context
{
    // Bütün katmanların erişebilmesi için 'public' yapıyoruz
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // C# modellerimizi SQL tablosu olarak tanımlıyoruz
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Depo> Depolar { get; set; }
        public DbSet<Stok> Stoklar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tablo isimlerini SQL tarafında netleştiriyoruz
            modelBuilder.Entity<Urun>().ToTable("Urunler");
            // Fiyat alanının SQL'de decimal(18,2) olarak hassas bir şekilde tutulacağını belirtiyoruz
            modelBuilder.Entity<Urun>().Property(u => u.Fiyat).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Depo>().ToTable("Depolar");
            modelBuilder.Entity<Stok>().ToTable("Stoklar");

            // Stok tablosundaki ilişkileri (Foreign Key) kuruyoruz
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

            // 1. İlişkileri bağlayabilmek için sabit GUID'ler üretiyoruz
            var merkezDepoId = Guid.Parse("A1B2C3D4-E5F6-7A8B-9C0D-1E2F3A4B5C6D");
            var telefonUrunId = Guid.Parse("F6E5D4C3-B2A1-0F9E-8D7C-6B5A4F3DB1BA");
            var stokId = Guid.Parse("11223344-5566-7788-9900-AABBCCDDEEFF");

            // 2. İlk sahte depomuzu ekliyoruz
            // NOT: Depo sınıfının içindeki alan adlarına göre (eğer onda da Id yerine DepoId vs. varsa) düzenleyebilirsin.
            modelBuilder.Entity<Depo>().HasData(new Depo
            {
                Id = merkezDepoId, // Eğer Depo.cs içinde de 'DepoId' ise bunu da 'DepoId = merkezDepoId' yapmalısın balım!
                Ad = "Merkez Depo",
                Konum = "Eskişehir Tepebaşı",
                OlusturulmaTarihi = DateTime.UtcNow
            });

            // 3. İlk sahte ürünümüzü ekliyoruz (Senin alan adlarınla tamamen eşitlendi!)
            // 3. İlk sahte ürünümüzü ekliyoruz (Fiyat alanı dahil edildi!)
            modelBuilder.Entity<Urun>().HasData(new Urun
            {
                UrunId = telefonUrunId,
                UrunAdi = "Akıllı Telefon",
                SistemUrunKodu = "TEL-001",
                Marka = "Apple",
                Model = "iPhone 15",
                Fiyat = 45000, // Yeni eklediğimiz decimal alanımız
                OlusturulmaTarihi = DateTime.UtcNow
            });

            // 4. Bu depo ve ürünü birbirine bağlayan stok verisini ekliyoruz
            modelBuilder.Entity<Stok>().HasData(new Stok
            {
                Id = stokId, // Eğer Stok.cs içinde de 'StokId' ise 'StokId = stokId' yapmalısın!
                DepoId = merkezDepoId,
                UrunId = telefonUrunId,
                Adet = 50,
                OlusturulmaTarihi = DateTime.UtcNow
            });
        }
    }
}