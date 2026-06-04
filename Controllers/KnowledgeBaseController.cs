using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITDestek.Controllers;

[Authorize]
public class KnowledgeBaseController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Details(int id)
    {
        // Makale verilerini simüle et
        var articles = new Dictionary<int, ArticleViewModel>
        {
            [1] = new ArticleViewModel
            {
                Id = 1,
                Title = "Yazıcıya Nasıl Bağlanır?",
                Icon = "fa-print",
                Color = "blue",
                Views = 1234,
                Content = @"
                    <h3 class='text-xl font-bold mb-3'>Ağ Yazıcısı Ekleme</h3>
                    <p class='mb-3'>Windows 10/11 sistemine ağ yazıcısı eklemek için şu adımları izleyin:</p>
                    <ol class='list-decimal list-inside space-y-2 mb-4'>
                        <li>Başlat > Ayarlar > Bluetooth ve cihazlar > Yazıcılar ve tarayıcılar yolunu izleyin</li>
                        <li>'Yazıcı veya tarayıcı ekle' butonuna tıklayın</li>
                        <li>Listede yazıcınızı bulun ve seçin</li>
                        <li>Sürücü otomatik olarak yüklenecektir</li>
                        <li>Yükleme tamamlandığında 'Yazdır' test sayfasını yazdırarak kontrol edin</li>
                    </ol>
                    <h3 class='text-xl font-bold mb-3'>Kablosuz Bağlantı</h3>
                    <p class='mb-3'>WiFi bağlantılı yazıcılar için:</p>
                    <ul class='list-disc list-inside space-y-2 mb-4'>
                        <li>Yazıcının WiFi menüsünden belediye ağını seçin (TuzlaBelediye)</li>
                        <li>Gerekirse yazıcı IP adresini not alın</li>
                        <li>Bilgisayarınızdan IP adresi ile manuel ekleyebilirsiniz</li>
                    </ul>
                    <div class='bg-yellow-50 border border-yellow-200 rounded-lg p-4 mt-4'>
                        <p class='font-semibold text-yellow-800'><i class='fas fa-exclamation-triangle mr-2'></i>Önemli Not</p>
n                        <p class='text-sm text-yellow-700 mt-1'>Sorun devam ederse Bilgi İşlem Müdürlüğü'nü arayınız.</p>
                    </div>"
            },
            [2] = new ArticleViewModel
            {
                Id = 2,
                Title = "VPN Bağlantısı Kurulumu",
                Icon = "fa-shield-alt",
                Color = "green",
                Views = 987,
                Content = @"
                    <h3 class='text-xl font-bold mb-3'>VPN Nedir?</h3>
                    <p class='mb-3'>VPN (Virtual Private Network), uzaktan güvenli şekilde belediye ağına bağlanmanızı sağlar.</p>
                    <h3 class='text-xl font-bold mb-3 mt-6'>Kurulum Adımları</h3>
                    <ol class='list-decimal list-inside space-y-2 mb-4'>
                        <li>Windows'ta Ayarlar > Ağ ve İnternet > VPN yolunu izleyin</li>
                        <li>'VPN ekle' butonuna tıklayın</li>
                        <li>Sağlayıcı: Windows (yerleşik)</li>
                        <li>Bağlantı adı: TuzlaBelediyeVPN</li>
                        <li>Sunucu adı: vpn.tuzla.bel.tr</li>
                        <li>VPN türü: Otomatik</li>
                        <li>Kullanıcı adı ve şifre: Kurumsal kimlik bilgileriniz</li>
                    </ol>
                    <h3 class='text-xl font-bold mb-3 mt-6'>Bağlantı Kurma</h3>
                    <p class='mb-3'>Sağ alt köşedeki WiFi simgesine tıklayın, TuzlaBelediyeVPN'i seçin ve Bağlan'ı tıklayın.</p>"
            },
            [3] = new ArticleViewModel
            {
                Id = 3,
                Title = "E-posta Şifre Sıfırlama",
                Icon = "fa-envelope",
                Color = "purple",
                Views = 2156,
                Content = @"
                    <h3 class='text-xl font-bold mb-3'>Şifrenizi Mi Unuttunuz?</h3>
                    <p class='mb-3'>Kurumsal e-posta şifrenizi sıfırlamak için:</p>
                    <ol class='list-decimal list-inside space-y-2 mb-4'>
                        <li>https://mail.tuzla.bel.tr adresine gidin</li>
                        <li>'Şifremi unuttum' bağlantısına tıklayın</li>
                        <li>Kurumsal e-posta adresinizi girin</li>
                        <li>Alternatif e-posta veya telefonunuza kod gönderin</li>
                        <li>Gelen kodu kullanarak yeni şifrenizi belirleyin</li>
                    </ol>
                    <h3 class='text-xl font-bold mb-3 mt-6'>Şifre Gereksinimleri</h3>
                    <ul class='list-disc list-inside space-y-2 mb-4'>
                        <li>En az 8 karakter</li>
                        <li>En az 1 büyük harf</li>
                        <li>En az 1 rakam</li>
                        <li>Özel karakter içermesi önerilir</li>
                    </ul>
                    <div class='bg-blue-50 border border-blue-200 rounded-lg p-4 mt-4'>
                        <p class='font-semibold text-blue-800'><i class='fas fa-info-circle mr-2'></i>İpucu</p>
                        <p class='text-sm text-blue-700 mt-1'>Şifrenizi her 90 günde bir değiştirmeniz güvenlik için önemlidir.</p>
                    </div>"
            },
            [4] = new ArticleViewModel
            {
                Id = 4,
                Title = "Bilgisayar Açılmıyor",
                Icon = "fa-desktop",
                Color = "orange",
                Views = 756,
                Content = @"
                    <h3 class='text-xl font-bold mb-3'>Güç Sorunları</h3>
                    <p class='mb-3'>Bilgisayar hiç açılmıyorsa:</p>
                    <ol class='list-decimal list-inside space-y-2 mb-4'>
                        <li>Güç kablosunun hem bilgisayara hem de prize tam takılı olduğundan emin olun</li>
                        <li>Prizde elektrik olup olmadığını başka bir cihazla test edin</li>
                        <li>Masaüstü bilgisayarda güç kaynağı anahtarının açık (I) konumda olduğunu kontrol edin</li>
                        <li>Farklı bir güç kablosu deneyin</li>
                    </ol>
                    <h3 class='text-xl font-bold mb-3 mt-6'>Ekran Geliyor Ama Windows Açılmıyor</h3>
                    <ul class='list-disc list-inside space-y-2 mb-4'>
                        <li>Bilgisayarı yeniden başlatın</li>
                        <li>F8 tuşuna basarak Güvenli Mod'u deneyin</li>
                        <li>Son yüklenen programları kaldırın</li>
                    </ul>"
            },
            [5] = new ArticleViewModel
            {
                Id = 5,
                Title = "İnternet Bağlantı Sorunları",
                Icon = "fa-wifi",
                Color = "red",
                Views = 1845,
                Content = @"
                    <h3 class='text-xl font-bold mb-3'>WiFi Bağlantısı Koptu</h3>
                    <ol class='list-decimal list-inside space-y-2 mb-4'>
                        <li>WiFi simgesine sağ tıklayın > Sorun giderici'yi seçin</li>
                        <li>Uçak modunu açıp 10 saniye bekleyip kapatın</li>
                        <li>WiFi'yi kapatıp tekrar açın</li>
                        <li>Ağı unut deyin ve tekrar bağlanın</li>
                    </ol>
                    <h3 class='text-xl font-bold mb-3 mt-6'>Modemi Sıfırlama</h3>
                    <p class='mb-3'>Kablolu bağlantıda sorun varsa:</p>
                    <ul class='list-disc list-inside space-y-2 mb-4'>
                        <li>Modemin arkasındaki küçük deliğe 10 saniye basılı tutun</li>
                        <li>Modemi fişten çekin, 30 saniye bekleyin, tekrar takın</li>
                        <li>Tüm ışıkların yanmasını bekleyin (2-3 dakika)</li>
                    </ul>"
            },
            [6] = new ArticleViewModel
            {
                Id = 6,
                Title = "Ortak Dosya Paylaşımı",
                Icon = "fa-folder-open",
                Color = "indigo",
                Views = 623,
                Content = @"
                    <h3 class='text-xl font-bold mb-3'>Ağ Klasörü Oluşturma</h3>
                    <ol class='list-decimal list-inside space-y-2 mb-4'>
                        <li>Paylaşmak istediğiniz klasöre sağ tıklayın > Özellikler</li>
                        <li>'Paylaşım' sekmesine gidin</li>
                        <li>'Gelişmiş Paylaşım' > 'Bu klasörü paylaş'</li>
                        <li>'İzinler' butonuna tıklayın</li>
                        <li>Departmandaki kişileri ekleyin ve yetkileri ayarlayın (Okuma/Yazma)</li>
                    </ol>
                    <h3 class='text-xl font-bold mb-3 mt-6'>Erişim</h3>
                    <p class='mb-3'>Paylaşılan klasöre erişmek için:</p>
                    <code class='bg-gray-100 px-3 py-2 rounded block my-2'>\\bilgisayar-adi\paylasilan-klasor</code>
                    <p class='mt-3'>veya Ağ bölümünden bulabilirsiniz.</p>"
            }
        };
        
        if (!articles.ContainsKey(id))
        {
            return NotFound();
        }
        
        return View(articles[id]);
    }
}

public class ArticleViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Views { get; set; }
    public string Content { get; set; } = string.Empty;
}
