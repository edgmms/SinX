# SinX İş Planı ve Agent Yürütme Kaydı

Bu dosya, kullanıcı tarafından onaylanmış çok adımlı geliştirme planlarının tek doğruluk kaynağıdır. Tek dosyalık küçük işler için plan kaydı zorunlu değildir.

## Çalışma biçimi

1. Kullanıcı iş planını doğal dilde iletir.
2. `product_owner` planı aşağıdaki iş kalemlerine dönüştürür; kapsam, bağımlılık ve kabul kriterlerini netleştirir.
3. İlgili uzman agent'lar teknik sözleşme, risk, doğrulama ve rollback girdisi üretir.
4. `product_owner` uzman çıktılarındaki tekrar ve çelişkileri giderir; doğrulanmış bilgi, varsayım ve açık soruları ayırarak geliştirici uygulama paketini hazırlar.
5. `senior_fullstack_dotnet_developer` yalnız Hazır durumdaki uygulama paketini repository standardına göre uygular.
6. Bağımsız işler paralel çalışabilir. Aynı dosya, migration veya contract'a dokunan işler sıralı yürütülür.
7. Build, test ve mimari kontroller tamamlandıktan sonra iş kaleminin kanıtları kaydedilir.
8. `product_owner` uygulamayı paket ve kabul kriterlerine göre değerlendirir; kullanıcıya tamamlananları, kalan riskleri ve sıradaki işi bildirir.

## Durumlar

| Durum | Anlamı |
|---|---|
| `Taslak` | Kapsam henüz kesinleşmedi |
| `Hazır` | Bağımlılıklar ve kabul kriterleri tamam |
| `Çalışıyor` | Bir agent tarafından yürütülüyor |
| `İncelemede` | Kod tamamlandı; test/review bekliyor |
| `Bloke` | Kullanıcı kararı veya haricî koşul gerekiyor |
| `Tamamlandı` | Kabul kriterleri ve doğrulama kanıtları tamam |

## Agent rolleri

| Rol | Sorumluluk |
|---|---|
| `product_owner` | Planı parçalar, agent atar, dosya çakışmasını önler, kabul kriterlerini ve sonuçları yönetir |
| Uzman agent | Teknik gereksinim ve sözleşme sağlar; ürün kodu yazmaz |
| `senior_fullstack_dotnet_developer` | Backend, UI, Worker, adapter, test, migration ve geliştirme dokümantasyonunu uygular |
| Review/test agent | Değişikliği bağımsız inceler; güvenlik, mimari ve kabul kriterlerini doğrular |

## İş kalemi şablonu

Her iş kalemi aşağıdaki formatta eklenir:

```markdown
### SX-000 — Kısa başlık

- Durum: Taslak | Hazır | Çalışıyor | İncelemede | Bloke | Tamamlandı
- Amaç:
- Kapsam:
- Kapsam dışı:
- Bağımlılıklar:
- Birincil uzman agent:
- Destek agent'ları:
- Implementasyon agent'ı: senior_fullstack_dotnet_developer
- Etkilenecek projeler/dosyalar:
- Güvenlik ve tenant etkisi:
- Migration/rollback etkisi:

Geliştirici uygulama paketi:

- Kullanıcı değeri ve beklenen davranış:
- Ön koşullar:
- İş kuralları:
- Ekran ve kullanıcı akışı:
- Model ve contract gereksinimleri:
- Provider/platform teknik sözleşmesi:
- Hata ve edge-case davranışı:
- Authorization, tenant, secret, audit ve idempotency gereksinimleri:
- Dosya/proje sahipliği ve sınırları:
- Migration ve rollback/compensation:
- Açık varsayımlar ve bloke konular:
- Yararlanılan uzman çıktıları:

Kabul kriterleri:

1. ...
2. ...

Doğrulama:

- Build:
- Test:
- Windows VM testi:
- Manuel kontrol:

Kararlar ve kanıtlar:

- ...
```

## Yürütme kuralları

- Kullanıcı `planı hazırla` derse yalnız plan oluşturulur; kod yazılmaz.
- Kullanıcı `planı uygula` veya eşdeğer açık talimat verirse hazır iş kalemleri uygulanır.
- Kullanıcı belirli iş kalemlerini onaylarsa yalnız onlar başlatılır.
- Uzman çıktıları Product Owner tarafından geliştirici uygulama paketine dönüştürülmeden iş kalemi `Hazır` yapılamaz ve implementasyon agent'ına atanamaz.
- Uygulama paketi, geliştiricinin ihtiyaç duyduğu tekil iş tarifidir; repository standartları ve ürün dokümanlarıyla çelişemez.
- Aynı anda çalışan agent'lara çakışan dosya sahipliği verilmez.
- Uzman çıktısı tamamlanmadan provider/Windows davranışı varsayılarak kodlanmaz.
- Her iş kalemi kendi kabul kriteri ve doğrulama kanıtıyla kapanır.
- Yeni mimari karar gerekiyorsa ilgili iş durdurulur ve `docs/adr/` altında karar hazırlanır.
- Production, gerçek sunucu veya haricî hesaplarda işlem plan kapsamına otomatik olarak dahil edilmez.

## Aktif plan

### SinX başlangıç ekranları — Setup ve yönetici Login (uygulanıyor)

**Ürün hedefi:** Windows Server üzerinde self-hosted SinX yönetim paneli için `/setup` ve `/login` ASP.NET Core MVC ekranlarını, Gentelella v4'ün yalnız yerel ve sürümü sabit varlıklarıyla hazırlamak. Setup ekranı yalnız ilk yönetici hesabını oluşturma akışını; Login ekranı yalnız bu kurulumda sabitlenen Windows SID'li hesabın yönetici oturumunu kapsar.

**Planlama sınırı:** Plesk kapsam dışıdır. Kullanıcı, SX-001 → SX-004 sırasındaki planın uygulanmasını onayladı; kalemler bağımlılık sırasıyla yürütülür.

**UI teknoloji sınırı:** Kullanıcının son kararı bağlayıcıdır: tüm ekranlar yalnız ASP.NET Core MVC ile, `Controllers/`, Web'e ait `ViewModels/` ve `Views/*.cshtml` yerleşiminde geliştirilir. Razor Pages (`Pages/`, `.cshtml.cs`) ve Blazor (`.razor`, component runtime veya ilişkili bağımlılıklar) yasaktır.

