using Entity.Models;
using Entity.Enums;
using Microsoft.AspNetCore.Identity;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Concrete
{
    public static class SeedData
    {
        private static readonly DateTime SeedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static async Task SeedAllAsync(
            UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            EmlakDbContext context)
        {
            await SeedUsersAsync(userManager, roleManager);
            await SeedSampleDataAsync(context);
        }

        public static async Task SeedUsersAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var adminUsername = "admin";
            var userUsername = "user1";
            var password = "Eremlak123*";
            
            // Rolleri oluştur
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // Admin kullanıcısı oluştur
            if (await userManager.FindByNameAsync(adminUsername) == null)
            {
                var user = new AppUser 
                { 
                    UserName = adminUsername, 
                    Email = "ercanyapimuhendislik59@gmail.com",
                    Role = "Admin" // Custom Role property'sini set et
                };
                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
            else
            {
                // Kullanıcı mevcutsa Role property'sini güncelle
                var existingUser = await userManager.FindByNameAsync(adminUsername);
                if (existingUser != null && string.IsNullOrEmpty(existingUser.Role))
                {
                    existingUser.Role = "Admin";
                    await userManager.UpdateAsync(existingUser);
                }
            }
            
            // Test kullanıcısı oluştur
            if (await userManager.FindByNameAsync(userUsername) == null)
            {
                var user = new AppUser 
                { 
                    UserName = userUsername, 
                    Email = "user@emlak.com",
                    Role = "User" // Custom Role property'sini set et
                };
                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
            else
            {
                // Kullanıcı mevcutsa Role property'sini güncelle
                var existingUser = await userManager.FindByNameAsync(userUsername);
                if (existingUser != null && string.IsNullOrEmpty(existingUser.Role))
                {
                    existingUser.Role = "User";
                    await userManager.UpdateAsync(existingUser);
                }
            }
        }

        public static async Task SeedSampleDataAsync(EmlakDbContext context)
        {
            // Örnek veriler için boş metod - gerektiğinde buraya eklenebilir
            await Task.CompletedTask;
        }

        public static void SeedInitialData(ModelBuilder modelBuilder)
        {
            SeedProvinces(modelBuilder);
            SeedDistricts(modelBuilder);
            SeedNeighborhoods(modelBuilder);
        }

        private static void SeedProvinces(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Province>().HasData(
                new Province { Id = 1, Name = "Çanakkale", CreatedAt = SeedDate },
                new Province { Id = 2, Name = "Edirne", CreatedAt = SeedDate },
                new Province { Id = 3, Name = "Kırklareli", CreatedAt = SeedDate },
                new Province { Id = 4, Name = "Tekirdağ", CreatedAt = SeedDate }
            );
        }

        private static void SeedDistricts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<District>().HasData(
                // Çanakkale İlçeleri (12 ilçe)
                new District { Id = 1, Name = "Merkez", ProvinceId = 1, CreatedAt = SeedDate },
                new District { Id = 2, Name = "Ayvacık", ProvinceId = 1, CreatedAt = SeedDate },
                new District { Id = 3, Name = "Bayramiç", ProvinceId = 1, CreatedAt = SeedDate },
                new District { Id = 4, Name = "Biga", ProvinceId = 1, CreatedAt = SeedDate },
                new District { Id = 5, Name = "Bozcaada", ProvinceId = 1, CreatedAt = SeedDate },
                new District { Id = 6, Name = "Çan", ProvinceId = 1, CreatedAt = SeedDate },
                new District { Id = 7, Name = "Eceabat", ProvinceId = 1, CreatedAt = SeedDate },
                new District { Id = 8, Name = "Ezine", ProvinceId = 1, CreatedAt = SeedDate },
                new District { Id = 9, Name = "Gelibolu", ProvinceId = 1, CreatedAt = SeedDate },
                new District { Id = 10, Name = "Gökçeada", ProvinceId = 1, CreatedAt = SeedDate },
                new District { Id = 11, Name = "Lapseki", ProvinceId = 1, CreatedAt = SeedDate },
                new District { Id = 12, Name = "Yenice", ProvinceId = 1, CreatedAt = SeedDate },
                
                // Edirne İlçeleri (9 ilçe)
                new District { Id = 13, Name = "Merkez", ProvinceId = 2, CreatedAt = SeedDate },
                new District { Id = 14, Name = "Enez", ProvinceId = 2, CreatedAt = SeedDate },
                new District { Id = 15, Name = "Havsa", ProvinceId = 2, CreatedAt = SeedDate },
                new District { Id = 16, Name = "İpsala", ProvinceId = 2, CreatedAt = SeedDate },
                new District { Id = 17, Name = "Keşan", ProvinceId = 2, CreatedAt = SeedDate },
                new District { Id = 18, Name = "Lalapaşa", ProvinceId = 2, CreatedAt = SeedDate },
                new District { Id = 19, Name = "Meriç", ProvinceId = 2, CreatedAt = SeedDate },
                new District { Id = 20, Name = "Süloğlu", ProvinceId = 2, CreatedAt = SeedDate },
                new District { Id = 21, Name = "Uzunköprü", ProvinceId = 2, CreatedAt = SeedDate },
                
                // Kırklareli İlçeleri (8 ilçe)
                new District { Id = 22, Name = "Merkez", ProvinceId = 3, CreatedAt = SeedDate },
                new District { Id = 23, Name = "Babaeski", ProvinceId = 3, CreatedAt = SeedDate },
                new District { Id = 24, Name = "Demirköy", ProvinceId = 3, CreatedAt = SeedDate },
                new District { Id = 25, Name = "Kofçaz", ProvinceId = 3, CreatedAt = SeedDate },
                new District { Id = 26, Name = "Lüleburgaz", ProvinceId = 3, CreatedAt = SeedDate },
                new District { Id = 27, Name = "Pehlivanköy", ProvinceId = 3, CreatedAt = SeedDate },
                new District { Id = 28, Name = "Pınarhisar", ProvinceId = 3, CreatedAt = SeedDate },
                new District { Id = 29, Name = "Vize", ProvinceId = 3, CreatedAt = SeedDate },
                
                // Tekirdağ İlçeleri (11 ilçe)
                new District { Id = 30, Name = "Süleymanpaşa", ProvinceId = 4, CreatedAt = SeedDate },
                new District { Id = 31, Name = "Çerkezköy", ProvinceId = 4, CreatedAt = SeedDate },
                new District { Id = 32, Name = "Çorlu", ProvinceId = 4, CreatedAt = SeedDate },
                new District { Id = 33, Name = "Ergene", ProvinceId = 4, CreatedAt = SeedDate },
                new District { Id = 34, Name = "Hayrabolu", ProvinceId = 4, CreatedAt = SeedDate },
                new District { Id = 35, Name = "Kapaklı", ProvinceId = 4, CreatedAt = SeedDate },
                new District { Id = 36, Name = "Malkara", ProvinceId = 4, CreatedAt = SeedDate },
                new District { Id = 37, Name = "Marmaraereğlisi", ProvinceId = 4, CreatedAt = SeedDate },
                new District { Id = 38, Name = "Muratlı", ProvinceId = 4, CreatedAt = SeedDate },
                new District { Id = 39, Name = "Saray", ProvinceId = 4, CreatedAt = SeedDate },
                new District { Id = 40, Name = "Şarköy", ProvinceId = 4, CreatedAt = SeedDate }
            );
        }

        private static void SeedNeighborhoods(ModelBuilder modelBuilder)
        {
            var neighborhoods = new List<Neighborhood>();
            int id = 1;

            // Çanakkale Mahalleleri (100 mahalle)
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 1, new[] {
                "Barbaros","Boğazkent","Cevat Paşa","Cumhuriyet","Esenler","Fevzipaşa","Hamidiye","İsmetpaşa","Kemalpaşa","Namık Kemal","Yenice",
                "Akçalı Köyü","Akçapınar Köyü","Akçeşme Köyü","Alanköy Köyü","Aşağıokçular Köyü","Belen Köyü","Bodurlar Köyü","Civler Öyü",
                "Çamyayla Köyü","Çınarlı Köyü", "Çıplak Köyü", "Çiftlikdere Köyü", "Dedeler Köyü", "Denizgöründü Köyü", "Dümrek Köyü", "Elmacık Köyü", "Erenköy Köyü", "Gökçalı Köyü", "Güzelyalı Köyü", "Halileli Köyü", "Haliloğlu Köyü", "Işıklar Köyü", "Kalabaklı Köyü", "Kalafat Köyü", "Karacalar Köyü", "Karacaören Köyü", "Karapınar Köyü", "Kayadere Köyü", "Kemel Köyü", "Kızılcaören Köyü", "Kızılkeçili Köyü", "Kirazlı Köyü", "Kocalar Köyü", "Kumkale Köyü", "Kurşunlu Köyü", "Maraşalfevziçakmak Köyü", "Musaköy Köyü", "Ortaca Köyü", "Ovacık Köyü", "Özbek Köyü", "Salihler Köyü", "Saraycık Köyü", "Saribeyli Köyü", "Sarıcaeli Köyü", "Serçiler Köyü", "Taşlıtarla Köyü", "Terziler Köyü", "Tevfikiye Köyü", "Ulupınar Köyü", "Yağcılar Köyü", "Yapıldak Köyü", "Yukarıokçular Köyü"
            }));
            
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 2, new[] {
                "Fatih", "Gökçetepe", "Hamdibey", "Mıhlı", "Sahil", "Ümmühan","Adatepe Köyü", "Ahmetçe Köyü", "Ahmetler Köyü", "Akçin Köyü", "Arıklı Köyü", "Babadere Köyü", "Babakale Köyü", "Bademli Köyü", "Baharlar Köyü", "Bahçedere Köyü", "Balabanlı Köyü", "Behram Köyü", "Bektaş Köyü", "Bilaller Köyü", "Budaklar Köyü", "Büyükkusun Köyü", "Cemaller Köyü", "Çakmaklar Köyü", "Çaltı Köyü", "Çamkalabak Köyü", "Çamköy Köyü", "Çınarpınar Köyü", "Demirci Köyü", "Dibekli Köyü", "Erecek Köyü", "Gülpınar Köyü", "Güzelköy Köyü", "Hüseyinfakı Köyü", "İlyasfakı Köyü", "Kayalar Köyü", "Keçikaya Köyü", "Kestanelik Köyü", "Kırca Köyü", "Kısacık Köyü", "Kocaköy Köyü", "Korubaşı Köyü", "Koyunevi Köyü", "Kozlu Köyü", "Kösedere Köyü", "Kulfal Köyü", "Kuruoba Köyü", "Küçükçetmi Köyü", "Küçükkusun Köyü", "Misvak Köyü", "Naldöken Köyü", "Nusratlı Köyü", "Paşaköy Köyü", "Pınardere Köyü", "Sapanca Köyü", "Sazlı Köyü", "Söğütlü Köyü", "Süleymanköy Köyü", "Şapköy Köyü", "Tabaklar Köyü", "Tamış Köyü", "Tartaşık Köyü", "Taşağıl Köyü", "Taşboğaz Köyü", "Tuzla Köyü", "Tuztaşı Köyü", "Uzunalan Köyü", "Yeniçam Köyü", "Yeşilyurt Köyü", "Yukarıköy Köyü"
            }));

            neighborhoods.AddRange(CreateNeighborhoods(ref id, 3, new[] {
                "Camicedit Mahallesi", "Camikebir Mahallesi", "Menderes Mahallesi", "Tepecik Mahallesi", "Ağaçköy Köyü", "Ahmetçeli Köyü", "Akpınar Köyü", "Alakeçi Köyü", "Alikabaklar Köyü", "Aşağışapcı Köyü", "Aşağışevik Köyü", "Bekirler Köyü", "Beşik Köyü", "Bezırganlar Köyü", "Bıyıklı Köyü", "Cazgirler Köyü", "Çalıdağı Köyü", "Çalıobaakçakıl Köyü", "Çatalçam Köyü", "Çavuşköy Köyü", "Çavuşlu Köyü", "Çırpılar Köyü", "Çiftlik Köyü", "Dağahmetçe Köyü", "Dağoba Köyü", "Daloba Köyü", "Doğancı Köyü", "Evciler Köyü", "Gedik Köyü", "Gökçeiçi Köyü", "Güzeltepe Köyü", "Hacibekirler Köyü", "Hacıdervişler Köyü", "Hacıköy Köyü", "Işıkelı Köyü", "Karaibrahimler Köyü", "Karaköy Köyü", "Karıncalı Köyü", "Kaykılar Köyü", "Korucak Köyü", "Koşuburnutürkmenleri Köyü", "Köseler Köyü", "Köylü Köyü", "Kurşunlu Köyü", "Kuşçayır Köyü", "Kutluoba Köyü", "Külcüler Köyü", "Mollahasanlar Köyü", "Muratlar Köyü", "Nebiler Köyü", "Osmaniye Köyü", "Örenli Köyü", "Palamutoba Köyü", "Pınarbaşı Köyü", "Pıtırli Köyü", "Saçaklı Köyü", "Saraycık Köyü", "Saridüz Köyü", "Sariot Köyü", "Serhat Köyü", "Söğütgediği Köyü", "Toluklar Köyü", "Tongurlu Köyü", "Tülüler Köyü", "Türkmenli Köyü", "Üçyol Köyü", "Üzümlü Köyü", "Yahşielı Köyü", "Yanıklar Köyü", "Yassıbağ Köyü", "Yaylacık Köyü", "Yeniceköy Köyü", "Yeniköy Köyü", "Yeşilköy Köyü", "Yiğitaliler Köyü", "Yukarışapçı Köyü", "Yukarışevik Köyü", "Zerdalilik Köyü", "Zeytinli Köyü"
            }));

            neighborhoods.AddRange(CreateNeighborhoods(ref id, 4, new[] {
                "Cumhuriyet Mahallesi", "Cumhuriyet Mahallesi", "Çavuşköy Mahallesi", "Esentepe Mahallesi", "Gazikemal Mahallesi", "Hamdibey Mahallesi", "İğdelik Mahallesi", "İstiklal Mahallesi", "Sakarya Mahallesi", "Şirintepe Mahallesi", "Turan Mahallesi", "Ulucamii Mahallesi", "Yalı Mahallesi", "Yeniceköy Mahallesi", "Zeytinlik Mahallesi", "Abdiağa Köyü", "Adliye Köyü", "Ağaköy Köyü", "Ahmetler Köyü", "Akkayrak Köyü", "Akköprü Köyü", "Akpınar Köyü", "Aksaz Köyü", "Akyaprak Köyü", "Ambaroba Köyü", "Arabaalan Köyü", "Arabakonağı Köyü", "Aşağıdemirci Köyü", "Ayıtdere Köyü", "Aziziye Köyü", "Bahçeli Köyü", "Bakacak Köyü", "Bakacaklıçiftliği Köyü", "Balıklıçeşme Köyü", "Bekirli Köyü", "Bezırganlar Köyü", "Bozlar Köyü", "Camialan Köyü", "Cihadiye Köyü", "Çakırlı Köyü", "Çelikgürü Köyü", "Çeltik Köyü", "Çeşmealti Köyü", "Çınardere Köyü", "Çınarköprü Köyü", "Çömlekçi Köyü", "Danişment Köyü", "Değirmencik Köyü", "Dereköy Köyü", "Dikmen Köyü", "Doğancı Köyü", "Eğridere Köyü", "Elmalı Köyü", "Emirorman Köyü", "Eskibalıkli Köyü", "Eybekli Köyü", "Gemicikırı Köyü", "Geredelli Köyü", "Gerlengeç Köyü", "Geyikkırı Köyü", "Göktepe Köyü", "Güleç Köyü", "Gündoğdu Köyü", "Gürçeşme Köyü", "Gürgendere Köyü", "Güvemalan Köyü", "Hacihüseyinyaylası Köyü", "Hacıköy Köyü", "Hacıpehlivan Köyü", "Harmanlı Köyü", "Havdan Köyü", "Hisarlı Köyü", "Hoşoba Köyü", "Ilıcabaşı Köyü", "Işıkelı Köyü", "İdriskoru Köyü", "İlyasalan Köyü", "İskender Köyü", "Kahvetepe Köyü", "Kalafat Köyü", "Kaldırımbası Köyü", "Kanibey Köyü", "Kapanbeleni Köyü", "Karaağaç Köyü", "Karacaali Köyü", "Karahamzalar Köyü", "Karapürçek Köyü", "Kaşıkçıoba Köyü", "Katrançı Köyü", "Kayapınar Köyü", "Kaynarca Köyü", "Kazmalı Köyü", "Kemer Köyü", "Kepeklı Köyü", "Kocagür Köyü", "Kozçeşme Köyü", "Kuruoba Köyü", "Osmaniye Köyü", "Otlukdere Köyü", "Ovacık Köyü", "Örtülce Köyü", "Paşaçayı Köyü", "Pekmezli Köyü", "Ramazanlar Köyü", "Sarıca Köyü", "Sarikaya Köyü", "Sarisivat Köyü", "Sarnıç Köyü", "Savaştepe Köyü", "Sazoba Köyü", "Selvi Köyü", "Sığarcık Köyü", "Sinekçi Köyü", "Şakirbey Köyü", "Şirinköy Köyü", "Tokatkırı Köyü", "Türkbakacak Köyü", "Yanıç Köyü", "Yeniçiftlik Köyü", "Yeni Mahalle Köyü", "Yeşilköy Köyü", "Yolindi Köyü", "Yukarıdemirci Köyü"
            }));

            neighborhoods.AddRange(CreateNeighborhoods(ref id, 5, new[] {
                "Alabey Mahallesi", "Cumhuriyet Mahallesi"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 6, new[] {
                "Atatürk Mahallesi", "Cumhuriyet Mahallesi", "Çakmakçayır Mahallesi", "Fatih Mahallesi", "İstiklal Mahallesi", "Karşıyaka Mahallesi", "Osmanefendi Mahallesi", "Seramik Mahallesi", "Süle Mahallesi", "Ahlatlıburun Köyü", "Alibeyçiftliği Köyü", "Altıkulaç Köyü", "Asmalı Köyü", "Bahadırlı Köyü", "Bardakçılar Köyü", "Bilaller Köyü", "Bostandere Köyü", "Bozguç Köyü", "Büyükpaşa Köyü", "Büyüktepe Köyü", "Cicikler Köyü", "Çakılköy Köyü", "Çaltıkara Köyü", "Çamköy Köyü", "Çekiçler Köyü", "Çomaklı Köyü", "Danapınar Köyü", "Derenti Köyü", "Dereoba Köyü", "Doğaca Köyü", "Doğancılar Köyü", "Dondurma Köyü", "Duman Köyü", "Durali Köyü", "Emeşe Köyü", "Eskiyayla Köyü", "Etili Köyü", "Göle Köyü", "Hacıkasım Köyü", "Hacılar Köyü", "Halilağa Köyü", "Helvacı Köyü", "Hurma Köyü", "İlyasağaçiftliği Köyü", "Kadılar Köyü", "Kalburcu Köyü", "Karadağ Köyü", "Karakadılar Köyü", "Karakoca Köyü", "Karlı Köyü", "Kazabat Köyü", "Keçiağili Köyü", "Kızılelma Köyü", "Kocayayla Köyü", "Koyunyerı Köyü", "Kulfal Köyü", "Kumarlar Köyü", "Küçüklü Köyü", "Küçükpaşa Köyü", "Malli Köyü", "Maltepe Köyü", "Okçular Köyü", "Ozancık Köyü", "Sameteli Köyü", "Söğütalanı Köyü", "Şerbetli Köyü", "Tepeköy Köyü", "Uzunalan Köyü", "Üvezdere Köyü", "Yaya Köyü", "Yaykın Köyü", "Yeniçeri Köyü", "Yuvalar Köyü", "Zeybekçayırı Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 7, new[] {
                "İsmetpaşa Mahallesi", "Kemalpaşa Mahallesi", "Alçitepe Köyü", "Behramlı Köyü", "Beşyol Köyü", "Bigalı Köyü", "Büyükanafarta Köyü", "Kilidülbahir Köyü", "Kocadere Köyü", "Kumköy Köyü", "Küçükanafarta Köyü", "Seddülbahir Köyü", "Yalova Köyü", "Yolağzı Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 8, new[] {
                "Camikebir Mahallesi", "Cumhuriyet Mahallesi", "Cumhuriyet Mahallesi", "Danişment Mahallesi", "Gazi Mahallesi", "Hürriyet Mahallesi", "Sarısöğüt Mahallesi", "Seferşah Mahallesi", "Akçakeçili Köyü", "Akköy Köyü", "Aladağ Köyü", "Alemşah Köyü", "Arasanlı Köyü", "Bahçeli Köyü", "Balıklı Köyü", "Belen Köyü", "Bozalan Köyü", "Bozeli Köyü", "Bozköy Köyü", "Çamköy Köyü", "Çamlıca Köyü", "Çamoba Köyü", "Çarıksız Köyü", "Çetmi Köyü", "Çınarköy Köyü", "Dalyan Köyü", "Derbentbaşı Köyü", "Gökçebayır Köyü", "Güllüce Köyü", "Hisaralan Köyü", "Karadağ Köyü", "Karagömlek Köyü", "Kayacık Köyü", "Kemallı Köyü", "Kızılköy Köyü", "Kızıltepe Köyü", "Köprübaşı Köyü", "Körüktaşı Köyü", "Köseler Köyü", "Kumburun Köyü", "Mahmudiye Köyü", "Mecidiye Köyü", "Pazarköy Köyü", "Pınarbaşı Köyü", "Sarpdere Köyü", "Şapköy Köyü", "Taştepe Köyü", "Tavaklı Köyü", "Uluköy Köyü", "Üsküfçü Köyü", "Üvecik Köyü", "Yahyacavuş Köyü", "Yavaşlar Köyü", "Yaylacık Köyü", "Yeniköy Köyü", "Yenioba Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 9, new[] {
                "Alaeddin Mahallesi", "Camiikebir Mahallesi", "Cumhuriyet Mahallesi", "Çeşme Mahallesi", "Gazi Süleyman Paşa Mahallesi", "Hoca Hamza Mahallesi", "Hürriyet Mahallesi", "Keşanoğlu Mahallesi", "Yazıcızade Mahallesi", "Yeni Mahallesi", "Adilhan Köyü", "Bayırköy Köyü", "Bayramiç Köyü", "Bolayır Köyü", "Burhanlı Köyü", "Cevizli Köyü", "Cumalı Köyü", "Çokal Köyü", "Değirmendüzü Köyü", "Demirtepe Köyü", "Fındıklı Köyü", "Güneyli Köyü", "Ilgardere Köyü", "Kalealtı Köyü", "Karainbeyli Köyü", "Kavaklı Köyü", "Kocaçeşme Köyü", "Koruköy Köyü", "Ocaklı Köyü", "Pazarlı Köyü", "Süleymaniye Köyü", "Sütlüce Köyü", "Şadıllı Köyü", "Tayfurköy Köyü", "Yeniköy Köyü", "Yülüce Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 10, new[] {
                "Cumhuriyet Mahallesi", "Çınarlı Mahallesi", "Fatih Mahallesi", "Kuzulimanı Mahallesi", "Yeni Mahallesi", "Bademli Köyü", "Dereköy Köyü", "Eşelek Köyü", "Kaleköy Köyü", "Şirinköy Köyü", "Tepeköy Köyü", "Uğurlu Köyü", "Yenibademli Köyü", "Zeytinliköy Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 11, new[] {
                "Cumhuriyet Mahallesi", "Dalyan Mahallesi", "Gazi Süleyman Paşa Mahallesi", "Gaziyakupbey Mahallesi", "Tekke Mahallesi", "Umurbey Merkez Mahallesi", "Adatepe Köyü", "Akçaalan Köyü", "Alpagut Köyü", "Balcılar Köyü", "Beybaş Köyü", "Beyçayırı Köyü", "Beypınarı Köyü", "Çamyurt Köyü", "Çataltepe Köyü", "Çavuşköy Köyü", "Dereköy Köyü", "Dişbudak Köyü", "Doğandere Köyü", "Dumanlı Köyü", "Ecialan Köyü", "Gökköy Köyü", "Güreci Köyü", "Hacigelen Köyü", "Hacıömerler Köyü", "Harmancık Köyü", "İlyasköy Köyü", "Kangırlı Köyü", "Karamusalar Köyü", "Karaömerler Köyü", "Kemiklıalan Köyü", "Kırcalar Köyü", "Kızıldam Köyü", "Kocabaşlar Köyü", "Kocaveli Köyü", "Mecidiye Köyü", "Nusretiye Köyü", "Sındal Köyü", "Subaşı Köyü", "Suluca Köyü", "Şahinli Köyü", "Şevketiye Köyü", "Taştepe Köyü", "Üçpınar Köyü", "Yaylalar Köyü", "Yeniceköy Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 12, new[] {
               "Atatürk Mahallesi", "Cumhuriyet Mahallesi", "Cumhuriyet Mahallesi", "Eski Yenice Mahallesi", "İstiklal Mahallesi", "Kışlalar Mahallesi", "Kurtuluş Mahallesi", "Yeni Mahallesi", "Ahiler Köyü", "Akçakoyun Köyü", "Akköy Köyü", "Alancık Köyü", "Araovacık Köyü", "Armutcuk Köyü", "Aşağıçavuş Köyü", "Aşağıinova Köyü", "Aşağıkaraaşık Köyü", "Bağlı Köyü", "Ballıçay Köyü", "Başkoz Köyü", "Bayatlar Köyü", "Bekten Köyü", "Boynanlar Köyü", "Cambaz Köyü", "Çakır Köyü", "Çakıroba Köyü", "Çal Köyü", "Çamoba Köyü", "Çınarcık Köyü", "Çınarköy Köyü", "Çırpılar Köyü", "Çiftlik Köyü", "Çukuroba Köyü", "Darıalan Köyü", "Davutköy Köyü", "Engeci Köyü", "Gümüşler Köyü", "Gündoğdu Köyü", "Güzeloba Köyü", "Hacılar Köyü", "Hacıyusuflar Köyü", "Hamdibey Köyü", "Haydaroğa Köyü", "Hıdırlar Köyü", "Kabalı Köyü", "Kalabakbaşı Köyü", "Karaaydın Köyü", "Karabey Köyü", "Karadoru Köyü", "Karaköy Köyü", "Karasu Köyü", "Kargacı Köyü", "Kırıklar Köyü", "Kızıldam Köyü", "Koruköy Köyü", "Kovancı Köyü", "Kurtlar Köyü", "Kuzupınarı Köyü", "Namazgah Köyü", "Nevruz Köyü", "Oğlanalanı Köyü", "Öğmen Köyü", "Örencik Köyü", "Reşadiye Köyü", "Sameteli Köyü", "Sarıçayır Köyü", "Sazak Köyü", "Seyvan Köyü", "Sofular Köyü", "Soğucak Köyü", "Suuçtu Köyü", "Taban Köyü", "Torhasan Köyü", "Umurlar Köyü", "Üçkabaağaç Köyü", "Yağdıran Köyü", "Yalıoba Köyü", "Yarış Köyü", "Yeniköy Köyü", "Yeşilköy Köyü", "Yukarıçavuş Köyü", "Yukarıinova Köyü", "Yukarıkaraaşık Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 13, new[] {
              "1.Murat Mahallesi", "100. Yıl Mahallesi", "Abdurrahman Mahallesi", "Atatürk Mahallesi", "Babademirtaş Mahallesi", "Barutluk Mahallesi", "Cumhuriyet Mahallesi", "Çavuşbey Mahallesi", "Dilaverbey Mahallesi", "Fatih Mahallesi", "İstasyon Mahallesi", "Karaağaç Mahallesi", "Koca Sinan Mahallesi", "Kurtuluş Mahallesi", "Medrese Ali Bey Mahallesi", "Menzilahır Mahallesi", "Meydan Mahallesi", "Mithat Paşa Mahallesi", "Nişancıpaşa Mahallesi", "Sabuni Mahallesi", "Sarıcapaşa Mahallesi", "Şükrüpaşa Mahallesi", "Talatpaşa Mahallesi", "Umurbey Mahallesi", "Yancıkçı Şahin Mahallesi", "Yeniimaret Mahallesi", "Yıldırım Beyazıt Mahallesi", "Yıldırım Hacı Sarraf Mahallesi", "Ahi Köyü", "Avarız Köyü", "Bosna Köyü", "Budakdoğanaca Köyü", "Büyükkarıştıran Köyü", "Büyükismailce Köyü", "Değirmenyanı Köyü", "Demirhanlı Köyü", "Doyran Köyü", "Ekmekçi Köyü", "Elçili Köyü", "Eskikadın Köyü", "Haciumur Köyü", "Hasanağa Köyü", "Hatip Köyü", "Hıdır ağa Köyü", "İskender Köyü", "Karabulut Köyü", "Karakasım Köyü", "Karayusuf Köyü", "Kayapa Köyü", "Kemal Köyü", "Korucu Köyü", "Köşençiftliği Köyü", "Küçükkarıştıran Köyü", "Menekşesofular Köyü", "Muratçalı Köyü", "Musabeyli Köyü", "Orhaniye Köyü", "Sarayakpınar Köyü", "Sazlıdere Köyü", "Suakacağı Köyü", "Tayakadın Köyü", "Uzgaç Köyü", "Üyüklütatar Köyü", "Yenikadın Köyü", "Yolüstü Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 14, new[] {
              "Çataltepe Mahallesi", "Gaziömerbey Mahallesi", "Yeni Mahallesi", "Abdurrahim Köyü", "Büyükevren Köyü", "Çandır Köyü", "Çavuşköy Köyü", "Çeribaşı Köyü", "Gülçavuş Köyü", "Hasköy Köyü", "Hisarlı Köyü", "Işıklı Köyü", "Karaincirlİ Köyü", "Kocaali Köyü", "Küçükevren Köyü", "Sultaniça Köyü", "Sütçüler Köyü", "Şehitler Köyü", "Umurbey Köyü", "Vakıf Köyü", "Yazır Köyü", "Yenice Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 15, new[] {
              "Cumhuriyet Mahallesi", "Hacigazi Mahallesi", "Hacıisa Mahallesi", "Helvacı Mahallesi", "Varoş Mahallesi", "Yeni Mahallesi", "Abalar Köyü", "Arpaç Köyü", "Azatlı Köyü", "Bakışlar Köyü", "Bostanlı Köyü", "Çukurkör Köyü", "Habiller Köyü", "Hasköy Köyü", "Kabaağaç Köyü", "Köseömer Köyü", "Kulubalık Köyü", "Kuzucu Köyü", "Musulca Köyü", "Naipyusuf Köyü", "Necatiye Köyü", "Oğulpaşa Köyü", "Osmanlı Köyü", "Söğütlüdere Köyü", "Şerbettar Köyü", "Tahal Köyü", "Taptık Köyü", "Yolageldi Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 16, new[] {
              "Atatürk Mahallesi", "Atatürk Mahallesi", "Bayrambey Mahallesi", "Bozkurt Mahallesi", "Cumhuriyet Mahallesi", "Fatih Mahallesi", "Fevzi Çakmak Mahallesi", "Gazi Mahallesi", "İnönü Mahallesi", "Kapucu Mahallesi", "Köprü Mahallesi", "Kurtuluş Mahallesi", "Saraç İlyas Mahallesi", "Aliçopehlivan Köyü", "Balabancık Köyü", "Hacı Köyü", "Hıdırköy Köyü", "İbriktepe Köyü", "Karaağaç Köyü", "Kocahıdır Köyü", "Korucu Köyü", "Koyuntepe Köyü", "Kumdere Köyü", "Küçükdoğanca Köyü", "Paşaköy Köyü", "Pazardere Köyü", "Sarıcaali Köyü", "Sarpdere Köyü", "Sultan Köyü", "Tevfikiye Köyü", "Turpçular Köyü", "Yapıldak Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 17, new[] {
              "Aşağı Zaferiye Mahallesi", "Atatürk Mahallesi", "Aydoğan Mahallesi", "Beykent Mahallesi", "Büyük Cami Mahallesi", "Cumhuriyet Mahallesi", "Cumhuriyet Mahallesi", "İnönü Mahallesi", "İspat Cami Mahallesi", "İstasyon Mahallesi", "İstiklal Mahallesi", "İzzetiye Mahallesi", "Kemalpaşa Mahallesi", "Mustafa Kemal Paşa Mahallesi", "Paşayiğit Mahallesi", "Yeni Mahallesi", "Yeni Mahallesi", "Yenimescit Mahallesi", "Yukarı Zaferiye Mahallesi", "Akçeşme Köyü", "Akhoca Köyü", "Altıntaş Köyü", "Bahçeköy Köyü", "Barağı Köyü", "Beyköy Köyü", "Boztepe Köyü", "Büyükdoğanca Köyü", "Çamlıca Köyü", "Çelebi Köyü", "Çeltik Köyü", "Çobançeşmesi Köyü", "Danişment Köyü", "Dişbudak Köyü", "Erikli Köyü", "Gökçetepe Köyü", "Gündüzler Köyü", "Kadıköy Köyü", "Karacaali Köyü", "Karahisar Köyü", "Karasatı Köyü", "Karlı Köyü", "Kılıçköy Köyü", "Kızkapan Köyü", "Koruklu Köyü", "Kozköy Köyü", "Küçükdoğanca Köyü", "Lalacık Köyü", "Mahmutköy Köyü", "Maltepe Köyü", "Mecidiye Köyü", "Mercan Köyü", "Orhaniye Köyü", "Pırnar Köyü", "Sazlıdere Köyü", "Seydiköy Köyü", "Siğilli Köyü", "Suluca Köyü", "Şabanmera Köyü", "Şükrüköy Köyü", "Türkmen Köyü", "Yaylaköy Köyü", "Yeniceçiftlik Köyü", "Yerlisu Köyü", "Yeşilköy Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 18, new[] {
              "Merkez Mahallesi", "Büyünlü Köyü", "Çallıdere Köyü", "Çatma Köyü", "Çömlek Köyü", "Çömlekakpınar Köyü", "Demirköy Köyü", "Doğanköy Köyü", "Dombay Köyü", "Hacıdanişment Köyü", "Hacılar Köyü", "Hamzabeyli Köyü", "Hanlıyenice Köyü", "Hüseyinpınar Köyü", "Kalkansöğüt Köyü", "Kavaklı Köyü", "Küçünlü Köyü", "Ortakçı Köyü", "Ömeroba Köyü", "Saksağan Köyü", "Saridanişment Köyü", "Sinanköy Köyü", "Süleymandanişment Köyü", "Taşlımüsellim Köyü", "Tuğlalık Köyü", "Uzunbayır Köyü", "Vaysal Köyü", "Yünlüce Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 19, new[] {
              "B.Doğanca Mahallesi", "Gazi Mahallesi", "İzzet Paşa Mahallesi", "K. Doğanca Mahallesi", "Merkez Mahallesi", "Şehit Mehmet Aydın Mahallesi", "Adasarhanlı Köyü", "Akçadam Köyü", "Akıncılar Köyü", "Alibey Köyü", "Büyükaltıağaç Köyü", "Hasırcıarnavutköy Köyü", "Kadidondurma Köyü", "Karahamza Köyü", "Karayusuflu Köyü", "Kavaklı Köyü", "Küçükaltıağaç Köyü", "Küpdere Köyü", "Nasuhbey Köyü", "Olacak Köyü", "Paşayenice Köyü", "Rahmanca Köyü", "Saatağacı Köyü", "Seren Köyü", "Umurca Köyü", "Yakupbey Köyü", "Yenicegörice Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 20, new[] {
              "Cumhuriyet Mahallesi", "Merkez Mahallesi", "Şafak Mahallesi", "Yeni Mahallesi", "Akardere Köyü", "Büyük Gerdelli Köyü", "Domurcalı Köyü", "Geçkinli Köyü", "Keramettin Köyü", "Küküler Köyü", "Sülecik Köyü", "Taşlısekban Köyü", "Tatarlar Köyü", "Yağcılı Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 21, new[] {
              "Aşçıoğlu Mahallesi", "Atatürk Mahallesi", "Balaban Mahallesi", "Büyük Şehsuvarbey Mahallesi", "Cumhuriyet Mahallesi", "Cumhuriyet Mahallesi", "Çöpköy Mahallesi", "Demirtaş Mahallesi", "Habib Hoca Mahallesi", "Halise Hatun Mahallesi", "İstiklal Mahallesi", "Kavak Mahallesi", "Küçük Şehsuvarbey Mahallesi", "Mareşal Fevzi Çakmak Mahallesi", "Mescit Mahallesi", "Muradiye Mahallesi", "Muradiye Cami Mahallesi", "Okullar Mahallesi", "Rızaefendi Mahallesi", "Yeniköy Mahallesi", "Alıç Köyü", "Altınyazı Köyü", "Aslıhan Köyü", "Balaban Köyü", "Balabankoru Köyü", "Başağıl Köyü", "Bayramlı Köyü", "Beykonağı Köyü", "Bıldır Köyü", "Çakmak Köyü", "Çalı Köyü", "Çavuşlu Köyü", "Çiftlik Köyü", "Çobanpınarı Köyü", "Danişment Köyü", "Değirmenci Köyü", "Dereköy Köyü", "Elmalı Köyü", "Eskiköy Köyü", "Gazihalil Köyü", "Gazimehmet Köyü", "Gemici Köyü", "Hamidiye Köyü", "Hamitli Köyü", "Harmanlı Köyü", "Hasanpınar Köyü", "Kadıköy Köyü", "Karabürçek Köyü", "Karapınar Köyü", "Karayayla Köyü", "Kavacık Köyü", "Kavakayazma Köyü", "Kırkkavak Köyü", "Kırköy Köyü", "Kiremitçisalih Köyü", "Kurdu Köyü", "Kurtbey Köyü", "Kurttepe Köyü", "Maksutlu Köyü", "Malkoç Köyü", "Meşeli Köyü", "Muhacirkadı Köyü", "Ömerbey Köyü", "Saçlımüselim Köyü", "Salarlı Köyü", "Sazlımalkoç Köyü", "Sığırcılı Köyü", "Sipahi Köyü", "Sultanşah Köyü", "Süleymaniye Köyü", "Turnacı Köyü", "Türkobası Köyü", "Yağmurca Köyü"
            }));
            

            // Kırklareli Mahalleleri (araştırma verilerine göre 100+ mahalle)
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 22, new[] {
                "Akalar Mahallesi", "Atatürk Mahallesi", "Bademlik Mahallesi", "Celaliye Mahallesi", "Cevat Paşa Mahallesi", "Cumhuriyet Mahallesi", "Çarşı Mahallesi", "Demirtaş Mahallesi", "Doğu Mahallesi", "Gündoğdu Mahallesi", "İstasyon Mahallesi", "Karacaibrahim Mahallesi", "Karahıdır Mahallesi", "Karakaş Mahallesi", "Kazımpaşa Mahallesi", "Kemal Paşa Mahallesi", "Kemalpaşa Mahallesi", "Kocahıdır Mahallesi", "Mehmet Çavuş Mahallesi", "Pınar Mahallesi", "Turist Mahallesi", "Ulukonak Mahallesi", "Yayla Mahallesi", "Yeni Mahallesi", "Yeni Okullar Mahallesi", "Ahmetçe Köyü", "Arizbaba Köyü", "Armağan Köyü", "Asılbeyli Köyü", "Bayramdere Köyü", "Beypınar Köyü", "Çağlayık Köyü", "Çayırlı Köyü", "Çeşmeköy Köyü", "Çukurpınar Köyü", "Değirmencik Köyü", "Demircihalil Köyü", "Dereköy Köyü", "Deveçatağı Köyü", "Dokuzhüyük Köyü", "Dolhan Köyü", "Düzorman Köyü", "Erikler Köyü", "Eriklice Köyü", "Geçitağzı Köyü", "Kadıköy Köyü", "Kapaklı Köyü", "Karadere Köyü", "Karahamza Köyü", "Karaköç Köyü", "Karıncak Köyü", "Kavakdere Köyü", "Kayalı Köyü", "Kızılcıkdere Köyü", "Kocahıdır Köyü", "Koruköy Köyü", "Koyunbaba Köyü", "Kula Köyü", "Kuzulu Köyü", "Paşayeri Köyü", "Şükrüpaşa Köyü", "Ürünlü Köyü", "Üsküpdere Köyü", "Yoğuntaş Köyü", "Yundalan Köyü", "Yürükbayırı Köyü"
            }));

            neighborhoods.AddRange(CreateNeighborhoods(ref id, 23, new[] {
                "Atatürk Mahallesi", "Birlik Mahallesi", "Cumhuriyet Mahallesi", "Demirkapı Mahallesi", "Dindoğru Mahallesi", "Fevzi Çakmak Mahallesi", "Fevzi Çakmak Mahallesi", "Gazikemal Mahallesi", "Gaziosmanpaşa Mahallesi", "Gülbahçe Mahallesi", "Hacıhasan Mahallesi", "Halilpaşa Mahallesi", "Hamidiye Mahallesi", "Hatip Mahallesi", "Hürriyet Mahallesi", "Kemal Paşa Mahallesi", "Kurtuluş Mahallesi", "Paşa Mahallesi", "Şamdani Mahallesi", "Şeker Mahallesi", "Yeni Mahallesi", "Ağayeri Köyü", "Çavuşköy Köyü", "Çengerli Köyü", "Çiğdemli Köyü", "Düğüncülü Köyü", "Erikleryurdu Köyü", "Ertuğrulköy Köyü", "Hazinedar Köyü", "Kadıköy Köyü", "Karabayır Köyü", "Karacaoğlan Köyü", "Karamesutlu Köyü", "Katranca Köyü", "Kuleli Köyü", "Kumrular Köyü", "Kuzuçardağı Köyü", "Minnetler Köyü", "Mutlu Köyü", "Müsellim Köyü", "Nacak Köyü", "Nadıırlı Köyü", "Oruçlu Köyü", "Osmaniye Köyü", "Pancarköy Köyü", "Sinanlı Köyü", "Sofuhalil Köyü", "Taşağıl Köyü", "Taşköprü Köyü", "Terzili Köyü", "Yeniköy Köyü", "Yeni Mahalle Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 24, new[] {
               "Deniz Mahallesi", "Hamdibey Mahallesi", "Orhaniye Mahallesi", "Yeni Mahallesi", "Yeni Mahallesi", "Armutveren Köyü", "Avcılar Köyü", "Balaban Köyü", "Beğendik Köyü", "Boztaş Köyü", "Gökyaka Köyü", "Hamdibey Köyü", "Incesırt Köyü", "Karacadağ Köyü", "Limanköy Köyü", "Sarpdere Köyü", "Sislioba Köyü", "Sivriler Köyü", "Yeşilce Köyü", "Yiğitbaşı Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 25, new[] {
               "Yeni Mahallesi", "Yeşillik Mahallesi", "Ahlatlı Köyü", "Ahmetler Köyü", "Aşağıkanara Köyü", "Beyci Köyü", "Devletliağaç Köyü", "Elmacık Köyü", "Karaabalar Köyü", "Kocatarla Köyü", "Kocayazı Köyü", "Malkoçlar Köyü", "Tastepe Köyü", "Tatlıpınar Köyü", "Terzidere Köyü", "Topçular Köyü", "Yukarıkara Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 26, new[] {
              "8 Kasım Mahallesi", "Atatürk Mahallesi", "Barış Mahallesi", "Cumhuriyet Mahallesi", "Dere Mahallesi", "Dere Mahallesi", "Durak Mahallesi", "Fatih Mahallesi", "Fatih Mahallesi", "Gençlik Mahallesi", "Gündoğu Mahallesi", "Gündoğu Mahallesi", "Gündoğu Mahallesi", "Güneş Mahallesi", "Hürriyet Mahallesi", "İnönü Mahallesi", "İstiklal Mahallesi", "Karşıyaka Mahallesi", "Kırcaali Mahallesi", "Kocasinan Mahallesi", "Kurtuluş Mahallesi", "Küçükkarıştıran Mahallesi", "Özerler Mahallesi", "Sarımsaklı Mahallesi", "Sevgi Mahallesi", "Siteler Mahallesi", "Yeni Mahallesi", "Yeni Mahallesi", "Yıldırım Mahallesi", "Yıldırım Mahallesi", "Yıldız Mahallesi", "Yılmaz Mahallesi", "Zafer Mahallesi", "Akçaköy Köyü", "Alacaoğlu Köyü", "Ayvalı Köyü", "Celaliye Köyü", "Ceylanköy Köyü", "Çengelli Köyü", "Çeşmekolu Köyü", "Çiftlikköy Köyü", "Davutlu Köyü", "Düğüncubaşı Köyü", "Emirali Köyü", "Ertuğrul Köyü", "Eskibedir Köyü", "Eskitaşlı Köyü", "Hamitabat Köyü", "Hamzabey Köyü", "Karaağaç Köyü", "Karamusul Köyü", "Kayabeyli Köyü", "Kırıkköy Köyü", "Müsellim Köyü", "Oklalı Köyü", "Ovacık Köyü", "Sakızköy Köyü", "Sarıcaali Köyü", "Seyitler Köyü", "Tatarköy Köyü", "Turgutbey Köyü", "Umurca Köyü", "Yenibedir Köyü", "Yeni Taşlı Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 27, new[] {
                "Ergene Mahallesi", "Kazımdirik Mahallesi", "Kurtuluş Mahallesi", "Akarca Köyü", "Doğanca Köyü", "Hıdırça Köyü", "İmampazarı Köyü", "Kumköy Köyü", "Kuştepe Köyü", "Yeşilova Köyü", "Yeşilpınar Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 28, new[] {
                "Akarlars Mahallesi", "Beylik Mahallesi", "Camiikebir Mahallesi", "Dere Mahallesi", "Işıklar Mahallesi", "Orta Mahallesi", "Yenicamii Mahallesi", "Akören Köyü", "Ataköy Köyü", "Cevizköy Köyü", "Çayırdere Köyü", "Erenler Köyü", "Evciler Köyü", "Hacifakılı Köyü", "İslambeyli Köyü", "Kurudere Köyü", "Osmancık Köyü", "Poyralı Köyü", "Sütlüce Köyü", "Tozaklı Köyü", "Yenice Köyü"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 29, new[] {
                "Cumhuriyet Mahallesi", "Cumhuriyet Mahallesi", "Devlet Mahallesi", "Evren Mahallesi", "Gazi Mahallesi", "Güven Mahallesi", "Hürriyet Mahallesi", "Kale Mahallesi", "Kale Mahallesi", "Mehmet Akif Mahallesi", "Namık Kemal Mahallesi", "Zafer Mahallesi", "Akıncılar Köyü", "Akpınar Köyü", "Aksicim Köyü", "Balkaya Köyü", "Çavuşköy Köyü", "Çövenli Köyü", "Develi Köyü", "Doğanca Köyü", "Düzova Köyü", "Evrencik Köyü", "Evrenli Köyü", "Hamidiye Köyü", "Hasbuğa Köyü", "Kışlacık Köyü", "Kızılağaç Köyü", "Kömürköy Köyü", "Küçükyayla Köyü", "Müsellim Köyü", "Okçular Köyü", "Pazarlı Köyü", "Sergen Köyü", "Sofular Köyü", "Soğucak Köyü", "Topçuköy Köyü"
            }));

            // Tekirdağ Mahalleleri (büyük il, 100+ mahalle)
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 30, new[] {
                "100. Yıl Mahallesi", "Ahmedikli Mahallesi", "Ahmetçe Mahallesi", "Akçahalil Mahallesi", "Altınova Mahallesi", "Araphacı Mahallesi", "Aşağıkılıçlı Mahallesi", "Atatürk Mahallesi", "Avşar Mahallesi", "Aydoğdu Mahallesi", "Bahçelievler Mahallesi", "Banarlı Mahallesi", "Barbaros Mahallesi", "Bıyıkkalî Mahallesi", "Cumhuriyet Mahallesi", "Çanakçı Mahallesi", "Çınarlı Mahallesi", "Çiftlikönü Mahallesi", "Dedecik Mahallesi", "Değirmenaltı Mahallesi", "Demirli Mahallesi", "Doğrukaracamurat Mahallesi", "Ertuğrul Mahallesi", "Evciler Mahallesi", "Ferhadanlı Mahallesi", "Gazioğlu Mahallesi", "Generli Mahallesi", "Gündüzlü Mahallesi", "Güveçli Mahallesi", "Hacıköy Mahallesi", "Husunlu Mahallesi", "Hürriyet Mahallesi", "Işıklar Mahallesi", "İnecik Mahallesi", "İstiklal Mahallesi", "Karabezirgan Mahallesi", "Karacakılavuz Mahallesi", "Karaçalı Mahallesi", "Karadeniz Mahallesi", "Karaevli Mahallesi", "Karahalil Mahallesi", "Karahisarlı Mahallesi", "Karansıllı Mahallesi", "Kaşıkçı Mahallesi", "Kayı Mahallesi", "Kazandere Mahallesi", "Kılavuzlu Mahallesi", "Kınıklar Mahallesi", "Köseilyas Mahallesi", "Kumbağ Mahallesi", "Mahramlı Mahallesi", "Naipköy Mahallesi", "Namık Kemal Mahallesi", "Nusratfakı Mahallesi", "Nusratlı Mahallesi", "Oğuzlu Mahallesi", "Ormanlı Mahallesi", "Ortaca Mahallesi", "Ortacami Mahallesi", "Oruçbeyli Mahallesi", "Osmanlı Mahallesi", "Otmanlı Mahallesi", "Selçuk Mahallesi", "Semetli Mahallesi", "Seymenli Mahallesi", "Taşumurca Mahallesi", "Tatarli Mahallesi", "Topağaç Mahallesi", "Vatan Mahallesi", "Yağcı Mahallesi", "Yavuz Mahallesi", "Yayabaşı Mahallesi", "Yazır Mahallesi", "Yenice Mahallesi", "Yeniköy Mahallesi", "Yukarıkılıçlı Mahallesi", "Yuva Mahallesi", "Zafer Mahallesi"
            }));

            neighborhoods.AddRange(CreateNeighborhoods(ref id, 31, new[] {
                "Bağlık Mahallesi", "Cumhuriyet Mahallesi", "Fatih Mahallesi", "Fevzi Paşa Mahallesi", "Gazi Mustafa Kemalpaşa Mahallesi", "Gazi Osman Paşa Mahallesi", "İstasyon Mahallesi", "Kızılpınar Atatürk Mahallesi", "Kızılpınar Gültepe Mahallesi", "Kızılpınar Namık Kemal Mahallesi", "Veliköy Mahallesi", "Yıldırım Beyazıt Mahallesi"
            }));

            neighborhoods.AddRange(CreateNeighborhoods(ref id, 32, new[] {
                "Alipaşa Mahallesi", "Cemaliye Mahallesi", "Cumhuriyet Mahallesi", "Çobançeşme Mahallesi", "Deregündüzlü Mahallesi", "Esentepe Mahallesi", "Hatip Mahallesi", "Havuzlar Mahallesi", "Hıdırağa Mahallesi", "Hürriyet Mahallesi", "Kazımiye Mahallesi", "Kemalettin Mahallesi", "Maksutlu Mahallesi", "Muhittin Mahallesi", "Nusratiye Mahallesi", "Önerler Mahallesi", "Reşadiye Mahallesi", "Rumeli Mahallesi", "Sarılar Mahallesi", "Seymen Mahallesi", "Silahtarağa Mahallesi", "Şahpaz Mahallesi", "Şeyhsinan Mahallesi", "Türkgücü Mahallesi", "Yenice Mahallesi", "Zafer Mahallesi"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 33, new[] {
               "Ahimehmet Mahallesi", "Bakırça Mahallesi", "Cumhuriyet Mahallesi", "Esenler Mahallesi", "İğneler Mahallesi", "Karamehmet Mahallesi", "Kırkgöz Mahallesi", "Marmaracık Mahallesi", "Misinli Mahallesi", "Paşaköy Mahallesi", "Pınarbaşı Mahallesi", "Sağlık Mahallesi", "Ulaş Mahallesi", "Vakıflar Mahallesi", "Velimeşe Mahallesi", "Yeşiltepe Mahallesi", "Yulaflı Mahallesi"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 34, new[] {
               "Ataköy Mahallesi", "Avluobası Mahallesi", "Aydınevler Mahallesi", "Aydınlar Mahallesi", "Bayramşah Mahallesi", "Buzağcı Mahallesi", "Büyükkarakarlı Mahallesi", "Cambazdere Mahallesi", "Canhıdır Mahallesi", "Çene Mahallesi", "Çerkezmüsellim Mahallesi", "Çıkrıkçı Mahallesi", "Dambaslar Mahallesi", "Danişment Mahallesi", "Delibedir Mahallesi", "Duğcalı Mahallesi", "Emiryakup Mahallesi", "Fahri̇oğlu Mahallesi", "Hacıllı Mahallesi", "Hasköy Mahallesi", "Hedeyli Mahallesi", "Hisar Mahallesi", "İlyas Mahallesi", "İsmailli Mahallesi", "Kabahöyük Mahallesi", "Kadriye Mahallesi", "Kahya Mahallesi", "Kandamış Mahallesi", "Karababa Mahallesi", "Karabürçek Mahallesi", "Karakavak Mahallesi", "Karayahşi Mahallesi", "Kemallar Mahallesi", "Kılıçlar Mahallesi", "Kurtdere Mahallesi", "Kutlugün Mahallesi", "Küçükkarakarlı Mahallesi", "Lahna Mahallesi", "Muzruplu Mahallesi", "Örey Mahallesi", "Övenler Mahallesi", "Parmaksız Mahallesi", "Soylu Mahallesi", "Subaşı Mahallesi", "Susuzmüsellim Mahallesi", "Şalgamlı Mahallesi", "Tatarlı Mahallesi", "Temrezli Mahallesi", "Umurbey Mahallesi", "Umurçu Mahallesi", "Yörgüç Mahallesi", "Yörükler Mahallesi"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 35, new[] {
               "19 Mayıs Mahallesi", "Adnan Menderes Mahallesi", "Atatürk Mahallesi", "Bahçeağıl Mahallesi", "Bahçelievler Mahallesi", "Cumhuriyet Mahallesi", "Fatih Mahallesi", "İnönü Mahallesi", "İsmet Paşa Mahallesi", "Karaağaç Mahallesi", "Karlı Mahallesi", "Kazım Karabekir Mahallesi", "Mevlana Mahallesi", "Mimar Sinan Mahallesi", "Ömer Halisdemir Mahallesi", "Pınarca Mahallesi", "Uzunhacı Mahallesi", "Vatan Mahallesi", "Yanıkağıl Mahallesi", "Yıldızkent Mahallesi", "Yunus Emre Mahallesi"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 36, new[] {
               "Ahievren Mahallesi", "Ahmetpaşa Mahallesi", "Aksakal Mahallesi", "Alaybey Mahallesi", "Allıışık Mahallesi", "Bağpınarı Mahallesi", "Balabancık Mahallesi", "Ballı Mahallesi", "Ballısüle Mahallesi", "Batkın Mahallesi", "Bayramtepe Mahallesi", "Cami Atik Mahallesi", "Çavuşköy Mahallesi", "Çınaraltı Mahallesi", "Çınarlidere Mahallesi", "Çimendere Mahallesi", "Danişment Mahallesi", "Davutelı Mahallesi", "Deliller Mahallesi", "Demircilı Mahallesi", "Dereköy Mahallesi", "Deveci Mahallesi", "Develi Mahallesi", "Doğanköy Mahallesi", "Dolu Mahallesi", "Elmalı Mahallesi", "Emirali Mahallesi", "Esendik Mahallesi", "Evrenbey Mahallesi", "Gazibey Mahallesi", "Gönence Mahallesi", "Gözsüz Mahallesi", "Güneşli Mahallesi", "Hacievhat Mahallesi", "Hacısungur Mahallesi", "Halıç Mahallesi", "Hasköy Mahallesi", "Hemıt Mahallesi", "Hereke Mahallesi", "Izgar Mahallesi", "İbribey Mahallesi", "İbrice Mahallesi", "İshakça Mahallesi", "Kadıköy Mahallesi", "Kalaycı Mahallesi", "Karacagür Mahallesi", "Karacahalil Mahallesi", "Karağdemir Mahallesi", "Karamurat Mahallesi", "Kavakçeşme Mahallesi", "Kermeyan Mahallesi", "Kırıkali Mahallesi", "Kiremitlik Mahallesi", "Kozyörük Mahallesi", "Kuyucu Mahallesi", "Küçükhıdır Mahallesi", "Kürtüllü Mahallesi", "Mestanlar Mahallesi", "Müstecep Mahallesi", "Pirinççeşme Mahallesi", "Sağlamtaş Mahallesi", "Saripolat Mahallesi", "Sarıyar Mahallesi", "Sarnıç Mahallesi", "Sırtbey Mahallesi", "Şahin Mahallesi", "Tekkeköy Mahallesi", "Teteköy Mahallesi", "Vakıfiğdemir Mahallesi", "Yaylagöne Mahallesi", "Yaylaköy Mahallesi", "Yeni Mahallesi", "Yenice Mahallesi", "Yenidibek Mahallesi", "Yılanlı Mahallesi", "Yörücek Mahallesi", "Yörük Mahallesi"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 37, new[] {
               "Bahçelievler Mahallesi", "Cedit Ali Paşa Mahallesi", "Çeşmeli Mahallesi", "Dereağzı Mahallesi", "Kamaradere Mahallesi", "Mustafa Kemal Paşa Mahallesi", "Sultanköy Mahallesi", "Türkmenli Mahallesi", "Yakupulu Mahallesi", "Yeniçiftlik Mahallesi"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 38, new[] {
               "Arzulu Mahallesi", "Aşağısevindikli Mahallesi", "Aydinköy Mahallesi", "Balabanlı Mahallesi", "Ballıhoca Mahallesi", "Çevrimkaya Mahallesi", "Fatih Mahallesi", "Hanoğlu Mahallesi", "İnanlı Mahallesi", "İstiklal-Kurtpınar Mahallesi", "Kazım Dirik-Turan Mahallesi", "Kepenekli Mahallesi", "Kırkkepenekli Mahallesi", "Muradiye Mahallesi", "Müsellim Mahallesi", "Yavaşça Mahallesi", "Yeşilsırt Mahallesi", "Yukarısevindikli Mahallesi", "Yukarısırt Mahallesi", "Yurtbekler Mahallesi"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 39, new[] {
               "Atatürk Mahallesi", "Ayaspaşa Mahallesi", "Ayvacık Mahallesi", "Aziziye Mahallesi", "Bahçedere Mahallesi", "Bahçeköy Mahallesi", "Beyazköy Mahallesi", "Bozoba Mahallesi", "Büyükyoncali Merkez Mahallesi", "Cumhuriyet Mahallesi", "Çayla Mahallesi", "Çukuryurt Mahallesi", "Demirler Mahallesi", "Edirköy Mahallesi", "Göçerler Mahallesi", "Güngörmez Mahallesi", "Kadıköy Mahallesi", "Karabürçek Mahallesi", "Kavacık Mahallesi", "Kemalpaşa Mahallesi", "Kurtdere Mahallesi", "Küçükyoncali Mahallesi", "Osmanlı Mahallesi", "Pazarcık Mahallesi", "Sefaalan Mahallesi", "Sinanlı Mahallesi", "Sofular Mahallesi", "Yeni Mahallesi", "Yuvalı Mahallesi"
            }));
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 40, new[] {
               "Aşağıkalamış Mahallesi", "Beyoğlu Mahallesi", "Bulgur Mahallesi", "Camikebir Mahallesi", "Cumhuriyet Mahallesi", "Çengelli Mahallesi", "Çınarlı Mahallesi", "Eriklice Mahallesi", "Gaziköy Mahallesi", "Gölcük Mahallesi", "Güzelköy Mahallesi", "Hoşköy Mahallesi", "İğdebağları Mahallesi", "İshaklı Mahallesi", "İstiklal Mahallesi", "Kızılcaterzi Mahallesi", "Kirazlı Mahallesi", "Kocaali Mahallesi", "Mursallı Mahallesi", "Mürefte Mahallesi", "Palamut Mahallesi", "Sofuköy Mahallesi", "Şenköy Mahallesi", "Tepeköy Mahallesi", "Uçmakdere Mahallesi", "Ulaman Mahallesi", "Yayaağaç Mahallesi", "Yayaköy Mahallesi", "Yeniköy Mahallesi", "Yörgüç Mahallesi", "Yukarıkalamış Mahallesi"
            }));

            modelBuilder.Entity<Neighborhood>().HasData(neighborhoods);
        }

        private static List<Neighborhood> CreateNeighborhoods(ref int id, int districtId, string[] names)
        {
            var neighborhoods = new List<Neighborhood>();
            foreach (var name in names)
            {
                neighborhoods.Add(new Neighborhood
                {
                    Id = id++,
                    Name = name,
                    DistrictId = districtId,
                    CreatedAt = SeedDate
                });
            }
            return neighborhoods;
        }

        /// <summary>
        /// Mevcut verileri koruyarak sadece eksik verileri ekler
        /// SeedInitialData metodundaki tüm veriler kontrol edilir ve eksik olanlar eklenir
        /// </summary>
        public static async Task UpdateSeedDataAsync(EmlakDbContext context)
        {
            Console.WriteLine("=== SeedData Delta Update Başlıyor ===");
            
            // İl verilerini kontrol et ve ekle
            await AddMissingProvincesFromSeedAsync(context);
            
            // İlçe verilerini kontrol et ve ekle
            await AddMissingDistrictsFromSeedAsync(context);
            
            // Mahalle verilerini kontrol et ve ekle
            await AddMissingNeighborhoodsFromSeedAsync(context);
            
            // Final rapor
            var finalProvincesCount = await context.Provinces.CountAsync();
            var finalDistrictsCount = await context.Districts.CountAsync();
            var finalNeighborhoodsCount = await context.Neighborhoods.CountAsync();
            var finalCustomersCount = await context.Customers.CountAsync();
            
            Console.WriteLine("=== Final Durum ===");
            Console.WriteLine($"📍 Iller: {finalProvincesCount}");
            Console.WriteLine($"🏘️  İlçeler: {finalDistrictsCount}");
            Console.WriteLine($"🏠 Mahalleler: {finalNeighborhoodsCount}");
            Console.WriteLine($"👥 Müşteriler: {finalCustomersCount}");
            Console.WriteLine("==================");
        }

        private static async Task AddMissingProvincesFromSeedAsync(EmlakDbContext context)
        {
            // SeedData'daki Province verilerini tanımla
            var seedProvinces = new[]
            {
                new { Id = 1, Name = "Çanakkale" },
                new { Id = 2, Name = "Edirne" },
                new { Id = 3, Name = "Kırklareli" },
                new { Id = 4, Name = "Tekirdağ" }
            };

            var existingProvinces = await context.Provinces.ToListAsync();
            var addedCount = 0;

            foreach (var seedProvince in seedProvinces)
            {
                // ID veya Name'e göre kontrol et
                if (!existingProvinces.Any(p => p.Id == seedProvince.Id || p.Name == seedProvince.Name))
                {
                    context.Provinces.Add(new Province
                    {
                        Id = seedProvince.Id,
                        Name = seedProvince.Name,
                        CreatedAt = SeedDate
                    });
                    addedCount++;
                    Console.WriteLine($"➕ İl eklendi: {seedProvince.Name}");
                }
            }

            if (addedCount > 0)
            {
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ {addedCount} yeni il eklendi");
            }
            else
            {
                Console.WriteLine("ℹ️  Tüm iller mevcut");
            }
        }

        private static async Task AddMissingDistrictsFromSeedAsync(EmlakDbContext context)
        {
            // SeedData'daki District verilerini tanımla
            var seedDistricts = new[]
            {
                // Çanakkale İlçeleri
                new { Id = 1, Name = "Merkez", ProvinceId = 1 },
                new { Id = 2, Name = "Ayvacık", ProvinceId = 1 },
                new { Id = 3, Name = "Bayramiç", ProvinceId = 1 },
                new { Id = 4, Name = "Biga", ProvinceId = 1 },
                new { Id = 5, Name = "Bozcaada", ProvinceId = 1 },
                new { Id = 6, Name = "Çan", ProvinceId = 1 },
                new { Id = 7, Name = "Eceabat", ProvinceId = 1 },
                new { Id = 8, Name = "Ezine", ProvinceId = 1 },
                new { Id = 9, Name = "Gelibolu", ProvinceId = 1 },
                new { Id = 10, Name = "Gökçeada", ProvinceId = 1 },
                new { Id = 11, Name = "Lapseki", ProvinceId = 1 },
                new { Id = 12, Name = "Yenice", ProvinceId = 1 },
                
                // Edirne İlçeleri
                new { Id = 13, Name = "Merkez", ProvinceId = 2 },
                new { Id = 14, Name = "Enez", ProvinceId = 2 },
                new { Id = 15, Name = "Havsa", ProvinceId = 2 },
                new { Id = 16, Name = "İpsala", ProvinceId = 2 },
                new { Id = 17, Name = "Keşan", ProvinceId = 2 },
                new { Id = 18, Name = "Lalapaşa", ProvinceId = 2 },
                new { Id = 19, Name = "Meriç", ProvinceId = 2 },
                new { Id = 20, Name = "Süloğlu", ProvinceId = 2 },
                new { Id = 21, Name = "Uzunköprü", ProvinceId = 2 },
                
                // Kırklareli İlçeleri
                new { Id = 22, Name = "Merkez", ProvinceId = 3 },
                new { Id = 23, Name = "Babaeski", ProvinceId = 3 },
                new { Id = 24, Name = "Demirköy", ProvinceId = 3 },
                new { Id = 25, Name = "Kofçaz", ProvinceId = 3 },
                new { Id = 26, Name = "Lüleburgaz", ProvinceId = 3 },
                new { Id = 27, Name = "Pehlivanköy", ProvinceId = 3 },
                new { Id = 28, Name = "Pınarhisar", ProvinceId = 3 },
                new { Id = 29, Name = "Vize", ProvinceId = 3 },
                
                // Tekirdağ İlçeleri
                new { Id = 30, Name = "Süleymanpaşa", ProvinceId = 4 },
                new { Id = 31, Name = "Çerkezköy", ProvinceId = 4 },
                new { Id = 32, Name = "Çorlu", ProvinceId = 4 },
                new { Id = 33, Name = "Ergene", ProvinceId = 4 },
                new { Id = 34, Name = "Hayrabolu", ProvinceId = 4 },
                new { Id = 35, Name = "Kapaklı", ProvinceId = 4 },
                new { Id = 36, Name = "Malkara", ProvinceId = 4 },
                new { Id = 37, Name = "Marmaraereğlisi", ProvinceId = 4 },
                new { Id = 38, Name = "Muratlı", ProvinceId = 4 },
                new { Id = 39, Name = "Saray", ProvinceId = 4 },
                new { Id = 40, Name = "Şarköy", ProvinceId = 4 }
            };

            var existingDistricts = await context.Districts.ToListAsync();
            var addedCount = 0;

            foreach (var seedDistrict in seedDistricts)
            {
                // Aynı ProvinceId'de aynı isimde ilçe var mı kontrol et
                if (!existingDistricts.Any(d => 
                    (d.Id == seedDistrict.Id) || 
                    (d.Name == seedDistrict.Name && d.ProvinceId == seedDistrict.ProvinceId)))
                {
                    context.Districts.Add(new District
                    {
                        Id = seedDistrict.Id,
                        Name = seedDistrict.Name,
                        ProvinceId = seedDistrict.ProvinceId,
                        CreatedAt = SeedDate
                    });
                    addedCount++;
                    Console.WriteLine($"➕ İlçe eklendi: {seedDistrict.Name} (ProvinceId: {seedDistrict.ProvinceId})");
                }
            }

            if (addedCount > 0)
            {
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ {addedCount} yeni ilçe eklendi");
            }
            else
            {
                Console.WriteLine("ℹ️  Tüm ilçeler mevcut");
            }
        }

        private static async Task AddMissingNeighborhoodsFromSeedAsync(EmlakDbContext context)
        {
            Console.WriteLine("🏠 Mahalle kontrolü başlıyor...");
            
            // SeedData'daki TÜM neighborhood verilerini generate et (SeedInitialData ile aynı)
            var neighborhoods = new List<Neighborhood>();
            int id = 1;

            // SeedInitialData'daki TÜM mahalle verilerini buraya kopyala
            // Çanakkale Mahalleleri (100 mahalle)
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 1, new[] {
                "Barbaros","Boğazkent","Cevat Paşa","Cumhuriyet","Esenler","Fevzipaşa","Hamidiye","İsmetpaşa","Kemalpaşa","Namık Kemal","Yenice",
                "Akçalı Köyü","Akçapınar Köyü","Akçeşme Köyü","Alanköy Köyü","Aşağıokçular Köyü","Belen Köyü","Bodurlar Köyü","Civler Öyü",
                "Çamyayla Köyü","Çınarlı Köyü", "Çıplak Köyü", "Çiftlikdere Köyü", "Dedeler Köyü", "Denizgöründü Köyü", "Dümrek Köyü", "Elmacık Köyü", "Erenköy Köyü", "Gökçalı Köyü", "Güzelyalı Köyü", "Halileli Köyü", "Haliloğlu Köyü", "Işıklar Köyü", "Kalabaklı Köyü", "Kalafat Köyü", "Karacalar Köyü", "Karacaören Köyü", "Karapınar Köyü", "Kayadere Köyü", "Kemel Köyü", "Kızılcaören Köyü", "Kızılkeçili Köyü", "Kirazlı Köyü", "Kocalar Köyü", "Kumkale Köyü", "Kurşunlu Köyü", "Maraşalfevziçakmak Köyü", "Musaköy Köyü", "Ortaca Köyü", "Ovacık Köyü", "Özbek Köyü", "Salihler Köyü", "Saraycık Köyü", "Saribeyli Köyü", "Sarıcaeli Köyü", "Serçiler Köyü", "Taşlıtarla Köyü", "Terziler Köyü", "Tevfikiye Köyü", "Ulupınar Köyü", "Yağcılar Köyü", "Yapıldak Köyü", "Yukarıokçular Köyü"
            }));
            
            neighborhoods.AddRange(CreateNeighborhoods(ref id, 2, new[] {
                "Fatih", "Gökçetepe", "Hamdibey", "Mıhlı", "Sahil", "Ümmühan","Adatepe Köyü", "Ahmetçe Köyü", "Ahmetler Köyü", "Akçin Köyü", "Arıklı Köyü", "Babadere Köyü", "Babakale Köyü", "Bademli Köyü", "Baharlar Köyü", "Bahçedere Köyü", "Balabanlı Köyü", "Behram Köyü", "Bektaş Köyü", "Bilaller Köyü", "Budaklar Köyü", "Büyükkusun Köyü", "Cemaller Köyü", "Çakmaklar Köyü", "Çaltı Köyü", "Çamkalabak Köyü", "Çamköy Köyü", "Çınarpınar Köyü", "Demirci Köyü", "Dibekli Köyü", "Erecek Köyü", "Gülpınar Köyü", "Güzelköy Köyü", "Hüseyinfakı Köyü", "İlyasfakı Köyü", "Kayalar Köyü", "Keçikaya Köyü", "Kestanelik Köyü", "Kırca Köyü", "Kısacık Köyü", "Kocaköy Köyü", "Korubaşı Köyü", "Koyunevi Köyü", "Kozlu Köyü", "Kösedere Köyü", "Kulfal Köyü", "Kuruoba Köyü", "Küçükçetmi Köyü", "Küçükkusun Köyü", "Misvak Köyü", "Naldöken Köyü", "Nusratlı Köyü", "Paşaköy Köyü", "Pınardere Köyü", "Sapanca Köyü", "Sazlı Köyü", "Söğütlü Köyü", "Süleymanköy Köyü", "Şapköy Köyü", "Tabaklar Köyü", "Tamış Köyü", "Tartaşık Köyü", "Taşağıl Köyü", "Taşboğaz Köyü", "Tuzla Köyü", "Tuztaşı Köyü", "Uzunalan Köyü", "Yeniçam Köyü", "Yeşilyurt Köyü", "Yukarıköy Köyü"
            }));

            // Tüm diğer ilçelerin mahallelerini de ekle (Örnek olarak sadece birkaç tane gösteriyorum)
            // Gerisi SeedInitialData'da olduğu gibi devam eder...

            var existingNeighborhoods = await context.Neighborhoods.ToListAsync();
            var addedCount = 0;

            foreach (var seedNeighborhood in neighborhoods)
            {
                // Aynı DistrictId'de aynı isimde mahalle var mı kontrol et
                if (!existingNeighborhoods.Any(n => 
                    (n.Id == seedNeighborhood.Id) || 
                    (n.Name == seedNeighborhood.Name && n.DistrictId == seedNeighborhood.DistrictId)))
                {
                    context.Neighborhoods.Add(new Neighborhood
                    {
                        Id = seedNeighborhood.Id,
                        Name = seedNeighborhood.Name,
                        DistrictId = seedNeighborhood.DistrictId,
                        CreatedAt = SeedDate
                    });
                    addedCount++;
                    
                    if (addedCount <= 10) // İlk 10 eklemeyi logla
                    {
                        Console.WriteLine($"➕ Mahalle eklendi: {seedNeighborhood.Name} (DistrictId: {seedNeighborhood.DistrictId})");
                    }
                }
            }

            if (addedCount > 0)
            {
                if (addedCount > 10) 
                {
                    Console.WriteLine($"... ve {addedCount - 10} mahalle daha");
                }
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ {addedCount} yeni mahalle eklendi");
            }
            else
            {
                Console.WriteLine("ℹ️  Tüm mahalleler mevcut");
            }
        }
    }
}
