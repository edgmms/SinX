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

Henüz kullanıcı iş planı aktarılmadı.

