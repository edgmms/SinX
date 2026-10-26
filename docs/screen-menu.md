# SinX Ekran ve Menü Dokümantasyonu

> **Doküman sahibi:** `domain_hosting_specialist`  
> **Teknik koordinasyon:** `server_operations_specialist`

Bu doküman SinX uygulamasının sol menüsünü, Domain Detayı navigasyonunu, alt ekranlarını ve sunucu/domain kapsam ayrımını tanımlar. Ürün mimarisi, kurulum, kimlik doğrulama, job ve audit kararları için [Uygulama Dokümantasyonu](application-documentation.md) esas alınır.

## 1. Sol menü: sunucu seviyesindeki özellikler

> **Birincil agent:** `server_operations_specialist`  
> **Destek agent'ları:** Aşağıdaki tabloda her modül için ayrıca belirtilmiştir.

### 1.1 Sol menü hiyerarşisi

> **Birincil agent:** `server_operations_specialist`  
> **Destek agent'ları:** `windows_server_engineer`, `domain_hosting_specialist`

```text
Dashboard

Domainler

Sunucu Yönetimi
├── Windows Rolleri
├── IIS Yönetimi
├── DNS Sunucusu
├── Windows Kullanıcıları
├── Servisler
├── Scheduled Tasks
├── Firewall
├── Sertifikalar
├── Depolama
└── Güncellemeler

Database Sistemleri
├── Provider'lar
├── SQL Server
├── PostgreSQL
├── MySQL
└── MariaDB

Mail Sunucusu
└── MailEnable

Operasyon
├── Backup Yönetimi
├── Monitoring
├── Loglar
├── Job Geçmişi
└── Audit Log

Ayarlar
```

`Domainler` menüsü sunucu seviyesindeki domain envanterine açılır. Domain seçildikten sonra domaine bağlı işlemler sol menüde çoğaltılmaz; Domain Detayı sekmelerinde gösterilir.

### 1.2 Domain oluşturma ekranı

> **Birincil agent:** `domain_hosting_specialist`  
> **Destek agent'ları:** `web_server_specialist`, `web_security_specialist`, `database_integration_specialist`, `file_deployment_specialist`

```text
Yeni Domain
├── Domain Adı
├── Domain Panel Hesabı
│   ├── Kullanıcı Adı
│   ├── Parola
│   └── Parola Tekrarı
├── Website
│   ├── IIS Site Adı (otomatik)
│   ├── Application Pool Adı (otomatik)
│   └── Pool Identity: ApplicationPoolIdentity (salt okunur)
├── Database (isteğe bağlı)
│   ├── Provider ve Instance
│   └── Database Oluştur
└── FTP Erişimi (isteğe bağlı)
    ├── FTP Kullanıcı Adı
    ├── FTP Parolası
    └── FTP Parola Tekrarı
```

- Domain panel hesabı `http://<domain>:8080` girişinde kullanılır ve yalnızca oluşturulan domainin detay menülerini açar.
- Application Pool için parola alanı gösterilmez; IIS sanal `ApplicationPoolIdentity` otomatik oluşturulur.
- SQL Server yerel provider ise pool kimliğine bağlı Windows login hazırlanır. Diğer provider'larda pool/domain binding'ine özel native database principal otomatik üretilir.
- FTP etkinleştirilirse domain panel hesabından ayrı non-admin Windows hesabı oluşturulur.
- IIS hazır değilse domain oluşturma tamamlanmaz; kullanıcı IIS kurulum/aktivasyon geçiş kapısına yönlendirilir.

### 1.3 Sol menü ekran sorumlulukları

> **Birincil agent:** `server_operations_specialist`  
> **Destek agent'ları:** Aşağıdaki tabloda her ekran için belirtilmiştir.

IIS, DNS Sunucusu, database provider'ları ve MailEnable menüleri bileşenin anlık durumunu bir badge ile gösterir: `Hazır`, `Durmuş`, `Kurulu Değil`, `Yapılandırma Gerekli`, `Onarım Gerekli` veya `Yeniden Başlatma Bekliyor`. Kullanıcı menüye girdiğinde normal yönetim ekranından önce gerekli bileşen geçiş kapısı çalışır.