**Başlangıç host sınırı:** Kullanıcının son kararı bağlayıcıdır: bu uygulama yalnız `http://localhost:8080` üzerinde çalışır; `57500` kullanılmaz. Bu başlangıç kapsamı domain paneli yayınlamaz. HTTP credential/session şifrelemediği için Setup ve Login'de görünür güvenlik uyarısı korunur; HTTPS yalnız ayrı bir gelecek karar ile eklenir.

**Yerelleştirme sınırı:** Kullanıcının kapsam genişletmesi onaylandı: görünür Setup/Login/public-layout ve Web katmanı validation metinleri `IStringLocalizer` ve `.resx` kaynaklarıyla `tr-TR`/`en-US` için sunulur. Dil seçimi culture cookie ile route-independent kalır; varsayılan/fallback `tr-TR` olur. Domain/application error code'ları sabit kalır; Web mapping katmanı yalnız kullanıcı metnini yerelleştirir.

**Kaynak ağacı sınırı:** Kullanıcının onaylı kararıyla solution ve bütün test projeleri de `src/` altında bulunur: `src/SinX.sln` ve `src/tests/`. Repository kökünde `SinX.sln` veya `tests/` kalmaz; tüm build/test/doküman/agent referansları bu yapıyı kullanır.

**Doğrulanmış başlangıç durumu:** Başlangıç iskeleti artık `src/SinX.sln` ve `src/tests/` altında bulunur; kaynak/test yerleşimi tek `src/` ağacındadır.

**Sıralama:** `SX-001 → SX-002 → SX-003 → SX-004`. SX-002, görünür ekran kabuğunu; SX-003 ve SX-004 gerçek güvenlik davranışını tamamlar. Hiçbir kalem, onay olmadan başlatılmaz.

### SX-006 — Solution ve test ağacını src altına taşıma

- Durum: İncelemede
- Amaç: Kullanıcının kaynak-ağacı standardına göre solution ve tüm test projelerini `src/` altında toplamak.
- Kapsam: `SinX.sln` → `src/SinX.sln`; `tests/` → `src/tests/`; solution project path'leri, test/build komutları, doküman ve AGENTS/PLANS referansları güncellemesi.
- Kapsam dışı: Proje isimleri, assembly namespace'leri, package sürümleri, MVC/yerelleştirme/Setup davranışı değişikliği.
- Bağımlılıklar: Yok; mevcut worktree değişiklikleri korunur.
- Birincil uzman agent: `senior_fullstack_dotnet_developer`.
- Implementasyon agent'ı: senior_fullstack_dotnet_developer.
- Etkilenecek projeler/dosyalar: `SinX.sln`, `tests/`, `src/tests/`, `AGENTS.md`, `src/AGENTS.md`, `docs/`, `PLANS.md` ve path içeren build/test yapılandırmaları.
- Güvenlik ve tenant etkisi: Yok.
- Migration/rollback etkisi: Yok; sürüm kontrolü hareketleri geri alınabilir.

Geliştirici uygulama paketi:

- Kullanıcı değeri ve beklenen davranış: Tüm ürün kaynakları, solution ve testler tek `src/` ağacında bulunur; geliştirici `src/SinX.sln` üzerinden build/test başlatır.
- Ön koşullar: Mevcut değişiklikler korunur; dosya hareketleri izlenebilirliği korumak için mümkün olduğunda `git mv` ile yapılır.
- İş kuralları: Yeni root-level `SinX.sln` veya `tests/` oluşturulmaz. Solution proje göreli yolları `src/SinX.sln` konumuna göre doğru çözülür.
- Dosya/proje sahipliği ve sınırları: Yalnız taşıma ve path referansları; kaynak davranışı/asset/auth kodu değişmez.
- Doğrulama: `rg` sonucu aktif referanslarda root `SinX.sln` veya `tests/` path'i bulunmaz; `git diff --check` geçer. SDK 10.0.403 yoksa build/test engeli açıkça kaydedilir.
- Açık varsayımlar ve bloke konular: SDK blocker taşıma işini engellemez; gerçek build/test yalnız uygun SDK ile sonradan doğrulanır.

Kabul kriterleri:

1. Solution `src/SinX.sln`, test projesi `src/tests/SinX.Web.Tests` altında bulunur.
2. Solution içindeki tüm proje referansları geçerlidir; root-level eski hedefler bulunmaz.
3. AGENTS, docs ve PLANS test/solution path'lerini yeni konuma yönlendirir.
4. MVC/yerelleştirme dosyaları değişmeden kalır; `git diff --check` geçer.

Kararlar ve kanıtlar:

- 2026-10-27: Kullanıcı onayıyla implementasyon başlatıldı.
- 2026-10-27: `SinX.sln` `src/SinX.sln` konumuna, `tests/` ise `src/tests/` konumuna taşındı; solution proje göreli yolları ve active path referansları güncellendi. Kök düzeyde eski hedefler yoktur.
- 2026-10-27: `git diff --check` ve root-target yokluk kontrolleri geçti. `dotnet build src/SinX.sln --no-restore` .NET SDK 10.0.403 eksikliği (yalnız 9.0.121 mevcut) nedeniyle çalışmadı; uygun SDK ile build/test tekrar edilmeden kalem Tamamlandı olmaz.

### SX-001 — Web çözüm ve test kabuğu

- Durum: Çalışıyor
- Amaç: MVC tabanlı Setup/Login ekranlarını taşıyabilecek, Clean Architecture bağımlılık yönünü koruyan en küçük çözüm ve test temelini kurmak.
- Kapsam: Mevcut izinli proje adlarıyla `src/SinX.sln`; `SinX.Web` Windows Service/Kestrel host başlangıcı; uygulama katmanı sınırları; test projeleri; route temel kaydı; static-file ve MVC altyapısı.
- Kapsam dışı: Windows hesabı oluşturma, gerçek credential doğrulama, Worker IPC, SQLite şeması dışındaki ürün özellikleri, dashboard ve domain ekranları.
- Bağımlılıklar: Yok.
- Birincil uzman agent: `senior_fullstack_dotnet_developer` (uygulama keşfi doğrulandı).
- Destek agent'ları: `web_security_specialist`.
- Implementasyon agent'ı: senior_fullstack_dotnet_developer.
- Etkilenecek projeler/dosyalar: Yeni, yalnız izinli proje adlarında `src/SinX.sln`, `src/SinX.Web/`, gerekli iç katman projeleri ve karşılık gelen `src/tests/SinX.*Tests/`; `Directory.Build.props`, `Directory.Packages.props`, `global.json` yalnız uyum gerektirirse.
- Güvenlik ve tenant etkisi: Başlangıç hostu yalnız localhost `8080`'dir; domain paneli bu kalemde yayınlanmaz. Gelecekte tenant endpoint'i eklenirse ayrı host/policy kararı gerekir.
- Migration/rollback etkisi: Migration yoktur. Yeni iskelet geri alınabilir dosya ekleme niteliğindedir; sürüm kontrolünden geri alma yeterlidir.

