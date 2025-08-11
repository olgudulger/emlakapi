# Production DB HasShareholder Fix Script
# PowerShell/Bash script for production DB fix

echo "🔧 Production DB HasShareholder Fix başlıyor..."

# Set Production Environment
$env:ASPNETCORE_ENVIRONMENT = "Production"

echo "📊 Migration durumunu kontrol ediliyor..."
dotnet ef migrations list --project DataAccess --startup-project WebAPI

echo "🎯 Specific migration uygulanıyor..."
dotnet ef database update 20250811151936_AddHasShareholderPropertyNew --project DataAccess --startup-project WebAPI

echo "✅ İşlem tamamlandı. API'yi test edin:"
echo "GET https://emlakapi.onrender.com/api/Property"
