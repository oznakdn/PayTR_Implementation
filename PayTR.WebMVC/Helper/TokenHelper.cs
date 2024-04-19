using System.Security.Cryptography;
using System.Text;

namespace PayTR.WebMVC.Helper;

public class TokenHelper
{
    public static string CreatePayTrToken(int merchant_id,string user_ip,string merchant_oid, string emailstr, double payment_amountstr, string payment_type, int installment_count, string currency, string test_mode, string non_3d, string merchant_salt, string merchant_key)
    {
        string concat = string.Concat(
            merchant_id,
            user_ip,
            merchant_oid,
            emailstr,
            payment_amountstr.ToString(),
            payment_type.ToString(),
            installment_count,
            currency,
            test_mode,
            non_3d,
            merchant_salt);

        HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchant_key));
        byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(concat));
        return Convert.ToBase64String(b);
    }
}
