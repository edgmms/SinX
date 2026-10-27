# ADR-0002: Yerel Worker IPC ve geçici secret vault

## Bağlam

SinX.Web düşük yetkili, SinX.Worker ise farklı bir Windows service identity ile ayrıcalıklı çalışır. Setup ve Login akışları Windows hesabı oluşturma/doğrulama gerektirir. Ham credential, komut veya yol Web-to-Worker sözleşmesine, loga, audit kaydına ya da uygulama veritabanına giremez.

## Karar

- Web-to-Worker iletişimi, yalnız yerel Windows named pipe üzerinde versioned gRPC ile yapılacaktır. Worker TCP/HTTP listener açmaz.
- Pipe ACL yalnız Web ve Worker service identity'lerine izin verir; Interactive, Everyone ve Network erişimi reddedilir.
- Dispatcher yalnız sabit allowlist operation türlerini kabul eder. Her istek contract version, operation/idempotency ID, actor, server scope/target, correlation ID ve yalnız opaque secret reference taşır.
- Setup parolası kısa ömürlü uygulama-özel vault girdisidir. Web create, Worker claim-and-consume/delete yetkisine sahiptir; key-store ve vault dosyalarının ACL'leri bu ayrımı uygular. Reference rastgele, operation/scope/purpose/TTL ile bağlıdır ve tek kullanımlıdır.
- Setup state atomik `NotStarted → InProgress → Completed|Failed` ilerler. Completed yalnız Worker'ın account/service health doğrulamasından sonra accepted SID ile yazılır.

## Alternatifler

- BCL `NamedPipeServerStream` ile özel binary protokol seçilmedi: framing, versioning, serializer, timeout ve güvenlik sorumluluğunu ürün koduna taşır.
- LocalMachine DPAPI tek başına seçilmedi: farklı service identity'leri arasında yeterli least-privilege ayrımı sağlamaz.
- Parolanın IPC veya SQLite'a şifreli blob olarak konması seçilmedi: contract/persistence secret sınırını ihlal eder.

## Sonuçlar

Merkezi paket yönetimine açıkça sürümlenmiş gRPC named-pipe transport bağımlılıkları eklenecek; bu ADR öncesinde eklenmez. Worker, caller ACL'sini ve metadata'yı tekrar doğrular; aynı operation terminal sonucunu tekrar verir, paralel işlemi kilitler. Secret TTL sonunda, başarıda, hatada veya iptalde silinir. Worker restartında claimed fakat terminal olmayan işlem RecoveryRequired/Failed olur; secret yeniden kullanılamaz.

## Güvenlik ve rollback

Raw credential, crypto payload, Windows hata ayrıntısı ve provider output contract/audit/log/exception içinde bulunmaz. Audit yalnız actor, scope, operation, correlation ve güvenli sonuç kodu taşır. ACL veya service identity değişikliği öncesi snapshot alınır; health başarısızsa önceki yapılandırma geri yüklenir. Vault şeması/ACL geri alınmadan aktif references expire/iptal edilir. Yalnız operasyonun sahipliği kanıtlanmış account/group-right/service değişiklikleri compensation ile tersine çevrilir.

## Windows VM kabul kriterleri

1. Web identity pipe gRPC çağrısı yapar; yetkisiz local identity erişemez; Worker TCP dinlemez.
2. Yanlış version, allowlist dışı operation, scope/target uyumsuzluğu, TTL geçmiş veya tekrar kullanılan reference reddedilir.
3. Aynı operation ID çift kaynak üretmez; Worker restartı sonrası terminal/idempotency state korunur.
4. Web create, Worker one-time consume/delete yapar; yetkisiz identity vault içeriğini okuyamaz.
5. Account/service health başarısızsa Completed yazılmaz; sahip olunan kaynaklar compensation ile geri alınır.
6. IPC, SQLite, audit, log ve crash çıktılarında plaintext secret veya raw Windows output yoktur.
