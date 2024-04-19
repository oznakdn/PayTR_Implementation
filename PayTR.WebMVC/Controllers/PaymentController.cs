using Microsoft.AspNetCore.Mvc;
using PayTR.WebMVC.Models;
using System.Security.Cryptography;
using System.Text;

namespace PayTR.WebMVC.Controllers;

public class PaymentController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly PayTrConfiguration _payTrConfiguration;
    private readonly ILogger<PaymentController> _logger;
    private string? resultMessage;
    private readonly string url = "https://www.paytr.com/odeme";


    public PaymentController(HttpClient httpClient, PayTrConfiguration payTrConfiguration, ILogger<PaymentController> logger)
    {
        _httpClient = httpClient;
        _payTrConfiguration = payTrConfiguration;
        _logger = logger;
    }

    public IActionResult Pay()
    {
        return View();
    }

    /*
     Kart Sahibi Adı: TEST KARTI
     Kart Numarası: 9792030394440796
     Kart Son Kullanma Ay: 12
     Kart Son Kullanma Yıl: 99
     Kart Güvenlik Kodu: 000
     */

    [HttpPost]
    public async Task Pay([FromForm] PaymentRequestModel paymentRequest)
    {
        paymentRequest.merchant_id = _payTrConfiguration.merchant_id;
        paymentRequest.merchant_key = _payTrConfiguration.merchant_key;
        paymentRequest.merchant_salt = _payTrConfiguration.merchant_salt;

        paymentRequest.merchant_ok_url = $"{Request.Scheme}/{Request.Host}/Payment/Success/?SiparisNo= {paymentRequest.merchant_oid}";
        paymentRequest.merchant_fail_url = $"{Request.Scheme}/{Request.Host}/Payment/Fail/?SiparisNo= {paymentRequest.merchant_oid}";

        HttpResponseMessage request = await _httpClient.PostAsJsonAsync<PaymentRequestModel>(url, paymentRequest);
        resultMessage = await request.Content.ReadAsStringAsync();
        _logger.LogInformation($"Result Message: {resultMessage}");
    }


    [HttpPost]
    public async Task PayCallBack(string merchant_oid,
        string status,
        int total_amount,
        string hash,
        int failed_reason_code,
        string failed_reason_msg,
        string payment_type,
        string currency,
        decimal payment_amount)
    {

        _logger.LogInformation($"Failed Code: {failed_reason_code} - Failed Message: {failed_reason_msg} Payment Type: {payment_type} Currency :{currency} Payment Amount: {payment_amount}");

        string concat = string.Concat(merchant_oid, _payTrConfiguration.merchant_salt, status, total_amount);
        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_payTrConfiguration.merchant_key));
        byte[] hmacByte = hmac.ComputeHash(Encoding.UTF8.GetBytes(concat));
        string token = Convert.ToBase64String(hmacByte);

        if (hash.ToString() != token)
        {
            _logger.LogError("PAYTR notification failed: bad hash");
            Content("PAYTR notification failed: bad hash");
            return;
        }

        // BURADA YAPILMASI GEREKENLER
        // 1) Siparişin durumunu $post['merchant_oid'] değerini kullanarak veri tabanınızdan sorgulayın.
        // 2) Eğer sipariş zaten daha önceden onaylandıysa veya iptal edildiyse  echo "OK"; exit; yaparak sonlandırın. Yani await Response.WriteAsync("OK");


        //Ödeme Onaylandı
        if (status == "success")
        {
            // Bildirimin alındığını PayTR sistemine bildir.  
            await Response.WriteAsync("OK");

            // BURADA YAPILMASI GEREKENLER ONAY İŞLEMLERİDİR.
            // 1) Siparişi onaylayın.
            // 2) iframe çağırma adımında merchant_oid ve diğer bilgileri veri tabanınıza kayıp edip bu aşamada karşılaştırarak eğer var ise bilgieri çekebilir ve otomatik sipariş tamamlama işlemleri yaptırabilirsiniz.
            // 2) Eğer müşterinize mesaj / SMS / e-posta gibi bilgilendirme yapacaksanız bu aşamada yapabilirsiniz. Bu işlemide yine iframe çağırma adımında merchant_oid bilgisini kayıt edip bu aşamada sorgulayarak verilere ulaşabilirsiniz.
            // 3) 1. ADIM'da gönderilen payment_amount sipariş tutarı taksitli alışveriş yapılması durumunda
            // değişebilir. Güncel tutarı Request.Form['total_amount'] değerinden alarak muhasebe işlemlerinizde kullanabilirsiniz.
        }
        //Ödemeye Onay Verilmedi
        else
        {
            // Bildirimin alındığını PayTR sistemine bildir.
            await Response.WriteAsync("OK");
            // BURADA YAPILMASI GEREKENLER
            // 1) Siparişi iptal edin.
            // 2) Eğer ödemenin onaylanmama sebebini kayıt edecekseniz aşağıdaki değerleri kullanabilirsiniz.
            // $post['failed_reason_code'] - başarısız hata kodu
            // $post['failed_reason_msg'] - başarısız hata mesajı
        }
    }


    public IActionResult Success(string SiparisNo)
    {
        ViewData["successMessage"] = $"{resultMessage} - {SiparisNo}";
        return View();
    }

    public IActionResult Fail(string SiparisNo)
    {
        ViewData["failMessage"] = $"{resultMessage} - {SiparisNo}";
        return View();
    }
}