Geliştirici uygulama paketi:

- Kullanıcı değeri ve beklenen davranış: Uygulama IIS'e bağımlı olmadan MVC Views sunabilmeli; `/setup` ve `/login` için sonraki kalemlerin bağlanacağı güvenli, test edilebilir bir host bulunmalıdır.
- Ön koşullar: Kök ve `src/AGENTS.md` kuralları; .NET 10 LTS, nullable ve locked dependency politikası uygulanır. Yeni proje adları yalnız `src/AGENTS.md` izin listesinden seçilir.
- İş kuralları: Web yalnız UI/authentication/validation koordinasyonudur; ayrıcalıklı Windows işlemi doğrudan çalıştırmaz. Controller iş kuralı içermez.
- Ekran ve kullanıcı akışı: Anonim kullanıcı yalnız public ekran kabuğuna ulaşır; yönetim/domain menüsü veya mock dashboard sunulmaz.
- Model ve contract gereksinimleri: UI InputModel/ViewModel yalnız Web'de kalır. Uygulama use-case ve Windows port sözleşmeleri SX-003/SX-004'te eklenecektir; domain entity veya UI model API/IPC contract yapılmaz.
- Provider/platform teknik sözleşmesi: Kestrel/Windows Service hedefi yalnız `localhost:8080`; Web process düşük yetkili kalır. `57500` listener'ı veya domain endpoint'i bulunmaz.
- Hata ve edge-case davranışı: Başlangıç yapılandırması geçersizse fail-fast ve secret içermeyen hata/correlation bilgisi üretir; geliştirme/demo credential'ı veya mock yönetici yaratılmaz.
- Authorization, tenant, secret, audit ve idempotency gereksinimleri: Anonim erişim yalnız setup/login'e açık olur; diğer yeni route'lar varsayılan reddedilir. Secret kalıcılaştırılmaz; audit altyapısı yoksa audit gerektiren iş akışları SX-003/SX-004 dışında tamamlanmış sayılmaz.
- Dosya/proje sahipliği ve sınırları: Geliştirici yalnız yeni solution, izinli `src` projeleri ve karşılık gelen test projelerine dokunur; provider/adapter veya Plesk kodu eklemez.
- Migration ve rollback/compensation: DB şeması ve Windows state değişikliği yoktur; temiz geri alma kaynak dosyalarının kaldırılmasıdır.
- Açık varsayımlar ve bloke konular: Dokümanların izin verdiği mevcut proje adlarıyla solution oluşturmak ADR gerektirmez. Paket sürümleri mevcut merkezi paket politikasına uygun seçilir; yeni framework/major bağımlılık eklenmez.
- Yararlanılan uzman çıktıları: Repository teknik keşfi; mimari ve teknoloji dokümanları.

Kabul kriterleri:

1. Solution ve eklenen proje referansları `Web -> Application -> Domain` yönünü ihlal etmez; Web, Worker veya Windows adapter'ını doğrudan çağırmaz.
2. Web host IIS olmadan Kestrel üzerinde yalnız `localhost:8080` adresinde başlar ve `/setup` ile `/login` route'larını MVC controller action'larına yönlendirebilir.
3. Varsayılan yetkilendirme reddedicidir; `57500` listener'ı veya domain panel route'u yayınlanmaz.
4. Warning'siz build ve en az route/authorization smoke testi bulunur.

Doğrulama:

