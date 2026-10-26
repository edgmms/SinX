# SinX Agent Çalışma Standardı

Bu dosya repository'nin tamamı için bağlayıcıdır. SinX üzerinde kod, test, migration, script veya dokümantasyon değiştiren her coding agent aşağıdaki sırayla kaynakları okumalıdır:

1. `AGENTS.md`
2. `docs/project-architecture.md`
3. `docs/technology-stack.md`
4. `docs/application-documentation.md`
5. Ekran işi yapıyorsa `docs/screen-menu.md`
6. Değiştirilen klasörde daha yakın bir `AGENTS.md` varsa o dosya

Kullanıcı tarafından onaylanmış çok adımlı bir iş planı yürütülüyorsa ayrıca `PLANS.md` okunur ve durum/kanıt kayıtları güncel tutulur. Küçük, bağımsız işler için gereksiz plan dosyası güncellemesi yapılmaz.

## Rol ayrımı

- Kullanıcı iş planını öncelikle `product_owner` agent'ına verir. Product Owner işi `PLANS.md` içinde parçalara ayırır, ilgili uzmanlara ve implementasyon agent'ına dağıtır, kabul ve ilerleme takibini yapar.
- `.codex/agents` altındaki platform ve ürün uzmanları teknik gereksinim, risk, sözleşme, doğrulama ve rollback bilgisi sağlar; ürün kodu yazmaz.
- Product Owner uzman çıktısını geliştiriciye ham biçimde aktarmaz. Çıktıları ürün ve mimari kurallarıyla uzlaştırıp `PLANS.md` içinde geliştiricinin uygulayabileceği tek bir "Geliştirici uygulama paketi" haline getirir.
- Varsayılan coding agent `senior_fullstack_dotnet_developer` rolüdür. Coding agent, uzman çıktısını doğrudan kopyalamaz; bu repository'nin mimari sınırlarına dönüştürerek uygular.
- Uzman görüşü ile proje standardı çelişirse coding agent uygulamayı durdurur, çelişkiyi belgeler ve mimari karar ister.
- Plesk SinX'in bileşeni veya bağımlılığı değildir. Plesk'e özgü model, terim, API veya varsayım ürün koduna taşınmaz.

## Değişmez mimari kurallar

- Mimari: modüler monolit + Clean Architecture bağımlılık yönü + feature/vertical-slice organizasyonu.
- Bağımlılık yönü: `Web/Worker/Adapters/Infrastructure -> Application -> Domain`.
- `SinX.Domain` hiçbir framework, EF Core, ASP.NET Core, Windows API veya provider projesine referans vermez.
- İş kuralı controller, Razor view, EF configuration, PowerShell veya provider adapter içinde yazılmaz.
- MVC controller/PageModel yalnızca input mapping, authorization boundary, use-case çağrısı ve response mapping yapar.
- Domain entity hiçbir zaman view, API veya IPC modeli olarak dışarı verilmez.
- Web process yönetici işlemini doğrudan yapmaz; tip güvenli contract ile Worker'a job gönderir.
- Domain kapsamlı her sorgu ve komut server-side `domainId` authorization filtresinden geçer.
- Ham PowerShell, SQL, shell metni, serbest dosya yolu veya secret API/IPC contract'ına alınmaz.
- Uzun ve yan etkili operasyonlar kalıcı job olarak yürütülür; HTTP request içinde tamamlanmaya zorlanmaz.
- Yeni proje, framework, ORM abstraction, generic repository, event bus veya runtime plugin ancak ADR ile eklenebilir.

## Dosya yerleşimi hızlı haritası

| İçerik | Yer |
|---|---|
| Aggregate, entity, value object, invariant | `src/SinX.Domain/<Module>/` |
| Use case, command/query, handler, port interface | `src/SinX.Application/Features/<Module>/<UseCase>/` |
| API/IPC contract | `src/SinX.Contracts/` |
| SQLite, EF Core, Identity, repository implementasyonu | `src/SinX.Infrastructure/` |
| Windows işletim sistemi adapter'ı | `src/SinX.Windows/` |
| IIS/DNS/MailEnable/database provider adapter'ları | İlgili `src/SinX.*` provider projesi |
| Controller, PageModel, ViewModel, Razor view | `src/SinX.Web/` |
| Privileged job endpoint ve composition root | `src/SinX.Worker/` |
| Unit/integration/contract/VM/UI testleri | Karşılık gelen `tests/SinX.*Tests/` projesi |
| Mimari karar | `docs/adr/` |

## Kodlama ve teslim kuralları

- Nullable reference types açık kalır; uyarılar susturulmaz, kök neden düzeltilir.
- Async I/O metotları `CancellationToken` kabul eder ve `Async` son eki taşır.
- Zaman için doğrudan `DateTime.Now/UtcNow` yerine enjekte edilen `TimeProvider` kullanılır; persisted zamanlar UTC `DateTimeOffset` olur.
- Beklenen iş hataları typed `Result` ve kararlı error code ile döner; exception yalnızca beklenmeyen/teknik hata içindir.
- Secret değerler DTO, log, audit, exception veya view model içinde bulunmaz; yalnızca opaque secret reference taşınır.
- Her state-changing endpoint anti-forgery, authorization, idempotency ve audit ihtiyacı açısından değerlendirilir.
- Değişiklik en az bir uygun testle doğrulanır. Test edilemeyen Windows davranışı için VM test senaryosu ve doğrulama ölçütü yazılır.
- Yeni veya değişen davranış ilgili dokümanda ve gerekiyorsa ADR'de güncellenir.
- Tamamlanan işte build/test sonucu, değişen dosyalar, kalan riskler ve uygulanmayan doğrulamalar raporlanır.

