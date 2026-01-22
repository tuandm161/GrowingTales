using System.Net;
using System.Security.Cryptography;
using System.Text;
using Server.Models;

namespace Server.Services;

public class VnPayService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<VnPayService> _logger;
    
    private readonly string _vnpUrl;
    private readonly string _vnpTmnCode;
    private readonly string _vnpHashSecret;
    private readonly string _vnpVersion;

    public VnPayService(IConfiguration configuration, ILogger<VnPayService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        _vnpUrl = configuration["VnPay:Url"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        _vnpTmnCode = configuration["VnPay:TmnCode"] ?? "";
        _vnpHashSecret = configuration["VnPay:HashSecret"] ?? "";
        _vnpVersion = configuration["VnPay:Version"] ?? "2.1.0";
    }

    public PaymentResponse CreatePaymentUrl(Guid userId, string returnUrl, decimal amount = 150000)
    {
        try
        {
            if (string.IsNullOrEmpty(_vnpTmnCode) || string.IsNullOrEmpty(_vnpHashSecret))
            {
                return new PaymentResponse
                {
                    Success = false,
                    Message = "VNPay chưa được cấu hình. Vui lòng liên hệ admin."
                };
            }

            var transactionId = DateTime.Now.Ticks.ToString();
            var createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var expireDate = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");

            // VNPay amount is in VND * 100
            var vnpAmount = (long)(amount * 100);

            var vnpParams = new SortedList<string, string>
            {
                { "vnp_Version", _vnpVersion },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _vnpTmnCode },
                { "vnp_Amount", vnpAmount.ToString() },
                { "vnp_CreateDate", createDate },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", "127.0.0.1" },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", $"Thanh toan goi Premium GrowingTales - User {userId}" },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_TxnRef", transactionId },
                { "vnp_ExpireDate", expireDate }
            };

            // Build query string
            var queryString = new StringBuilder();
            foreach (var kvp in vnpParams)
            {
                if (queryString.Length > 0)
                    queryString.Append('&');
                queryString.Append(WebUtility.UrlEncode(kvp.Key));
                queryString.Append('=');
                queryString.Append(WebUtility.UrlEncode(kvp.Value));
            }

            // Calculate hash
            var signData = queryString.ToString();
            var vnpSecureHash = HmacSHA512(_vnpHashSecret, signData);

            // Build final URL
            var paymentUrl = $"{_vnpUrl}?{queryString}&vnp_SecureHash={vnpSecureHash}";

            _logger.LogInformation($"Created VNPay payment URL for user {userId}, transaction: {transactionId}");

            return new PaymentResponse
            {
                Success = true,
                Message = "Tạo thanh toán thành công",
                PaymentUrl = paymentUrl,
                TransactionId = transactionId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating VNPay payment URL");
            return new PaymentResponse
            {
                Success = false,
                Message = "Lỗi tạo thanh toán: " + ex.Message
            };
        }
    }

    public (bool isValid, string transactionId, string responseCode, long amount) ValidateCallback(
        IQueryCollection query)
    {
        try
        {
            var vnpParams = new SortedList<string, string>();
            
            foreach (var key in query.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpParams.Add(key, query[key].ToString());
                }
            }

            // Get secure hash from query
            var vnpSecureHash = vnpParams.GetValueOrDefault("vnp_SecureHash", "");
            vnpParams.Remove("vnp_SecureHash");
            vnpParams.Remove("vnp_SecureHashType");

            // Build sign data
            var signData = new StringBuilder();
            foreach (var kvp in vnpParams)
            {
                if (signData.Length > 0)
                    signData.Append('&');
                signData.Append(WebUtility.UrlEncode(kvp.Key));
                signData.Append('=');
                signData.Append(WebUtility.UrlEncode(kvp.Value));
            }

            // Verify hash
            var calculatedHash = HmacSHA512(_vnpHashSecret, signData.ToString());
            var isValid = vnpSecureHash.Equals(calculatedHash, StringComparison.OrdinalIgnoreCase);

            var transactionId = vnpParams.GetValueOrDefault("vnp_TxnRef", "");
            var responseCode = vnpParams.GetValueOrDefault("vnp_ResponseCode", "");
            var amountStr = vnpParams.GetValueOrDefault("vnp_Amount", "0");
            var amount = long.Parse(amountStr) / 100; // Convert back from VND * 100

            _logger.LogInformation($"VNPay callback validated: {isValid}, transaction: {transactionId}, code: {responseCode}");

            return (isValid, transactionId, responseCode, amount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating VNPay callback");
            return (false, "", "", 0);
        }
    }

    public bool IsPaymentSuccessful(string responseCode)
    {
        return responseCode == "00";
    }

    public string GetResponseMessage(string responseCode)
    {
        return responseCode switch
        {
            "00" => "Giao dịch thành công",
            "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).",
            "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.",
            "10" => "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
            "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.",
            "12" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.",
            "13" => "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP).",
            "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch",
            "51" => "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.",
            "65" => "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.",
            "75" => "Ngân hàng thanh toán đang bảo trì.",
            "79" => "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định.",
            "99" => "Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê)",
            _ => "Giao dịch thất bại"
        };
    }

    private static string HmacSHA512(string key, string data)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