- Build: `dotnet build` (warning'siz).
- Test: WebApplicationFactory ile route ve varsayılan authorization smoke testleri.
- Windows VM testi: Windows Service/Kestrel host doğrulaması, sonraki Windows CI ortamında.
- Manuel kontrol: `http://localhost:8080/setup` ve `/login` erişimi; `57500` listener'ının bulunmaması.

Kararlar ve kanıtlar:

- 2026-10-27: Kullanıcı SX-001 → SX-004 planını onayladı; implementasyon başlatıldı.
- 2026-10-27: İlk iskelet Razor Pages ile oluşturuldu; kullanıcı MVC kararını verdiği için `Pages/` ve `.cshtml.cs` dosyaları MVC `Controllers/`, `ViewModels/`, `Views/` yapısına güvenli biçimde dönüştürülecek. Bu dönüşüm bitene kadar kalem Çalışıyor durumundadır.
- 2026-10-27: `xmllint --noout` ve `git diff --check` geçti. Build/test kanıtı bloke: ortam yalnız .NET SDK 9.0.121 içeriyor, repository `global.json` ile 10.0.403 istiyor. .NET 10 ortamında build/test tekrar koşulmadan kalem Tamamlandı olmaz.

### SX-002 — Yerel Gentelella varlıkları ve public MVC ekran kabuğu

- Durum: İncelemede
- Amaç: Gentelella v4 HTML tasarımını, CDN veya frontend build zinciri olmadan, erişilebilir public MVC layout/partial'larına dönüştürmek; Setup ve Login'in görünür ekranlarını oluşturmak.
- Kapsam: Version-pinned Gentelella üretim CSS/JS/font/ikon varlıkları; SHA-256 ve lisans/upstream bilgili asset manifest; `wwwroot/vendor/gentelella`; public authentication layout; strongly typed Setup/Login ViewModel'ları; ekranların UI doğrulaması ve HTTP güvenlik uyarısı alanı.
- Kapsam dışı: Gentelella demo sayfaları/mock kullanıcılar, CDN, Node/Vite/PWA/service worker, Razor Pages/`Pages/`/`.cshtml.cs`, Blazor/`.razor` bileşenleri veya runtime'ı, dashboard/sidebar, gerçek setup veya login POST iş kuralları.
- Bağımlılıklar: SX-001.
- Birincil uzman agent: `web_server_specialist`.
- Destek agent'ları: `web_security_specialist`.
- Implementasyon agent'ı: senior_fullstack_dotnet_developer.
- Etkilenecek projeler/dosyalar: `src/SinX.Web/Controllers/`, `src/SinX.Web/ViewModels/Setup/`, `src/SinX.Web/ViewModels/Account/`, `src/SinX.Web/Views/Setup/`, `src/SinX.Web/Views/Account/`, `src/SinX.Web/Views/Shared/`, `src/SinX.Web/wwwroot/vendor/gentelella/`, asset manifest/lisans bildirimi ve `src/tests/SinX.Web.Tests/`.
- Güvenlik ve tenant etkisi: Public layout yönetici veya domain navigasyonu içermez. Formlar yalnız render edilir; parola değerleri HTML dışındaki hiçbir yüzeye taşınmaz.
- Migration/rollback etkisi: Migration yoktur. Asset/layout değişiklikleri sürüm kontrolüyle geri alınabilir.

Geliştirici uygulama paketi:

- Kullanıcı değeri ve beklenen davranış: İlk açılışta sade Gentelella temalı Setup ekranı; kurulumdan sonra aynı görsel dilde yönetici Login ekranı görünür. Ekranlar mobil/klavye erişilebilir, Türkçe ürün metinleriyle anlaşılırdır.
- Ön koşullar: SX-001 Web hostu ve MVC; Gentelella v4 dağıtımı resmî upstream kaynaktan edinilir, yalnız kullanılan üretim varlıkları seçilir.
- İş kuralları: Setup formu yalnız `Kullanıcı adı`, `Parola`, `Parola tekrarı` ve `Kurulumu Tamamla` alanlarını gösterir; bileşen seçimi ya da provider ayarı içermez. Login formu yalnız kullanıcı adı/parola ve giriş aksiyonunu içerir. Parola tekrarı kimlik bilgisi değildir.
- Ekran ve kullanıcı akışı: `/setup` ilk kurulum durumunda public layout ile açılır; `/login` yönetici giriş formunu açar. HTTP kullanılıyorsa iki ekranda credential/session trafiğinin şifrelenmediği görünür ama korkutmayan uyarı gösterilir; HTTPS'te uyarı gizlenir veya bilgi durumuna dönüşür. Post işlem sonucu davranışları SX-003/SX-004 belirler.
- Model ve contract gereksinimleri: `SetupViewModel` kullanıcı adı/parola/parola tekrarı; `LoginViewModel` kullanıcı adı/parola ve yalnız local doğrulanmış return URL taşıyabilir. DataAnnotations UX için kullanılır; server-side validation sonraki iş kalemlerinde zorunludur. Entity, IPC veya API contract yoktur.
- Provider/platform teknik sözleşmesi: Varlıklar yalnız `wwwroot/vendor/gentelella` altında yerel sunulur; haricî font, ikon, script veya CDN isteği oluşturulmaz. Version, upstream URL, MIT lisans bildirimi ve her dağıtılan dosyanın SHA-256 değeri manifestte tutulur.
- Hata ve edge-case davranışı: Sayfalar script olmadan okunabilir form/label yapısını korur. İstemci doğrulaması yardımcıdır; ekran, hesap varlığı/SID/parola ayrıntısı göstermez. Demo içerik, tema örnek sayfaları ve kullanılmayan plugin'ler pakete alınmaz.
- Authorization, tenant, secret, audit ve idempotency gereksinimleri: Anonim public sayfada tenant veya domain ID alınmaz. `autocomplete` ve parola alanları bilinçli değerlendirilir; password/confirm hiçbir log, TempData, query string, validation summary veya HTML değer özniteliğine yazılmaz. Bu salt görünüm kalemi audit üretmez.
- Dosya/proje sahipliği ve sınırları: Gentelella class'ları yalnız MVC `Views/*.cshtml` ve `wwwroot` içindedir. View service resolve, veri erişimi veya authorization kararı yapmaz; controller yalnız input/response mapping yapar ve layout genel yönetim layout'una dönüşmez.
- Migration ve rollback/compensation: DB/migration yoktur. Manifestte doğrulanmış varlıklar ve licence dosyaları birlikte geri alınır.
- Açık varsayımlar ve bloke konular: Gentelella v4'ün belirli patch sürümü, upstream ref'i ve seçili asset listesi uygulama sırasında manifestte sabitlenir; kullanıcı tasarım örneği vermediği için tema v4'ün public authentication page düzeni temel alınır.
- Yararlanılan uzman çıktıları: Web teknik keşfi; teknoloji-stack Gentelella/asset/lisans kuralları; güvenlik sözleşmesinin UI redaction ve HTTP uyarısı bölümü.

Kabul kriterleri:

1. `/setup` ve `/login` MVC views, Gentelella v4 yerel CSS/JS/fontlarını kullanarak render edilir; ağ kaydında haricî asset/CDN çağrısı yoktur.
2. Setup'ta yalnız tanımlı üç alan ve aksiyon, Login'de yalnız giriş alanları/aksiyonu bulunur; bileşen seçimi veya demo data yoktur.
3. Her input görünür label, hata alanı, keyboard focus sırası ve WCAG 2.2 AA hedefiyle uyumlu kontrast/semantik içerir.
4. Parola alanları render sonrası boş kalır; sayfa kaynak, validasyon özeti ve URL secret içermez.
5. Gentelella sürümü, upstream URL'si, SHA-256 değerleri ve MIT lisans bildirimi sürüm kontrolünde bulunur.

Doğrulama:

- Build: `dotnet build`.
- Test: WebApplicationFactory HTML smoke testleri; asset manifest doğrulama testi.
- Windows VM testi: Gerekmez; Windows-host testi SX-001 kapsamındaki genel host doğrulamasına bağlıdır.
- Manuel kontrol: Chrome/Edge'de responsive ve klavye gezinimi; HTTP/HTTPS uyarı koşulları; ağ sekmesinde dış istek olmaması.

Kararlar ve kanıtlar:

- 2026-10-27: Kullanıcı MVC-only kararını verdi. Önceki yalnız placeholder Razor Pages dosyaları, doğrulanmış kullanılmamış agent üretimi oldukları için dar kapsamda kaldırıldı ve MVC Controller/ViewModel/View yapısına dönüştürüldü.
- 2026-10-27: `/setup` ve `/login` anonim MVC GET route'ları, strongly typed render-only modeller, anti-forgery token, erişilebilir Türkçe form/hata alanları, local-only `returnUrl` ve HTTP credential uyarısı ile eklendi. Gerçek POST/authentication/Windows/SQLite/Worker davranışı eklenmedi.
- 2026-10-27: Gentelella `4.2.0` resmî npm dağıtımından yalnız kullanılan CSS ve MIT licence alındı; upstream, paket ve dosya SHA-256 değerleri asset manifestte kaydedildi. CDN/Node/Vite/PWA/demo varlığı eklenmedi.
- 2026-10-27: `git diff --check`, XML parse, manifest hash, haricî asset, Razor Pages/Blazor artefaktı ve host statik kontrolleri geçti. Build/test kanıtı bloke: ortam yalnız .NET SDK 9.0.121 içeriyor, repository `global.json` ile 10.0.403 istiyor.

### SX-005 — Setup ve Login için çoklu dil altyapısı

- Durum: İncelemede
- Amaç: Public MVC ekranları ve Web doğrulama mesajlarını kaynak tabanlı, route bağımsız `tr-TR`/`en-US` kullanıcı deneyimine dönüştürmek.
- Kapsam: `IStringLocalizer`/`.resx` yapılandırması; `tr-TR` varsayılan/fallback; `en-US` destek kültürü; güvenli culture cookie provider; public layout dil seçici; Setup/Login görünür metinleri, alan label'ları, DataAnnotations/implicit-required doğrulama mesajları, validation summary, HTTP/lockout/setup-disabled uyarıları ve Web validation mapping metinleri; MVC/WebApplicationFactory doğrulaması; ilgili teknoloji/davranış dokümanı.
- Kapsam dışı: Route'da culture segmenti, tenant/domain kültürü, veritabanında kullanıcı kültürü kalıcılığı, domain/application error code değişimi, çeviri yönetim aracı veya üçüncü kültür.
- Bağımlılıklar: SX-002 MVC public layout ve view'ları. SX-003/SX-004, localize edilmiş Web hata mapping'ini kullanır.
- Birincil uzman agent: `senior_fullstack_dotnet_developer`.
- Destek agent'ları: `web_security_specialist`.
- Implementasyon agent'ı: senior_fullstack_dotnet_developer.
- Etkilenecek projeler/dosyalar: `SinX.Web/Program.cs`, `Controllers/`, `ViewModels/`, `Views/`, Web resource `.resx` dosyaları, culture cookie provider/configuration, `tests/SinX.Web.Tests/`, `docs/technology-stack.md`, `docs/application-documentation.md`.
- Güvenlik ve tenant etkisi: Culture değeri yalnız allowlisted `tr-TR`/`en-US` kabul eder; header/query/route/tenant kimliği authorization veya error-code kararını etkilemez.
- Migration/rollback etkisi: Migration yoktur. Resource/cookie provider değişimi kaynak sürümü geri alınarak geri çevrilir; cookie okunamazsa fallback uygulanır.

Geliştirici uygulama paketi:

- Kullanıcı değeri ve beklenen davranış: Kullanıcı Setup/Login ekranında Türkçe veya İngilizce metni seçer; seçim aynı tarayıcıdaki sonraki public isteklere uygulanır. Seçim yok/bozuk/geçersizse Türkçe görünür.
- Ön koşullar: MVC-only, localhost:8080 ve Gentelella local asset sınırları korunur.
- İş kuralları: Görünen label, heading, button, DataAnnotations/implicit-required validation, validation summary, generic failure, HTTP/lockout/setup-disabled risk metinleri resource key ile üretilir. Domain/Application `Result` error code'ları çevrilmez/değişmez; Web controller/mapping, bu code'u görünür localize message'a eşler.
- Ekran ve kullanıcı akışı: Public layout, form gönderimini değiştirmeyen erişilebilir dil seçici içerir. Seçici yalnız allowlisted culture değeri POST eder; başarılı seçim local-only culture cookie kurar ve güvenli local return URL'ye geri döner.
- Model ve contract gereksinimleri: Culture request/view modeli yalnız Web'dedir. Web ViewModel property `Display` ve validation message kaynakları resource tabanlıdır; domain modeline UI localization taşınmaz. Web-to-Worker/API/Domain contract'ına culture veya localized string eklenmez.
- Provider/platform teknik sözleşmesi: ASP.NET Core localization middleware ve `IStringLocalizer` kullanılır; resource dosyaları Web assembly'sinde sürüm kontrolündedir. External translation service/CDN kullanılmaz.
- Hata ve edge-case davranışı: Bilinmeyen culture, malformed cookie ve bulunmayan resource key exception üretmez; fallback `tr-TR` kullanılır. Secret, exception ve audit çıktısı yerelleştirilmez ve redacted kalır. Cookie tüm sensitive flags değerlendirilerek yalnız culture değerini taşır; credential/session cookie ile karıştırılmaz.
- Authorization, tenant, secret, audit ve idempotency gereksinimleri: Culture değişimi state-changing iş veya audit olayı değildir; anti-forgery gerekir mi uygulamadaki endpoint türüyle değerlendirilir, fakat hiçbir secret/tenant/domain ID kabul edilmez. Localized message raw Windows/provider hatası sızdırmaz.
- Dosya/proje sahipliği ve sınırları: Localization yalnız `SinX.Web` presentation/mapping katmanında kalır; controller ince kalır, ViewModel/Views business rule içermez.
- Migration ve rollback/compensation: DB değişikliği yoktur. Resource fallback bütünlüğü test edilir; rollback önceki resource set ve cookie name/value compatibility'sini korur.
- Açık varsayımlar ve bloke konular: Kullanıcının “gerek” ifadesi başlangıçta iki dilin uygulanması olarak yorumlanmıştır. Varsayılan/fallback `tr-TR`; daha fazla kültür ayrı SX kalemidir.
- Yararlanılan uzman çıktıları: Kullanıcı kapsam genişletmesi; Web security generic-error/redaction kuralları; MVC UI paketi.

Kabul kriterleri:

1. Setup, Login, public layout, alan label'ları, DataAnnotations/implicit-required validation, validation summary, HTTP/lockout/setup-disabled uyarıları ve Web validation metinleri `tr-TR` ve `en-US` resource kaynaklarından render edilir.
2. Dil seçici route-independent çalışır; yalnız `tr-TR`/`en-US` kabul eder; geçersiz/eksik cookie `tr-TR` fallback verir.
3. Domain/Application error code'ları sabit kalır; localize edilmiş görünür mesaj Web mapping katmanından gelir.
4. Culture seçimi authorization, tenant scope, credential veya returnUrl güvenlik kararını değiştirmez.
5. MVC/WebApplicationFactory testleri iki kültürü, fallback'i ve raw error/redaction korumasını doğrular.

Doğrulama:

- Build: `dotnet build` (warning'siz, .NET 10 ortamında).
- Test: WebApplicationFactory culture cookie/response/fallback ve localized validation mapping testleri.
- Windows VM testi: Gerekmez.
- Manuel kontrol: localhost:8080 Setup/Login'de seçici, Türkçe/İngilizce görünür metinler ve geri dönüş davranışı.

Kararlar ve kanıtlar:

- 2026-10-27: Kullanıcı kararıyla kalem eklendi; başlangıç uygulaması `tr-TR` ve `en-US` ile sınırlıdır.
- 2026-10-27: Web localization middleware yalnız culture-cookie provider ile `tr-TR`/`en-US` allowlist'i, `tr-TR` default/fallback'i ve route-independent local return URL ile `/culture` POST akışını uygular. Public layout, Setup/Login ve HTTP uyarısı resource tabanlıdır.
- 2026-10-27: `git diff --check`, resource XML parse ve MVC/Razor Pages/Blazor/external asset/57500 statik kontrolleri geçti. Build/test kanıtı .NET SDK 10.0.403 yokluğu nedeniyle bekliyor.
- 2026-10-27: MVC DataAnnotations localization `SharedResource` üzerinden bağlandı; Setup/Login ViewModel alan adları, required/compare hataları ve validation summary `tr-TR`/`en-US` resource kaynaklarına taşındı. Domain/application/audit/secret katmanına localized metin eklenmedi.

### SX-003 — İlk kurulum durumu ve yönetici hesabı oluşturma akışı

- Durum: Çalışıyor
- Amaç: Setup formunu ürün dokümanındaki tek seferlik, güvenli Windows yönetici hesabı ve Worker yapılandırma akışına bağlamak.
- Kapsam: Setup state sorgusu; anti-forgery korumalı use case; Windows kullanıcı adı doğrulaması; local hesabın oluşturulması, Administrators üyeliği, service-logon hakkı ve Worker service yapılandırması için typed port/Worker operasyonu; SID sabitleme; tamamlanma yönlendirmesi; audit ve compensation.
- Kapsam dışı: IIS/DNS/provider kurulumu, domain hesabı, built-in Administrator kullanımı, bileşen seçim sihirbazı, Plesk.
- Bağımlılıklar: SX-001, SX-002; Worker/Windows adapter temelinin bu kalemle aynı onaylı uygulama diliminde hazırlanması.
- Birincil uzman agent: `windows_server_engineer`.
- Destek agent'ları: `web_security_specialist`, `automation_api_specialist`.
- Implementasyon agent'ı: senior_fullstack_dotnet_developer.
- Etkilenecek projeler/dosyalar: `SinX.Application/Features/Setup/`, `SinX.Contracts/`, `SinX.Infrastructure` (setup state/audit/secret-safe persistence), `SinX.Windows/`, `SinX.Worker/`, `SinX.Web/Controllers/SetupController.cs`, `SinX.Web/ViewModels/Setup/`, `SinX.Web/Views/Setup/` ve uygun test projeleri.
- Güvenlik ve tenant etkisi: Yalnız sunucu kapsamı; domain/tenant bilgisi yoktur. Hesap parolası yalnız işlem ömründe tutulur; oluşturulan SID tek kabul edilen yönetici kimliğidir.
- Migration/rollback etkisi: Setup state ve sabit SID için SQLite migration gerektirir. Başarısız işlemde completed state yazılmaz; oluşturulan hesap/service ayarları için idempotent compensation planı gerekir.

Geliştirici uygulama paketi:

- Kullanıcı değeri ve beklenen davranış: İlk açılışta kullanıcı tek bir yerel yönetim hesabı oluşturur; başarılı ve doğrulanmış işlemden sonra tekrar Setup'a dönemez, Login'e yönlendirilir.
- Ön koşullar: SQLite migration altyapısı, `SinX.Worker` service ve ACL-korumalı typed Web-to-Worker sınırı bu use-case'i destekleyecek kadar kuruludur. Installer yükseltilmiş yönetici yetkisi şartını sağlayacak şekilde planlanır; gerçek production installer bu kalemin dışında tamamlanmış sayılmaz.
- İş kuralları: Windows hesap adının uygunluğu ve benzersizliği doğrulanır; local account oluşturulur, yerel Administrators'a eklenir, gerekli Log on as a service hakkı verilir, Worker bu hesapla yapılandırılıp başlatılır/doğrulanır, değişmez SID kaydedilir ve ancak tüm aşamalar başarılıysa setup state `Completed` olur. Built-in Administrator kopyalanmaz/kullanılmaz. Bileşen seçimi gösterilmez.
- Ekran ve kullanıcı akışı: Setup MVC GET action'ı yalnız `NotCompleted` state'de açılır. `Completed` state `/login`e güvenli yönlenir. State belirsiz/bozuksa fail-closed, ayrıntı/sır göstermeyen genel hata görülür. POST action'ı alan hatalarını ModelState'e mapler ve başarılı akışta parola alanlarını yeniden göstermez.
- Model ve contract gereksinimleri: Web InputModel Application command'a maplenir; Windows operation yalnız typed request/response ile Contracts/Worker sınırını geçer. Command/result ve error code feature'a özeldir; ham password, script, SQL, serbest path veya Windows hata çıktısı contract/log/audit içine girmez. SID, opaque yönetici kimliği olarak persistent state'te tutulur.
- Provider/platform teknik sözleşmesi: Web Windows API çağırmaz. Worker allowlist edilmiş kullanıcı/service operasyonunu yürütür; actor/scope/operation/correlation bilgilerini yeniden doğrular. Windows adapter `LogonUserW` dışındaki gerekli yerel hesap/service API'lerini typed port arkasında uygular.
- Hata ve edge-case davranışı: Aynı username varsa, parola uyuşmazsa veya Windows policy reddederse typed/alan-genel hatası verilir; hesap varlığı/Windows ayrıntısı dışarı sızmaz. Kısmi başarıda compensation denenir ve tamamlanmamış state korunur; compensation sonucu auditlenir. Eşzamanlı setup POST'ları idempotency/resource lock ile tek aktif akışa indirgenir.
- Authorization, tenant, secret, audit ve idempotency gereksinimleri: POST anti-forgery zorunludur. Parola log/audit/exception/TempData/redirect/model value'da bulunmaz. Setup request'i server scope'tur; actor yoksa açık bootstrap policy, tamamlandıktan sonra kapalı policy uygulanır. Başarı, başarısızlık ve compensation güvenli özetle auditlenir; idempotency anahtarı/işlem kilidi değerlendirilir.
- Dosya/proje sahipliği ve sınırları: UI yalnız Web; use case Application; state/audit/EF mapping Infrastructure; Windows çağrısı SinX.Windows; ayrıcalıklı çalışma SinX.Worker; IPC contract SinX.Contracts. Domain'e framework türü veya UI modeli eklenmez.
- Migration ve rollback/compensation: Yeni setup state/SID persistence migration'ı ileri/geri uyumlulukla gözden geçirilir. Hesap/service yaratımı dağıtık transaction değildir: adım kayıtları, doğrulama, mümkünse ters işlem, değilse operatöre güvenli recovery bilgisi gerekir.
- Açık varsayımlar ve bloke konular: Worker/service installation ve service identity'nin installer tarafından nasıl ilk kez sağlanacağı SX-003'ün uygulanabilir ön koşuludur. Bu kalem bu mekanizmayı uygulama diliminde kurar; WiX paketleme/upgrade senaryosu ayrı sonraki plan kalemi olarak görünür kalır.
- Yararlanılan uzman çıktıları: Güvenlik teknik sözleşmesi; uygulama dokümanı 4. bölüm; mimari Web/Worker/contract sınırları.

Kabul kriterleri:

1. Tamamlanmamış state'te `/setup` erişilebilir; tamamlanmış state'te hesap yeniden oluşturulmadan `/login`e yönlenir.
2. Setup formu anti-forgery ve server-side validation kullanır; password/confirm hiçbir log, audit, hata ya da response içinde görünmez.
3. Başarılı akış yalnız yeni, doğrulanmış yerel yönetici hesabının SID'sini kaydeder; Worker sağlıklı değilse setup tamamlanmış işaretlenmez.
4. Başarısız/kısmi akış güvenli compensation veya açık recovery kaydı üretir; tekrar deneme çift hesap/çift servis state'i yaratmaz.
5. SQLite migration, application/infrastructure testleri ve Windows VM olumlu/olumsuz senaryoları geçer.

Doğrulama:

- Build: Solution build, architecture testleri.
- Test: Setup state/use-case unit; SQLite migration integration; anti-forgery ve redaction WebApplicationFactory testleri.
- Windows VM testi: Local account, Administrators üyeliği, service-logon hakkı, Worker configuration/start/health, SID persistence, failed-step compensation.
- Manuel kontrol: İlk kurulum yalnız üç alanı gösterir; başarılı işlemden sonra `/setup` tekrar açılmaz.

Kararlar ve kanıtlar:

- 2026-10-27: Application Setup handler/ports, versioned opaque-secret worker request, Worker allowlist handler, Windows platform port and Windows VM scenario eklendi.
- 2026-10-27: Infrastructure EF Core SQLite `SetupDbContext` ile completed/in-progress/SID state, TTL'li Data Protection ile korunan transient secret reference ve secret içermeyen audit kaydı eklendi; DI'da `TimeProvider`, Data Protection ve persistence store'ları bağlandı.
- 2026-10-27: MVC Setup GET completed state'te `/login`e yönlenir; POST anti-forgery/validation ile parolayı yalnız transient reference'a çevirir, secret echo etmez ve generic/localized hata döndürür. Worker gateway metadata/reference doğrular ve Worker bağlı olmadığında `setup.worker.unavailable` ile fail-closed davranır.
- 2026-10-27: Kalem kabul edilmedi: gerçek Worker dispatcher/IPC, Windows local-account/group/service-right/service-health provisioning ve başarı/compensation Windows VM kanıtı henüz yoktur. Build/test de .NET SDK 10.0.403 eksikliği nedeniyle yürütülemedi.
- 2026-10-27: Windows ve IPC uzman sözleşmeleriyle tamamlayıcı paket doğrulandı: ACL-korumalı versioned named-pipe dispatcher; operation/actor/server-scope/correlation/idempotency doğrulaması; TTL/tek-kullanımlı opaque secret reference; yalnız operation-owned kaynaklar için ters-sıra compensation; Windows VM kabul senaryoları.
- 2026-10-27: [ADR-0002](docs/adr/0002-local-worker-ipc-and-transient-secret-vault.md) ile gRPC-over-Windows-Named-Pipes ve Web-create/Worker-consume ACL-korumalı transient vault kararı kaydedildi. IPC/secret mimari blokajı uygulama öncesi çözüldü; dependency ve kod ekleme ayrı uygulama işidir.

### SX-004 — Yönetici Login, cookie oturumu ve güvenli çıkış

- Durum: Çalışıyor
- Amaç: Kurulumda sabitlenen Windows SID'li yönetici hesabı için güvenli `/login` oturumu, logout ve giriş olaylarının audit akışını sağlamak.
- Kapsam: LogonUserW tabanlı doğrulama portu; SID eşleştirmesi; cookie scheme/policy; anti-forgery; rate limit/lockout; local-only return URL; logout; audit; generic hata davranışı.
- Kapsam dışı: Henüz yayımlanmayan domain paneli, domain kullanıcı parolası/hash'i, parola değiştirme ekranı, MFA/SSO, dashboard özellikleri.
- Bağımlılıklar: SX-001, SX-002, SX-003 ve setup state/SID persistence.
- Birincil uzman agent: `web_security_specialist`.
- Destek agent'ları: `windows_server_engineer`, `monitoring_logging_specialist`.
- Implementasyon agent'ı: senior_fullstack_dotnet_developer.
- Etkilenecek projeler/dosyalar: `SinX.Application/Features/Authentication/`, `SinX.Infrastructure/Identity/` ve audit alanı, `SinX.Windows/`, `SinX.Web/Controllers/AccountController.cs`, `SinX.Web/ViewModels/Account/`, `SinX.Web/Views/Account/`, Web authorization/composition, uygun test projeleri.
- Güvenlik ve tenant etkisi: Bu yalnız `localhost:8080` başlangıç yönetici authentication sınırıdır. Domain scheme/policy henüz yayımlanmaz; tenant context veya domainId kabul etmez.
- Migration/rollback etkisi: Rate-limit/lockout/audit persistence tasarımı kalıcı state gerektirirse migration eklenir. Cookie/route değişikliği deployment geri alınarak geri çevrilebilir; secret saklanmaz.

Geliştirici uygulama paketi:

- Kullanıcı değeri ve beklenen davranış: Kullanıcı, Setup'ta oluşturduğu hesapla giriş yapar ve süreli yönetici oturumu elde eder; başka local Administrator, built-in Administrator veya domain hesabı giriş yapamaz.
- Ön koşullar: SX-003 doğrulanmış setup state ve sabit SID; Windows VM test altyapısı.
- İş kuralları: Credential `LogonUserW` üzerinden geçici olarak doğrulanır, kabul ancak sonuç SID'si persisted setup SID'si ile eşleşirse verilir. Başarılı giriş session fixation'a dirençli yeni cookie/session üretir. Başarısızlık için tek generic mesaj kullanılır. Logout oturumu sonlandırır.
- Ekran ve kullanıcı akışı: `/login` anonime açıktır; başarılı local `returnUrl` varsa yalnız aynı host içi güvenli hedefe, yoksa sonraki yetkili başlangıç sayfasına yönlenir. Dış/schemeli returnUrl reddedilir. Kilitli kullanıcıya hesap ayrıntısı sızdırmayan bekleme mesajı verilir.
- Model ve contract gereksinimleri: Login InputModel kullanıcı adı/parola/yerel returnUrl taşır. Application sonucu kararlı error code ile gelir; Windows adapter credential/SID ayrıntılarını UI'ya vermez. Cookie claim'i yalnız gerekli yönetici actor/SID bilgisini içerir; password ya da domainId taşımaz.
- Provider/platform teknik sözleşmesi: Web credential doğrulamasını typed port üzerinden ister; Web ve Worker yetki sınırı korunur. Cookie `HttpOnly`, `SameSite=Strict`, HTTPS'te `Secure`; kısa idle timeout ayarlanır. HTTPS yoksa UI uyarısı SX-002 ile tutarlı kalır.
- Hata ve edge-case davranışı: Setup tamamlanmadıysa Login hesap kabul etmez ve Setup'a güvenli yönlendirir. Lockout/rate-limit, SID mismatch, disabled/unknown account ve yanlış password aynı ifşa etmeyen UI sonucunu üretir. Audit/loglarda credential, token, raw Windows error, SID gereğinden fazla ayrıntı bulunmaz.
- Authorization, tenant, secret, audit ve idempotency gereksinimleri: Login/logout POST anti-forgery ile korunur; rate limit uygulama katmanındadır ve Windows account lockout'unu tetiklemeyecek şekilde sınırlandırılır. Success, failure, lockout, logout ve session-end auditlenir; actor/SID, IP, session ve güvenli sonuç özeti kaydedilir. Domain endpoint/policy ile ortak cookie veya yetki kabulü yapılmaz.
- Dosya/proje sahipliği ve sınırları: Controller yalnız input/response mapping yapar; auth use-case Application'da; native Windows doğrulaması SinX.Windows'ta; cookie/policy Web composition'da; persistence/audit Infrastructure'dadır.
- Migration ve rollback/compensation: Lockout/audit state şeması gerekiyorsa migration review edilir. Uygulama geri alınırsa eski cookie scheme güvenli biçimde expire edilir; parola veya credential için compensation/saklama yoktur.
- Açık varsayımlar ve bloke konular: Dokümanlar lockout eşik ve süresini sayısal belirtmez. Uygulama paketi bunların Options ile doğrulanan, güvenli varsayılanlar olarak tanımlanmasını gerektirir; değerler ürün kararını değiştiriyorsa geliştirici başlamadan Product Owner kullanıcı onayı ister.
- Yararlanılan uzman çıktıları: Güvenlik teknik sözleşmesi; uygulama dokümanı 5. bölüm; teknoloji-stack authentication/session kuralları.

Kabul kriterleri:

1. Yalnız persisted setup SID'sine eşleşen yerel Windows hesabı giriş yapabilir; diğer Administrator, built-in Administrator ve domain hesabı reddedilir.
2. Login/logout state-changing POST'ları anti-forgery kullanır; returnUrl yalnız local hedef kabul eder.
3. Cookie `HttpOnly`/`SameSite=Strict`, HTTPS'te `Secure`, kısa idle timeout ve başarıda yenilenmiş oturumla çalışır.
4. Rate-limit/lockout Windows hesabını sınırsız denemeyle kilitlemez; kullanıcıya hesap/parola/SID ayrıntısı sızdırmayan generic hata verir.
5. Başarılı/başarısız giriş, lockout, logout ve session-end secret içermeden auditlenir; HTTP kullanım uyarısı görünürdür.
6. WebApplicationFactory ve Playwright pozitif/negatif senaryoları, Windows VM LogonUserW/SID senaryoları geçer.

Doğrulama:

- Build: Solution build, warning'siz.
- Test: SID allow/deny, anti-forgery negative, local returnUrl, cookie flag/timeout, rate-limit/lockout, audit redaction Web/Application testleri.
- Windows VM testi: LogonUserW ile doğru SID kabulü; yanlış parola, farklı admin, built-in Administrator ve domain hesabı retleri.
- Manuel kontrol: Login formu, HTTP uyarısı, logout ve timeout sonrası tekrar-login davranışı.

Kararlar ve kanıtlar:

- 2026-10-27: Güvenlik sözleşmesi doğrulandı: localhost:8080 başlangıç yönetici sınırı, persisted SID allow-match, CSRF/local returnUrl, cookie flags/session renewal/logout, 5/15m rate-limit + 15m lockout + 20m idle options ve redacted audit pakete eklendi. Worker/Windows doğrulayıcı hazır değilse login fail-closed kalır.
- 2026-10-27: ADR-0002, Login ön koşulu olan Worker transport ve transient secret identity/ACL modelini karara bağladı; native uygulama ve Windows VM doğrulaması henüz tamamlanmadı.

