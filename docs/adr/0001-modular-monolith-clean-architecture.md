# ADR-0001: Modüler Monolit ve Clean Architecture

> **Durum:** Kabul edildi  
> **Tarih:** 2026-10-27  
> **Karar sahibi:** `automation_api_specialist`  
> **Teknik koordinasyon:** `windows_server_engineer`, `web_security_specialist`

## Bağlam

SinX; ASP.NET Core MVC/Razor tabanlı web arayüzü ile ayrıcalıklı Windows yönetim operasyonlarını aynı ürün paketi içinde sunacaktır. Domain, IIS, DNS, mail ve farklı database provider'ları geliştikçe iş kurallarının UI, EF Core veya platform adapter'larına dağılması riski bulunmaktadır.

## Karar

- Ürün modüler monolit olarak geliştirilecektir.
- Runtime'da düşük yetkili `SinX.Web` ve ayrıcalıklı `SinX.Worker` process'leri bulunacaktır.
- Kaynak kod Clean Architecture bağımlılık yönünü kullanacaktır.
- Application kodu feature/vertical-slice düzeninde organize edilecektir.
- Domain, Application, Contracts, Infrastructure, platform/provider adapter'ları ve presentation ayrı projeler olacaktır.
- Ayrıntılı ve bağlayıcı kurallar `docs/project-architecture.md` içinde tutulacaktır.

## Alternatifler

### Tek ASP.NET Core projesi

Başlangıcı hızlıdır ancak ayrıcalıklı Windows kodu, UI, persistence ve iş kurallarının birbirine karışma riski yüksektir.

### Mikroservisler

Bağımsız dağıtım sağlayabilir fakat tek Windows Server üzerinde çalışan ilk ürün için deployment, güvenlik, gözlemlenebilirlik ve veri tutarlılığı maliyeti gereksizdir.

### Tam CQRS/event-sourcing

Audit ve job ihtiyaçlarına yardımcı olabilir fakat başlangıç kapsamı için ek framework ve operasyonel karmaşıklık getirir. Command/query ayrımı framework olmadan uygulanacaktır.

## Sonuçlar

Olumlu:

- İş kuralları Windows ve UI teknolojilerinden bağımsız test edilir.
- Provider implementasyonları birbirinden izole kalır.
- Web/Worker güven sınırı kaynak kodda görünür olur.
- Feature geliştiren agent dosyanın yerini deterministik biçimde bulur.

Olumsuz:

- Tek projeye göre daha fazla project reference ve mapping kodu oluşur.
- API/IPC/ViewModel/Domain modelleri arasında bilinçli dönüşüm gerekir.
- Architecture testlerinin ve dependency kurallarının sürekli korunması gerekir.

## Güvenlik etkisi

Web process doğrudan ayrıcalıklı platform API'si kullanamaz. Worker yalnız versionlanmış typed operation kabul eder. Domain scope ve authorization yalnız UI görünürlüğüne bırakılmaz; Application ve Worker sınırlarında yeniden doğrulanır.

## Geri dönüş koşulu

Bu karar ancak ölçülebilir bakım maliyeti oluşturduğu veya ürünün dağıtım/topoloji modeli temelden değiştiği kanıtlanırsa yeni bir ADR ile değiştirilir.

