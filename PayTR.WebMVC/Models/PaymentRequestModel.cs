using PayTR.WebMVC.Helper;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PayTR.WebMVC.Models;

public class PaymentRequestModel
{

    // Mağaza No: PayTR tarafından size verilen Mağaza numarası
    public int merchant_id { get; set; }

    // Key: PayTR tarafından size verilen key
    public string merchant_key { get; set; }

    // Key: PayTR tarafından size verilen salt
    public string merchant_salt { get; set; }



    // Müşteri ip: İstek anında aldığınız müşteri ip numarası (https://www.whatismyip.com/)
    public string user_ip { get; set; } = "XXXXXX";




    // Sipariş numarası: Her işlemde benzersiz olmalıdır!! Bu bilgi bildirim sayfanıza yapılacak bildirimde geri gönderilir.
    public string merchant_oid => Guid.NewGuid().ToString();


    // Müşterinizin sitenizde kayıtlı veya form vasıtasıyla aldığınız eposta adresi
    public string email { get; set; } = "info@siteniz.com";

    // Tahsil edilecek tutar.
    public double payment_amount { get; set; } = 3000.00;

    // Ödeme türü
    //public PaymentType payment_type { get; set; } // string
    public string payment_type { get; set; } = "card";


    // Alabileceği değerler; advantage, axess, combo, bonus, cardfinans, maximum, paraf, world, saglamkart
    //public CardType card_type { get; set; } // string
    public string card_type { get; set; } = "bonus";


    // Taksit Sayısı
    public int installment_count { get; set; } = 0;

    // Para birimi
    public string currency => "TR";

    // Mağaza canlı modda iken test işlem yapmak için 1 olarak gönderilebilir
    public string test_mode { get; set; } = "1";


    // Non 3D işlem yapabilmek için 1 gönderilebilir
    public string non_3d { get; set; } = "0";

    // Non 3D işlemde, başarısız işlem durumunu test etmek için 1 gönderilir (test_mode ve non_3d değerleri 1 ise dikkate alınır!)
    public string non3d_test_failed { get; set; } = "1";


    // İsteğin sizden geldiğine ve içeriğin değişmediğine emin olmamız için oluşturacağınız değerdir
    public string paytr_token => TokenHelper.CreatePayTrToken(merchant_id, user_ip, merchant_oid, email, payment_amount, payment_type, installment_count, currency, test_mode, non_3d, merchant_salt, merchant_key);

   
    public string client_lang { get; set; } = "tr";


    
    public string debug_on { get; set; } = "1";
    public string merchant_ok_url { get; set; }
    public string merchant_fail_url { get; set; }




    // Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız ad ve soyad bilgisi
    public string user_name { get; set; } = "Paytr Test";


    // Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız adres bilgisi
    public string user_address { get; set; } = "test test test";


    // Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız telefon bilgisi
    public string user_phone { get; set; } = "05555555555";


    // Sepet içeriği: Müşterinin siparişindeki ürün/hizmet bilgilerini içermelidir
    public string user_basket
    {
        get
        {
            return JsonSerializer.Serialize<List<UserBasket>>(new List<UserBasket>
            {
                new UserBasket{ProductName="Product1",UnitPrice=1000,Quantity=1},
                new UserBasket{ProductName="Product2",UnitPrice=2000,Quantity=1}
            });
        }
    }

    // Kart sahibi

    [MaxLength(50)]
    public string cc_owner { get; set; }

    // Kart numarası
    [CreditCard]
    [StringLength(maximumLength: 16, MinimumLength = 16)]
    public string card_number { get; set; }


    //Kart son kullanma tarihi(Ay)
    [StringLength(maximumLength: 12, MinimumLength = 1)]
    public string expiry_month { get; set; } // 1, 2, 3, .. , 11, 12


    // Kart son kullanma tarihi(Yıl)
    [StringLength(maximumLength: 2, MinimumLength = 2)]
    public string expiry_year { get; set; } // 18, 19, 20,…


    // Kart güvenlik kodu
    [StringLength(maximumLength: 3, MinimumLength = 3)]
    public string cvv { get; set; }


}
