using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Entity.Models;
using Entity.Base;
using Entity.Enums;
using DataAccess.Context;
using DataAccess.Concrete;

namespace DataAccess.Context
{
    public class EmlakDbContext : IdentityDbContext<AppUser>
    {
        public EmlakDbContext(DbContextOptions<EmlakDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<FieldProperty> FieldProperties { get; set; }
        public DbSet<LandProperty> LandProperties { get; set; }
        public DbSet<WorkplaceProperty> WorkplaceProperties { get; set; }
        public DbSet<ApartmentProperty> ApartmentProperties { get; set; }
        public DbSet<SharedParcelProperty> SharedParcelProperties { get; set; }
        public DbSet<PropertyDocument> PropertyDocuments { get; set; }
        public DbSet<PropertyPriceHistory> PropertyPriceHistories { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<CustomerProvincePreference> CustomerProvincePreferences { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Neighborhood> Neighborhoods { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<CustomerDistrictPreference> CustomerDistrictPreferences { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TPH (Table Per Hierarchy) konfigürasyonu
            modelBuilder.Entity<Property>()
                .HasDiscriminator<PropertyType>("PropertyType")
                .HasValue<FieldProperty>(PropertyType.Field)
                .HasValue<LandProperty>(PropertyType.Land)
                .HasValue<WorkplaceProperty>(PropertyType.Commercial)
                .HasValue<ApartmentProperty>(PropertyType.Apartment)
                .HasValue<SharedParcelProperty>(PropertyType.SharedParcel);

            // Customer ile Property ilişkisi
            modelBuilder.Entity<Property>()
                .HasOne(p => p.Customer)
                .WithMany(c => c.Properties)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer ile ProvincePreference ilişkisi
            modelBuilder.Entity<CustomerProvincePreference>()
                .HasOne(cpp => cpp.Customer)
                .WithMany(c => c.ProvincePreferences)
                .HasForeignKey(cpp => cpp.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Province ile District ilişkisi
            modelBuilder.Entity<District>()
                .HasOne(d => d.Province)
                .WithMany(p => p.Districts)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.Cascade);

            // District ile Neighborhood ilişkisi
            modelBuilder.Entity<Neighborhood>()
                .HasOne(n => n.District)
                .WithMany(d => d.Neighborhoods)
                .HasForeignKey(n => n.DistrictId)
                .OnDelete(DeleteBehavior.Cascade);

            // Property ile Neighborhood ilişkisi
            modelBuilder.Entity<Property>()
                .HasOne(p => p.Neighborhood)
                .WithMany(n => n.Properties)
                .HasForeignKey(p => p.NeighborhoodId)
                .OnDelete(DeleteBehavior.Restrict);

            // Property ile Province ilişkisi
            modelBuilder.Entity<Property>()
                .HasOne(p => p.Province)
                .WithMany()
                .HasForeignKey(p => p.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Property ile District ilişkisi
            modelBuilder.Entity<Property>()
                .HasOne(p => p.District)
                .WithMany()
                .HasForeignKey(p => p.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            // CustomerDistrictPreference ilişkileri (Cascade çakışmasını önlemek için NoAction)
            modelBuilder.Entity<CustomerDistrictPreference>()
                .HasOne(cdp => cdp.CustomerProvincePreference)
                .WithMany(cpp => cpp.DistrictPreferences)
                .HasForeignKey(cdp => cdp.CustomerProvincePreferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerDistrictPreference>()
                .HasOne(cdp => cdp.District)
                .WithMany()
                .HasForeignKey(cdp => cdp.DistrictId)
                .OnDelete(DeleteBehavior.NoAction); // Cascade çakışmasını önlemek için NoAction

            // Property ile PropertyDocument ilişkisi
            modelBuilder.Entity<PropertyDocument>()
                .HasOne(pd => pd.Property)
                .WithMany()
                .HasForeignKey(pd => pd.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Property ile PropertyPriceHistory ilişkisi
            modelBuilder.Entity<PropertyPriceHistory>()
                .HasOne(pph => pph.Property)
                .WithMany()
                .HasForeignKey(pph => pph.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserActivity ile AppUser ilişkisi
            modelBuilder.Entity<UserActivity>()
                .HasOne(ua => ua.User)
                .WithMany()
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sale ilişkileri
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Property)
                .WithMany()
                .HasForeignKey(s => s.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.BuyerCustomer)
                .WithMany()
                .HasForeignKey(s => s.BuyerCustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enum'lar için string conversion (opsiyonel)
            modelBuilder.Entity<Property>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Customer>()
                .Property(c => c.InterestType)
                .HasConversion<string>();

            modelBuilder.Entity<Customer>()
                .Property(c => c.CustomerType)
                .HasConversion<string>();

            modelBuilder.Entity<Sale>()
                .Property(s => s.Status)
                .HasConversion<string>();

            // Index'ler - Geliştirilmiş performans indeksleri
            // Property indeksleri
            modelBuilder.Entity<Property>()
                .HasIndex(p => p.ProvinceId);

            modelBuilder.Entity<Property>()
                .HasIndex(p => p.DistrictId);

            modelBuilder.Entity<Property>()
                .HasIndex(p => p.NeighborhoodId);

            modelBuilder.Entity<Property>()
                .HasIndex(p => p.PropertyType);

            // Composite indeksler - sık kullanılan sorgular için
            modelBuilder.Entity<Property>()
                .HasIndex(p => new { p.Status, p.CreatedAt })
                .HasDatabaseName("IX_Property_Status_CreatedAt");

            modelBuilder.Entity<Property>()
                .HasIndex(p => new { p.CustomerId, p.Status })
                .HasDatabaseName("IX_Property_CustomerId_Status");

            modelBuilder.Entity<Property>()
                .HasIndex(p => new { p.ProvinceId, p.DistrictId, p.Status })
                .HasDatabaseName("IX_Property_Location_Status");

            modelBuilder.Entity<Property>()
                .HasIndex(p => new { p.PropertyType, p.Status })
                .HasDatabaseName("IX_Property_Type_Status");

            // Sale indeksleri
            modelBuilder.Entity<Sale>()
                .HasIndex(s => s.PropertyId);

            modelBuilder.Entity<Sale>()
                .HasIndex(s => s.BuyerCustomerId);

            modelBuilder.Entity<Sale>()
                .HasIndex(s => s.UserId);

            modelBuilder.Entity<Sale>()
                .HasIndex(s => new { s.Status, s.SaleDate })
                .HasDatabaseName("IX_Sale_Status_SaleDate");

            modelBuilder.Entity<Sale>()
                .HasIndex(s => new { s.UserId, s.SaleDate })
                .HasDatabaseName("IX_Sale_UserId_SaleDate");

            modelBuilder.Entity<Sale>()
                .HasIndex(s => new { s.Status, s.UserId })
                .HasDatabaseName("IX_Sale_Status_UserId");

            // Customer indeksleri
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Phone);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.InterestType);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.CustomerType);

            // Arama için full-text indeks hazırlığı
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.FullName)
                .HasDatabaseName("IX_Customer_FullName");

            modelBuilder.Entity<Property>()
                .HasIndex(p => p.Title)
                .HasDatabaseName("IX_Property_Title");

            // Location indeksleri
            modelBuilder.Entity<Neighborhood>()
                .HasIndex(n => n.DistrictId);

            modelBuilder.Entity<District>()
                .HasIndex(d => d.ProvinceId);

            // Activity ve Token indeksleri
            modelBuilder.Entity<UserActivity>()
                .HasIndex(ua => new { ua.UserId, ua.ActivityDate })
                .HasDatabaseName("IX_UserActivity_UserId_ActivityDate");

            // RefreshToken indeksleri
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => new { rt.UserId, rt.ExpiresAt })
                .HasDatabaseName("IX_RefreshToken_UserId_ExpiresAt");

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique()
                .HasDatabaseName("IX_RefreshToken_Token_Unique");

            // RefreshToken için calculated properties'i ignore et
            modelBuilder.Entity<RefreshToken>()
                .Ignore(rt => rt.IsActive)
                .Ignore(rt => rt.IsExpired);

            // PropertyPriceHistory indeksleri
            modelBuilder.Entity<PropertyPriceHistory>()
                .HasIndex(pph => new { pph.PropertyId, pph.Date })
                .HasDatabaseName("IX_PropertyPriceHistory_PropertyId_Date");

            // PropertyDocument indeksleri
            modelBuilder.Entity<PropertyDocument>()
                .HasIndex(pd => pd.PropertyId);

            // AppUser performans indeksleri
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.IsOnline)
                .HasDatabaseName("IX_AppUser_IsOnline");

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => new { u.IsOnline, u.LastActivityDate })
                .HasDatabaseName("IX_AppUser_IsOnline_LastActivityDate");

            // Seed Data
            SeedInitialData(modelBuilder);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            // SeedData class'ından gerçek il/ilçe/mahalle verilerini kullan
            SeedData.SeedInitialData(modelBuilder);

            // Admin kullanıcısı (runtime'da SeedAllAsync ile oluşturulacak)
            // Identity kullanıcıları seed data olarak eklenmez, çünkü UserManager gerekli
        }
    }
} 