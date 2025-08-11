# 🔧 Database Configuration Guide

Bu proje environment-based database seçimi destekler. Tek bir flag ile local veya production database arasında geçiş yapabilirsiniz.

## 📋 Konfigürasyon Dosyaları

### 1. `appsettings.json` (Base Configuration)
```json
{
  "DatabaseSettings": {
    "UseLocalDatabase": false,
    "LocalConnection": "Server=localhost;Database=EmlakTakipDB;User Id=sa;Password=Oolgu59600;TrustServerCertificate=True;",
    "ProductionConnection": "Server=localhost:3306;Database=servisdasmekanik_com_emlak;User Id=servisdasmekanik_com_emlak;Password=BFsKuqmna3x6s9!#;TrustServerCertificate=True;"
  }
}
```

### 2. `appsettings.Development.json` (Development Override)
```json
{
  "DatabaseSettings": {
    "UseLocalDatabase": true
  }
}
```

### 3. `appsettings.Production.json` (Production Override)
```json
{
  "DatabaseSettings": {
    "UseLocalDatabase": false,
    "ProductionConnection": "Server=94.73.170.52;Database=u2290880_db31B;..."
  }
}
```

## 🚀 Nasıl Kullanılır?

### Local Development
```bash
# Development environment - otomatik local DB kullanır
dotnet run --environment Development

# Console output: 🔧 LOCAL DATABASE kullanılıyor: Server=localhost;Database=EmlakTakipDB...
```

### Production Deployment
```bash
# Production environment - otomatik production DB kullanır
dotnet run --environment Production

# Console output: 🚀 PRODUCTION DATABASE kullanılıyor: Server=94.73.170.52;Database=u2290880_db31B...
```

### Manuel Override (Test İçin)
```bash
# Base appsettings.json'da UseLocalDatabase değerini değiştirin
"UseLocalDatabase": true   # Local DB için
"UseLocalDatabase": false  # Production DB için
```

## ✅ Avantajlar

1. **Environment-based:** Development ortamında otomatik local DB
2. **Güvenli:** Production secrets sadece production dosyasında
3. **Kolay Test:** Tek flag ile DB değiştirme
4. **Debug Friendly:** Console'da hangi DB kullanıldığını gösterir
5. **Deployment Ready:** Render/Azure otomatik production DB kullanır

## 🔧 Connection String Güncelleme

Local DB connection string'inizi güncellemek için:
- `appsettings.json` → `DatabaseSettings.LocalConnection` 

Production DB connection string'inizi güncellemek için:
- `appsettings.Production.json` → `DatabaseSettings.ProductionConnection`
