# Üretim Takip Sistemi (UretimTakip)

Bu proje, bir üretim işletmesinin temel iş süreçlerini dijitalleştirmek ve takip etmek amacıyla geliştirilmiş olan bir eğitim ve portfolyo projesidir. Proje kapsamında çok katmanlı mimari (Layered Architecture) ve modern tasarım kalıpları uygulanmıştır.

## 🛠️ Kullanılan Teknolojiler
- **Backend**: .NET 8.0 ASP.NET Core MVC
- **Database**: MS SQL Server & Entity Framework Core (Code-First)
- **Frontend**: Bootstrap, jQuery, AJAX, SweetAlert2, CSS Grid/Flexbox
- **Tasarım & Mimari**: Generic DTO'lar, DB Transaction Yönetimi, Soft Delete (Yumuşak Silme) Mekanizması

## 📁 Katmanlı Mimari Yapısı
- **UretimTakip.core**: Entity (Varlık) tanımlamaları, DTO'lar ve ortak modeller.
- **UretimTakip.DataAccess**: Veritabanı bağlamı (`DbContext`), veritabanı tablolarının konfigürasyonları ve migration işlemleri.
- **UretimTakip.Business**: İş mantığı (Business Logic) servisleri ve kuralları.
- **UretimTakip.Web**: Kullanıcı arayüzü (Presentation) katmanı, Controller yapısı ve View bileşenleri.

## 🚀 Mevcut Özellikler
- **Ürün Yönetimi**: Ürün ekleme, listeleme, güncelleme ve soft-delete desteği.
- **Cari Yönetimi**: Müşteri/Tedarikçi (Cari) bilgilerinin yönetimi.
- **Depo ve Stok Yönetimi**: Depo tanımlamaları ve bu depolardaki ürün stok miktarlarının takibi.
- **Sipariş Yönetimi**: Master-Detail ilişkili sipariş oluşturma (tek seferde sipariş başlığı ve birden fazla ürün kalemi ekleme) ve sipariş iptali.

## ⚙️ Kurulum ve Çalıştırma
1. `UretimTakip.Web/appsettings.json` dosyasındaki `DefaultConnection` bağlantı dizesini kendi MS SQL Server bilgilerinize göre güncelleyin.
2. Paket Yöneticisi Konsolu'nda (Package Manager Console) veya terminalde migration'ları uygulayın:
   ```bash
   dotnet ef database update --project UretimTakip.DataAccess --startup-project UretimTakip.Web
