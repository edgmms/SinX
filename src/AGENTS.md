# SinX Source Agent Talimatları

Bu dosya `src/` altındaki bütün dosya ve klasörler için geçerlidir. Önce repository kökündeki `AGENTS.md` ve `docs/project-architecture.md` tamamen okunmalıdır.

## İzin verilen proje yerleşimi

Yeni kaynak kod yalnız aşağıdaki projelerden uygun olanına eklenir:

```text
src/
├── SinX.Domain/
├── SinX.Application/
├── SinX.Contracts/
├── SinX.Infrastructure/
├── SinX.Windows/
├── SinX.Iis/
├── SinX.Dns/
├── SinX.MailEnable/
├── SinX.Database.Abstractions/
├── SinX.Database.SqlServer/
├── SinX.Database.PostgreSql/
├── SinX.Database.MySql/
├── SinX.Database.MariaDb/
├── SinX.Web/
└── SinX.Worker/
```

Bu liste dışında proje oluşturmak veya mevcut proje sorumluluğunu değiştirmek ADR gerektirir.

## Kod yerleştirme kontrolü

Kod eklemeden önce şu sorular cevaplanır:

1. Bu bir değişmez iş kuralı mı? `SinX.Domain`.
2. Bu bir kullanıcı/use-case akışı mı? `SinX.Application/Features/<Module>/<UseCase>`.
3. Bu process dışına çıkan API/IPC modeli mi? `SinX.Contracts`.
4. Bu persistence, Identity, secret, audit veya job implementasyonu mu? `SinX.Infrastructure`.
5. Bu Windows ya da bir provider entegrasyonu mu? İlgili adapter projesi.
6. Bu controller, PageModel, ViewModel, Razor view veya statik UI varlığı mı? `SinX.Web`.
7. Bu ayrıcalıklı operation endpoint/job execution kodu mu? `SinX.Worker`.

Karar verilemiyorsa kod rastgele bir `Common`, `Helpers`, `Managers`, `Models` veya `Services` klasörüne konulmaz; mimari karar istenir.

## Zorunlu sınırlar

- Dependency yönü `Web/Worker/Adapters/Infrastructure -> Application -> Domain` şeklindedir.
- Domain katmanı framework ve infrastructure bağımlılığı alamaz.
- Application katmanı EF Core, ASP.NET Core, Windows/IIS/DNS/MailEnable SDK türü kullanamaz.
- Web doğrudan ayrıcalıklı Windows/provider operasyonu yürütemez.
- Domain entity; ViewModel, API response veya IPC contract olarak kullanılamaz.
- İş kuralı controller, Razor view, EF mapping, script veya adapter içine yazılamaz.
- Provider projeleri birbirine referans veremez.
- Plesk'e ait kod, model veya entegrasyon eklenemez.

## Her değişiklikte beklenenler

- Uygun test `tests/` altındaki karşılık gelen test projesine eklenir.
- Domain/tenant authorization için pozitif ve negatif senaryolar değerlendirilir.
- State-changing operasyon için audit, idempotency ve rollback/compensation tanımlanır.
- Secret, ham komut ve serbest path sözleşmelerden uzak tutulur.
- Public davranış değişiyorsa ilgili doküman veya ADR güncellenir.

