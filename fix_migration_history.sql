-- Production DB Migration History Düzeltme
-- Bu script production DB'de migration history'yi düzeltir

-- 1. Mevcut migration history'yi kontrol et
SELECT MigrationId, ProductVersion 
FROM __EFMigrationsHistory 
ORDER BY MigrationId;

-- 2. HasShareholder migration'ını history'ye ekle (eğer yoksa)
IF NOT EXISTS (
    SELECT * FROM __EFMigrationsHistory 
    WHERE MigrationId = '20250811151936_AddHasShareholderPropertyNew'
)
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250811151936_AddHasShareholderPropertyNew', '8.0.0');
    
    PRINT '✅ AddHasShareholderPropertyNew migration history eklendi.';
END

-- 3. HasShareholder kolonu var mı kontrol et
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('Properties') 
    AND name = 'HasShareholder'
)
BEGIN
    -- Kolon yoksa ekle
    ALTER TABLE Properties 
    ADD HasShareholder bit NOT NULL DEFAULT(0);
    
    PRINT '✅ HasShareholder kolonu eklendi.';
END
ELSE
BEGIN
    PRINT '✅ HasShareholder kolonu zaten mevcut.';
END

-- 4. Final verification
SELECT 
    COUNT(*) as HasShareholderColumnExists
FROM sys.columns 
WHERE object_id = OBJECT_ID('Properties') 
AND name = 'HasShareholder';

SELECT 
    COUNT(*) as MigrationHistoryExists
FROM __EFMigrationsHistory 
WHERE MigrationId = '20250811151936_AddHasShareholderPropertyNew';

PRINT '🎯 Production DB HasShareholder fix tamamlandı!';