| Menü | Kapsam | İçerik | Birincil agent | Destek agent'ları |
|---|---|---|---|---|
| Dashboard | Sunucu | CPU, RAM, disk, servis sağlığı, uyarılar, son job'lar ve audit özeti | `monitoring_logging_specialist` | `windows_server_engineer` |
| Domainler | Geçiş noktası | Domain listesi, arama, durum ve Domain Detayı'na geçiş | `domain_hosting_specialist` | `server_operations_specialist` |
| Windows Rolleri | Sunucu | IIS, DNS ve diğer desteklenen Windows rollerinin tespiti, kurulumu ve durumu | `windows_server_engineer` | `server_operations_specialist` |
| IIS Yönetimi | Sunucu | IIS servis durumu, global ayarlar, server-level limits, modüller ve varsayılan politikalar | `web_server_specialist` | `windows_server_engineer` |
| DNS Sunucusu | Sunucu | DNS servis durumu, forwarder, recursion, cache, zone transfer politikası ve server-wide ayarlar | `dns_specialist` | `windows_server_engineer` |
| Database Sistemleri | Sunucu | Provider kataloğu; SQL Server, PostgreSQL, MySQL ve MariaDB instance/service durumu, portlar ve global ayarlar | `database_integration_specialist` | `sql_server_engineer`, `postgresql_engineer` ve provider uzmanları |
| Mail Sunucusu | Sunucu | MailEnable servisleri, connector, queue, relay, anti-spam, antivirus ve global mail ayarları | `mailenable_specialist` | `mail_management_specialist`, `mail_security_specialist` |
| Windows Kullanıcıları | Sunucu | Yerel kullanıcılar, gruplar, durum ve grup üyelikleri | `windows_server_engineer` | `web_security_specialist` |
| Servisler | Sunucu | Windows servisleri, başlangıç tipi, durum ve kontrollü servis operasyonları | `windows_server_engineer` | `server_operations_specialist` |
| Scheduled Tasks | Sunucu | Windows Task Scheduler görevleri ve çalışma geçmişi | `server_operations_specialist` | `windows_server_engineer` |
| Firewall | Sunucu | Windows Firewall profilleri ve inbound/outbound kurallar | `web_security_specialist` | `windows_server_engineer` |
| Sertifikalar | Sunucu | Local Machine certificate store envanteri, süreler ve servis atamaları | `tls_certificate_specialist` | `windows_server_engineer` |
| Depolama | Sunucu | Diskler, volume'ler, boş alan, klasörler, paylaşımlar ve NTFS izinleri | `windows_server_engineer` | `server_operations_specialist` |
| Backup Yönetimi | Sunucu | Sunucu geneli backup hedefleri, zamanlama, retention ve restore job'ları | `backup_restore_specialist` | `remote_backup_storage_specialist` |
| Monitoring | Sunucu | Sistem metrikleri, servis sağlığı ve genel alarm kuralları | `monitoring_logging_specialist` | `windows_server_engineer` |
| Loglar | Sunucu | Windows Event Log, uygulama logları, servis logları ve merkezi arama | `monitoring_logging_specialist` | İlgili bileşenin uzman agent'ı |
| Güncellemeler | Sunucu | Windows Update, uygulama güncellemesi ve yeniden başlatma durumu | `server_operations_specialist` | `windows_server_engineer` |
| Job Geçmişi | Sunucu | Çalışan, tamamlanan, başarısız ve yeniden başlatma bekleyen operasyonlar | `automation_api_specialist` | `monitoring_logging_specialist` |
| Audit Log | Sunucu | Kullanıcı, zaman, hedef, önceki/yeni değer ve sonuç kayıtları | `monitoring_logging_specialist` | `web_security_specialist` |
| Ayarlar | Sunucu | Port, oturum, parola, HTTPS, retention ve uygulama tercihleri | `server_operations_specialist` | `web_security_specialist`, `tls_certificate_specialist` |

## 2. Domain Detayı: domaine bağlı özellikler

> **Birincil agent:** `domain_hosting_specialist`  
> **Destek agent'ları:** Aşağıdaki tabloda her sekme için ayrıca belirtilmiştir.

### 2.1 Domain ana navigasyonu

> **Birincil agent:** `domain_hosting_specialist`  
> **Destek agent'ı:** `server_operations_specialist`

