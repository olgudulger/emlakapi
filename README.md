# Emlak Takip API

Bu proje .NET 9.0 ile geliştirilmiş bir emlak takip sistemi API'sidir.

## 🚀 Render'da Deploy Etme

### Gereksinimler
- GitHub repository
- Render hesabı
- PostgreSQL/MySQL veritabanı (Render'da oluşturulabilir)

### Deploy Adımları

1. **GitHub Repository'sini Render'a Bağla**
   - Render Dashboard'a git
   - "New" → "Web Service" seç
   - GitHub repository'nizi seçin

2. **Konfigürasyon Ayarları**
   - **Name**: `emlak-api` (veya istediğiniz isim)
   - **Environment**: `Docker`
   - **Build Command**: (boş bırak - Dockerfile kullanılacak)
   - **Start Command**: (boş bırak - Dockerfile'da tanımlı)

3. **Environment Variables**
   Aşağıdaki environment variable'ları ekleyin:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   DATABASE_URL=your_database_connection_string
   JWT_KEY=your_super_secret_jwt_key_at_least_32_chars
   JWT_ISSUER=your-app-name.onrender.com
   JWT_AUDIENCE=your-app-name.onrender.com
   JWT_EXPIRES_IN_MINUTES=60
   ```

4. **Veritabanı Kurulumu**
   - Render'da PostgreSQL veritabanı oluşturun
   - DATABASE_URL'yi kopyalayın ve environment variable olarak ekleyin

### Environment Variables Açıklaması

- `DATABASE_URL`: Veritabanı bağlantı string'i
- `JWT_KEY`: JWT token'ları imzalamak için güvenli anahtar (minimum 32 karakter)
- `JWT_ISSUER`: JWT token'ı yayınlayan servis adı
- `JWT_AUDIENCE`: JWT token'ı alacak servis adı
- `JWT_EXPIRES_IN_MINUTES`: Token'ın geçerlilik süresi (dakika)

## 🐳 Docker

Proje Docker ile containerize edilmiştir:

### Lokal Docker Build
```bash
docker build -t emlak-api .
docker run -p 8080:8080 emlak-api
```

### Docker Compose (Geliştirme için)
```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - DATABASE_URL=your_connection_string
```

## 📋 API Endpoints

API'ye deploy edildikten sonra şu URL'lerden erişebilirsiniz:

- **Swagger UI**: `https://your-app-name.onrender.com/`
- **Health Check**: `https://your-app-name.onrender.com/health`
- **API Base**: `https://your-app-name.onrender.com/api/`

## 🔧 Development

### Yerel Geliştirme
```bash
# Dependencies restore
dotnet restore

# Database update
dotnet ef database update --project DataAccess

# Run the application
dotnet run --project WebAPI
```

### Veritabanı Migrations
```bash
# Yeni migration oluştur
dotnet ef migrations add YourMigrationName --project DataAccess

# Database güncelle
dotnet ef database update --project DataAccess
```

## 📝 Notlar

- Production ortamında Swagger UI aktif edilmiştir
- CORS production için AllowAnyOrigin olarak ayarlanmıştır
- Health check endpoint mevcut
- HTTPS redirect production'da kapalıdır (Render SSL terminasyonu yapar)

## 🔒 Güvenlik

- JWT authentication aktif
- Güçlü şifreler gerekli
- Environment variable'lar ile gizli bilgiler korunur
- CORS policy aktif 