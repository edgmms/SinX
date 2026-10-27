# SX-003 Windows VM doğrulaması

Ön koşullar: Temiz snapshot'tan Windows Server, yükseltilmiş installer bağlamı ve `SinX.Worker` servis kaydı.

1. `http://localhost:8080/setup` üzerinden geçerli kullanıcı/parola/parola tekrarı gönderin.
2. Yerel hesabın oluşturulduğunu, yalnız gerekli `Administrators` ve `Log on as a service` yetkilerini aldığını doğrulayın.
3. Worker service identity yapılandırması, başlangıcı ve sağlık kontrolünün başarılı olduğunu doğrulayın.
4. Persisted state'in yalnız accepted SID içerdiğini; parola, raw Windows output veya secret reference içermediğini doğrulayın.
5. `/setup` yeniden açıldığında `/login`e yönlendirmeyi doğrulayın.
6. Worker health başarısızlığını enjekte edin; completed state yazılmadığını, secret'ın tüketildiğini ve güvenli recovery/audit kaydı oluştuğunu doğrulayın.
7. Eşzamanlı iki POST gönderin; yalnız bir operation'ın hesap/service state'i oluşturabildiğini doğrulayın.
# Setup ve Login Windows VM doğrulama senaryosu

Ön koşul: Windows Server test VM, yükseltilmiş installer, Web ve Worker için farklı service identity'ler ve yalnız bu identity'lere izin veren local named-pipe ACL'si.

1. Setup POST'unda Worker envelope'unun sürüm, operation (`setup.provision`), bootstrap actor, server scope, target ve correlation alanlarını kabul ettiğini doğrulayın; farklı operation/sürüm/actor reddedilmelidir.
2. Worker'ın yeni local hesabı oluşturduğunu, iyi bilinen Administrators SID grubuna üyelik verdiğini, LSA `Log on as a service` hakkını tanımladığını ve SCM ile Worker identity/start/health sonucunu doğruladığını kontrol edin.
3. Başarıdan önce veya health başarısızlığında setup state'in `Completed` olmadığını, yalnız operasyonun oluşturduğu kaynakların telafi edildiğini ve audit kaydının parola içermediğini doğrulayın.
4. Başarıda kalıcı SID'nin yeni hesabın SID'si olduğunu, setup tekrarının Login'e yönlendiğini ve transient secret reference'ın bir kez tüketilip TTL sonunda temizlendiğini doğrulayın.
5. `LogonUserW` ile doğru local hesabın kabulünü; yanlış parola, farklı Administrator, built-in Administrator ve domain hesabının persisted SID allow-match nedeniyle reddini doğrulayın.
6. Beş başarısız girişin 15 dakikalık pencerede 15 dakika lockout oluşturduğunu, cookie'nin HttpOnly/SameSite Strict olduğunu ve HTTPS'te Secure olduğunu; logout sonrası cookie'nin geçersiz olduğunu doğrulayın.