Domain listesinde bir domain seçildiğinde aşağıdaki detay sayfası açılır:

```text
Domainler
  └── example.com
      ├── Genel Bakış
      ├── Web Sitesi
      ├── DNS Kayıtları
      ├── Mail
      ├── Database'ler
      ├── Sertifikalar
      ├── Dosyalar ve FTP
      ├── Backup
      ├── Loglar
      └── Ayarlar
```

Aynı sekme kümesi domain kullanıcısına `http://<domain>:8080` üzerinde gösterilir. Bu görünümde sunucu sol menüsü, başka domain seçimi, component/provider kurulumu ve global ayarlar bulunmaz. Sekmelerdeki tüm veri ve aksiyonlar oturumun `domainId` değeriyle backend tarafında sınırlandırılır.

| Domain sekmesi | Domaine bağlı içerik | Birincil agent | Destek agent'ları |
|---|---|---|---|
| Genel Bakış | Domain durumu, IP, web/mail/DNS/database özeti, uyarılar ve son işlemler | `domain_hosting_specialist` | `monitoring_logging_specialist` |
| Web Sitesi | IIS site, binding, host header, application pool ilişkisi, document root ve runtime ayarları | `web_server_specialist` | `application_runtime_specialist`, `file_deployment_specialist` |
| DNS Kayıtları | Seçili domaine ait DNS zone ve A/AAAA/CNAME/MX/TXT/CAA/SRV kayıtları | `dns_specialist` | `domain_hosting_specialist` |
| Mail | MailEnable mail domaini, mailbox, alias, forwarding, quota ve domaine özel kimlik doğrulama kayıtları | `mailenable_specialist` | `mail_management_specialist`, `mail_security_specialist` |
| Database'ler | Domaine atanmış database'ler, kullanıcılar ve uygulama bağlantı bilgileri | `database_integration_specialist` | `sql_server_engineer`, `postgresql_engineer` |
| Sertifikalar | Domaine ait sertifikalar, SAN/wildcard bilgisi, IIS/mail binding ve sona erme durumu | `tls_certificate_specialist` | `web_server_specialist`, `mailenable_specialist` |
| Dosyalar ve FTP | Domaine ait document root, dosya izinleri, FTP hesapları ve deployment hedefi | `file_deployment_specialist` | `windows_server_engineer` |
| Backup | Yalnızca bu domaine ait site, DNS, mail ve database backup/restore işlemleri | `backup_restore_specialist` | `remote_backup_storage_specialist` ve ilgili bileşen agent'ı |
| Loglar | Domaine filtrelenmiş IIS access/error logları, mail akışı ve ilgili uygulama logları | `monitoring_logging_specialist` | İlgili bileşenin uzman agent'ı |
| Ayarlar | Domain durumu, varsayılan davranışlar, limitler ve domaine özgü metadata | `domain_hosting_specialist` | `web_security_specialist` |

### 2.2 Genel Bakış ekranı

> **Birincil agent:** `domain_hosting_specialist`  
> **Destek agent'ı:** `monitoring_logging_specialist`

```text
Genel Bakış
├── Domain Durumu
├── IP ve Binding Özeti
├── Website Durumu
├── DNS Durumu
├── Mail Durumu
├── Database Özeti
├── Sertifika Uyarıları
├── Kaynak Kullanımı
├── Son Job'lar
└── Son Audit Olayları
```

Bu ekran yalnızca özet ve hızlı geçiş sağlar. Değişiklik yapan ayrıntılı işlemler ilgili domain sekmesinden yürütülür.

### 2.3 Web Sitesi ekranları

> **Birincil agent:** `web_server_specialist`  
> **Destek agent'ları:** `application_runtime_specialist`, `file_deployment_specialist`, `windows_server_engineer`

**Zorunlu bileşen geçişi:** Domainin web sitesi işlevi IIS olmadan kullanılamaz. IIS kurulu değilse bu sekme normal içeriği açmaz ve `IIS Web Server Kur` yönlendirmesi gösterir. IIS kurulu fakat servisleri durmuşsa `IIS'i Aktif Et` yönlendirmesi gösterir. Domainin aktif web durumu, IIS sağlık kontrolü başarılı olmadan tamamlanamaz.

