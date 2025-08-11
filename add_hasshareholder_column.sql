-- HasShareholder kolonu Properties tablosuna güvenli ekleme
-- Production DB için manuel SQL

-- 1. Kolonu kontrol et ve yoksa ekle
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('Properties') 
    AND name = 'HasShareholder'
)
BEGIN
    ALTER TABLE Properties 
    ADD HasShareholder bit NOT NULL DEFAULT(0);
    
    PRINT 'HasShareholder kolonu başarıyla eklendi.';
END
ELSE
BEGIN
    PRINT 'HasShareholder kolonu zaten mevcut.';
END

-- 2. Migration history'ye kayıt ekle (EF Core conflict'ini önlemek için)
IF NOT EXISTS (
    SELECT * FROM __EFMigrationsHistory 
    WHERE MigrationId = '20250811151936_AddHasShareholderPropertyNew'
)
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250811151936_AddHasShareholderPropertyNew', '8.0.0');
    
    PRINT 'Migration history kaydı eklendi.';
END
ELSE
BEGIN
    PRINT 'Migration history kaydı zaten mevcut.';
END

-- 3. Verification - kolonu kontrol et
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Properties' 
AND COLUMN_NAME = 'HasShareholder';

PRINT 'HasShareholder kolonu durumu kontrol edildi.';