```text
Web Sitesi
├── IIS Site Ayarları
├── Binding ve Host Header
├── Application Pool
├── Document Root
├── Runtime Ayarları
├── Varsayılan Dokümanlar
├── Authentication ve Authorization
├── HTTP Redirect ve Header Ayarları
├── Logging
└── Website Durumu
```

- `IIS Site Ayarları`: Site kimliği, fiziksel path, başlatma/durdurma durumu ve temel IIS ilişkileri.
- `Binding ve Host Header`: HTTP/HTTPS binding, IP, port, host name ve SNI ilişkileri.
- `Application Pool`: Atanmış pool, runtime, pipeline, identity, recycling ve limitler.
- `Document Root`: Domain dosyalarının kök dizini ve erişim bilgileri.
- `Runtime Ayarları`: ASP.NET/.NET ve desteklenen diğer runtime yapılandırmaları.
- `Authentication ve Authorization`: Yalnızca bu siteyi etkileyen IIS erişim ayarları.
- `Website Durumu`: Site, application pool ve binding sağlık kontrolleri.

Sunucunun tamamını etkileyen IIS modülleri ve global defaults bu ekranda değil, sol menüdeki `IIS Yönetimi` altında bulunur.

### 2.4 DNS Kayıtları ekranları

> **Birincil agent:** `dns_specialist`  
> **Destek agent'ları:** `domain_hosting_specialist`, `windows_server_engineer`

**Zorunlu bileşen geçişi:** Windows DNS Server kurulu değilse bu sekme DNS kayıtlarını göstermez ve `DNS Server Kur` yönlendirmesi sunar. DNS rolü kurulu fakat servis durmuşsa `DNS Server'ı Aktif Et` gösterilir. Zone işlemleri DNS servis doğrulaması başarılı olmadan başlatılmaz.

```text
DNS Kayıtları
├── Zone Özeti
├── A ve AAAA Kayıtları
├── CNAME Kayıtları
├── MX Kayıtları
├── TXT Kayıtları
├── CAA Kayıtları
├── SRV Kayıtları
├── SOA ve NS Bilgileri
├── Yeni Kayıt Ekle
└── Değişiklik Geçmişi
```

Bu alan yalnızca seçili domain zone'unu ve kayıtlarını yönetir. Forwarder, recursion, cache ve server-wide zone transfer politikası sol menüdeki `DNS Sunucusu` altında bulunur.

### 2.5 Mail ekranları

> **Birincil agent:** `mailenable_specialist`  
> **Destek agent'ları:** `mail_management_specialist`, `mail_security_specialist`, `dns_specialist`

**Zorunlu bileşen geçişi:** Mail özelliğinin desteklenen ilk sürümü ücretsiz [MailEnable Standard Edition](https://www.mailenable.com/standard_edition.asp) kullanır. MailEnable Standard kurulu değilse bu sekme `MailEnable Standard Kur`; kurulu fakat gerekli servisleri durmuşsa `MailEnable'ı Aktif Et` yönlendirmesi gösterir. Mail domaini ve mailbox işlemleri temel MailEnable sağlık kontrolü başarılı olmadan açılmaz.

```text
Mail
├── Mail Domaini
├── Mailbox'lar
├── Alias'lar
├── Yönlendirmeler
├── Kota Ayarları
├── Catch-All Ayarı
├── SPF
├── DKIM
├── DMARC
├── Mail Akışı ve Teslimat
└── Domain Mail Logları
```

- `Mailbox'lar`: Seçili domaine ait mailbox yaşam döngüsü ve durum bilgileri.
- `Alias'lar` ve `Yönlendirmeler`: Mail adresi eşleştirmeleri ve forwarding hedefleri.
- `SPF`, `DKIM` ve `DMARC`: Domaine bağlı authentication yapılandırması ve gerekli DNS kayıtları.
- `Mail Akışı ve Teslimat`: Domaine filtrelenmiş inbound/outbound durum ve hata bilgileri.

Connector, queue, relay, antivirus ve server-wide anti-spam politikaları sol menüdeki `Mail Sunucusu > MailEnable` ekranında bulunur.

### 2.6 Database ekranları

> **Birincil agent:** `database_integration_specialist`  
> **Destek agent'ları:** `sql_server_engineer`, `postgresql_engineer`

**Zorunlu bileşen geçişi:** Database özelliği kullanılmadan önce en az bir desteklenen provider hazır olmalıdır. SQL Server, PostgreSQL, MySQL ve MariaDB ayrı provider durum kartları olarak gösterilir. Seçilen provider'ın ürünü kurulu değilse `Kur`, kurulu fakat servisi durmuşsa `Aktif Et` yönlendirmesi sunulur. Kullanılabilir provider bulunmadığında database oluşturma, kullanıcı oluşturma, atama, backup ve restore işlemleri açılmaz.

```text
Database'ler
├── Provider Durum Kartları
├── Provider Seç
├── Atanmış Database'ler
├── Database Detayı
├── Database Kullanıcıları
├── Yeni Database Oluştur
├── Domaine Database Ata
├── Bağlantı Bilgileri
├── Backup
└── Restore
```

- Domain yalnızca kendisine atanmış database ve kullanıcı ilişkilerini gösterir.
- Her database satırı ilişkili website/Application Pool, authentication mode ve principal durumunu gösterir.
- Principal yalnızca atanmış database'e erişir; başka domain database'lerine erişim verilmez.
- SQL Server, PostgreSQL, MySQL ve MariaDB kurulumları; instance/service, port ve global yapılandırma sol menüdeki `Database Sistemleri` altında provider bazında bulunur.
- Domain ekranı provider'a özgü sabit formlar yerine provider capability metadata'sına göre alan ve aksiyon gösterir.
- Bağlantı bilgilerindeki parola ve secret değerleri varsayılan olarak gösterilmez veya loglanmaz.

### 2.7 Sertifika ekranları

> **Birincil agent:** `tls_certificate_specialist`  
> **Destek agent'ları:** `web_server_specialist`, `mailenable_specialist`

```text
Sertifikalar
├── Aktif Sertifika
├── Uygun Sertifikalar
├── IIS Binding'leri
├── Mail Binding'leri
├── SAN ve Wildcard Bilgisi
├── Sertifika Zinciri
├── Sona Erme Tarihi
├── Sertifika Ata veya Değiştir
└── Yenileme Durumu
```

Bu ekran domaine atanabilecek veya atanmış sertifikaları gösterir. Local Machine certificate store'un tamamı ve sunucu servislerine genel atamalar sol menüdeki `Sertifikalar` ekranında yönetilir.

### 2.8 Dosyalar ve FTP ekranları

> **Birincil agent:** `file_deployment_specialist`  
> **Destek agent'ları:** `windows_server_engineer`, `web_security_specialist`

```text
Dosyalar ve FTP
├── Dosya Yöneticisi
├── Document Root
├── Dosya ve Klasör Detayı
├── NTFS İzinleri
├── FTP Hesapları
├── FTP Yetkileri
├── Deployment Hedefleri
└── Dosya İşlem Geçmişi
```

- Dosya yöneticisi varsayılan olarak domain document root sınırları içinde çalışır.
- Domain dışındaki volume, share ve genel NTFS işlemleri sol menüdeki `Depolama` ekranında bulunur.
- Path traversal, reparse point ve symlink sınırları tüm dosya işlemlerinde doğrulanmalıdır.
- FTP hesabı isteğe bağlı ve Application Pool/domain panel hesabından ayrıdır; oluşturma, parola yenileme ve devre dışı bırakma bu sekmeden yapılır.

### 2.9 Backup ekranları

> **Birincil agent:** `backup_restore_specialist`  
> **Destek agent'ları:** `remote_backup_storage_specialist` ve backup kapsamındaki bileşen agent'ları

```text
Backup
├── Domain Backup Özeti
├── Yeni Backup
├── Backup Geçmişi
├── İçerik Kapsamı
├── Restore
├── Restore Ön İzlemesi
└── İlgili Job'lar
```

Domain backup'ı site dosyaları, DNS, mail ve atanmış database ilişkilerini seçilebilir kapsam olarak gösterebilir. Sunucu geneli backup hedefi, storage, retention ve schedule ayarları sol menüdeki `Backup Yönetimi` altında bulunur.

### 2.10 Loglar ekranları

> **Birincil agent:** `monitoring_logging_specialist`  
> **Destek agent'ları:** `web_server_specialist`, `mailenable_specialist`, `database_integration_specialist`

```text
Loglar
├── IIS Access Logları
├── IIS Error Logları
├── Mail Logları
├── Database İlişkili Olaylar
├── Domain Job Logları
├── Domain Audit Olayları
├── Filtreler
└── Dışa Aktarma
```

Bu ekran merkezi logların yalnızca seçili `domainId` ile ilişkilendirilebilen bölümünü gösterir. Windows ve servis loglarının tamamı sol menüdeki `Loglar` ekranında bulunur.

### 2.11 Domain Ayarları ekranları

> **Birincil agent:** `domain_hosting_specialist`  
> **Destek agent'ları:** `web_security_specialist`, `automation_api_specialist`

```text
Ayarlar
├── Domain Kimliği
├── Display Name ve Punycode Bilgisi
├── Domain Durumu
├── Varsayılan IP
├── Özellik Durumları
├── Domaine Özel Limitler
├── Metadata ve Etiketler
└── Domain Silme
```

Domain silme yüksek etkili bir operasyondur. Web sitesi, DNS zone, mail domaini, database atamaları, sertifikalar, dosyalar ve backup'lar için ayrı etki özeti sunulmalı; hiçbir bağımlılık varsayılan olarak sessizce silinmemelidir.

## 3. Sunucu ve domain kapsamı ayrım tablosu

> **Birincil agent:** `domain_hosting_specialist`  
> **Destek agent'ları:** `server_operations_specialist` ve ilgili bileşen agent'ları

| Özellik alanı | Sol menüdeki sunucu kapsamı | Domain Detayı kapsamı | Sorumlu agent'lar |
|---|---|---|---|
| IIS | Servis, global modüller, server defaults ve global limitler | Site, binding, app pool ilişkisi, runtime ve document root | `web_server_specialist`, `application_runtime_specialist` |
| DNS | Servis, forwarder, recursion, cache ve transfer politikası | Zone ve domain kayıtları | `dns_specialist`, `windows_server_engineer` |
| Database provider'ları | Provider kataloğu; SQL Server, PostgreSQL, MySQL ve MariaDB instance/service, port, global config ve sağlık | `providerId` ve `instanceId` üzerinden domaine atanmış database ve kullanıcılar | `database_integration_specialist`, `sql_server_engineer`, `postgresql_engineer` ve provider uzmanları |
| MailEnable | Servisler, connector, queue, relay ve güvenlik politikaları | Mail domaini, mailbox, alias, forwarding ve quota | `mailenable_specialist`, `mail_management_specialist`, `mail_security_specialist` |
| Sertifikalar | Certificate store ve genel envanter | Domaine ve binding'e atanmış sertifikalar | `tls_certificate_specialist` |
| Dosyalar | Disk, volume, share ve genel NTFS yönetimi | Document root, domain dosyaları ve FTP erişimi | `file_deployment_specialist`, `windows_server_engineer` |
| Backup | Hedef storage, schedule, retention ve server backup | Domain içeriği için backup/restore | `backup_restore_specialist`, `remote_backup_storage_specialist` |
| Monitoring | Sistem ve servis sağlığı | Domain/site/mail/database bazlı metrikler | `monitoring_logging_specialist` |
| Loglar | Windows ve servis loglarının tamamı | Domain kimliğiyle filtrelenmiş loglar | `monitoring_logging_specialist` ve ilgili bileşenin uzman agent'ı |
| Firewall | Sunucu profilleri ve kurallar | Domain Detayı'nda bulunmaz | `web_security_specialist`, `windows_server_engineer` |
| Windows kullanıcıları | Yerel hesaplar ve gruplar | Domain Detayı'nda bulunmaz | `windows_server_engineer` |
| Windows Update | Sunucu güncellemeleri | Domain Detayı'nda bulunmaz | `server_operations_specialist`, `windows_server_engineer` |

## 4. Menü girişinde bileşen kurulumu ve aktivasyonu

> **Birincil agent:** `server_operations_specialist`  
> **Destek agent'ları:** `windows_server_engineer` ve ilgili bileşenin uzman agent'ı

### 4.1 Geçiş kapısı ekranı

> **Birincil agent:** `server_operations_specialist`  
> **Destek agent'ı:** `automation_api_specialist`

```text
Bileşen Durumu
├── Bileşen Adı ve Sürümü
├── Mevcut Durum
├── Eksik Bağımlılıklar
├── Etkilenecek Windows Rolleri ve Servisler
├── Açılacak Portlar
├── İndirme ve Disk Gereksinimi
├── Yeniden Başlatma Gereksinimi
├── Ana Aksiyon
│   ├── Kur
│   ├── Aktif Et
│   ├── Yapılandır
│   ├── Onar
│   └── Tekrar Kontrol Et
└── Job Durumu
```

Normal özellik ekranı yalnızca bileşen `Kurulu ve Çalışıyor` doğrulamasını geçtiğinde açılır. Kurulum veya servis başlatma işlemi kullanıcı onayı olmadan otomatik başlamaz.

### 4.2 Bileşen ve menü eşleştirmesi

> **Birincil agent:** `windows_server_engineer`  
> **Destek agent'ları:** `web_server_specialist`, `dns_specialist`, `database_integration_specialist`, `mailenable_specialist`

| Açılan menü/ekran | Gerekli bileşen | Kurulu değil | Kurulu fakat durmuş | Hazır olduğunda |
|---|---|---|---|---|
| `Domainler > Domain Detayı > Web Sitesi` | IIS Web Server | `IIS Web Server Kur` | `IIS'i Aktif Et` | Web Sitesi ekranlarını aç |
| `IIS Yönetimi` | IIS Web Server | `IIS Web Server Kur` | `IIS'i Aktif Et` | Global IIS yönetimini aç |
| `Domainler > Domain Detayı > DNS Kayıtları` | Windows DNS Server | `DNS Server Kur` | `DNS Server'ı Aktif Et` | Zone ve kayıt ekranlarını aç |
| `DNS Sunucusu` | Windows DNS Server | `DNS Server Kur` | `DNS Server'ı Aktif Et` | Global DNS yönetimini aç |
| `Database Sistemleri > SQL Server` | Microsoft SQL Server | `SQL Server Kur` | `SQL Server'ı Aktif Et` | Instance yönetimini aç |
| `Database Sistemleri > PostgreSQL` | PostgreSQL | `PostgreSQL Kur` | `PostgreSQL'i Aktif Et` | Cluster/database yönetimini aç |
| `Database Sistemleri > MySQL` | MySQL | `MySQL Kur` | `MySQL'i Aktif Et` | Instance/service yönetimini aç |
| `Database Sistemleri > MariaDB` | MariaDB | `MariaDB Kur` | `MariaDB'yi Aktif Et` | Instance/service yönetimini aç |
| `Domainler > Domain Detayı > Database'ler` | En az bir hazır database provider'ı | Provider seçim ve kurulum kartlarını göster | Seçilen provider için `Aktif Et` | Domain database ekranlarını provider capability'lerine göre aç |
| `Mail Sunucusu > MailEnable` | MailEnable Standard Edition | `MailEnable Standard Kur` | `MailEnable'ı Aktif Et` | Global mail yönetimini aç |
| `Domainler > Domain Detayı > Mail` | MailEnable Standard Edition | `MailEnable Standard Kur` | `MailEnable'ı Aktif Et` | Domain mail ekranlarını aç |

### 4.3 Kurulum job ekranı

> **Birincil agent:** `automation_api_specialist`  
> **Destek agent'ları:** `server_operations_specialist`, `monitoring_logging_specialist`

```text
Kurulum Job'u
├── Plan Özeti
├── Kullanıcı Onayı
├── Paket İndirme
├── Hash ve Kaynak Doğrulama
├── Ön Koşul Kurulumu
├── Ana Bileşen Kurulumu
├── Servis Aktivasyonu
├── Sağlık Kontrolü
├── Yeniden Başlatma Beklentisi
├── Başarı veya Hata Özeti
└── Rollback Bilgisi
```

Job başarıyla tamamlandıktan sonra bileşen durumu yeniden keşfedilir. Sağlık kontrolü başarısızsa normal özellik ekranı açılmaz; `Onar` veya `Tekrar Kontrol Et` durumu gösterilir.
