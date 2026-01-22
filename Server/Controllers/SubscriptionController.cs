using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

public class SubscriptionController : Controller
{
    private readonly SubscriptionService _subscriptionService;
    private readonly VnPayService _vnPayService;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(
        SubscriptionService subscriptionService,
        VnPayService vnPayService,
        ILogger<SubscriptionController> logger)
    {
        _subscriptionService = subscriptionService;
        _vnPayService = vnPayService;
        _logger = logger;
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }

    // GET: /Subscription/Pricing
    public async Task<IActionResult> Pricing()
    {
        var userId = GetCurrentUserId();
        var limits = await _subscriptionService.GetUserLimitsAsync(userId);
        SubscriptionDto? subscription = null;

        if (userId.HasValue)
        {
            subscription = await _subscriptionService.GetActiveSubscription(userId.Value);
        }

        ViewBag.UserLimits = limits;
        ViewBag.ActiveSubscription = subscription;

        return View();
    }

    // POST: /Subscription/Upgrade
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult Upgrade()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account", new { returnUrl = "/Subscription/Pricing" });
        }

        var returnUrl = $"{Request.Scheme}://{Request.Host}/Subscription/PaymentCallback";
        var result = _vnPayService.CreatePaymentUrl(userId.Value, returnUrl, PlanLimits.Premium.MonthlyPrice);

        if (result.Success && !string.IsNullOrEmpty(result.PaymentUrl))
        {
            // Store transaction ID in session for verification
            HttpContext.Session.SetString("PendingTransaction", result.TransactionId ?? "");
            return Redirect(result.PaymentUrl);
        }

        TempData["ErrorMessage"] = result.Message;
        return RedirectToAction("Pricing");
    }

    // GET: /Subscription/PaymentCallback
    public async Task<IActionResult> PaymentCallback()
    {
        var (isValid, transactionId, responseCode, amount) = _vnPayService.ValidateCallback(Request.Query);

        if (!isValid)
        {
            _logger.LogWarning("Invalid VNPay callback signature");
            TempData["ErrorMessage"] = "Chữ ký không hợp lệ";
            return RedirectToAction("PaymentResult", new { success = false });
        }

        var isSuccess = _vnPayService.IsPaymentSuccessful(responseCode);
        var message = _vnPayService.GetResponseMessage(responseCode);

        if (isSuccess)
        {
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                try
                {
                    await _subscriptionService.CreateSubscription(userId.Value, transactionId, amount);
                    _logger.LogInformation($"Subscription created for user {userId}, transaction: {transactionId}");
                    
                    TempData["SuccessMessage"] = "Thanh toán thành công! Gói Premium đã được kích hoạt.";
                    return RedirectToAction("PaymentResult", new { success = true });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating subscription");
                    TempData["ErrorMessage"] = "Thanh toán thành công nhưng có lỗi khi kích hoạt gói. Vui lòng liên hệ hỗ trợ.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng. Vui lòng đăng nhập và thử lại.";
            }
        }
        else
        {
            TempData["ErrorMessage"] = message;
        }

        return RedirectToAction("PaymentResult", new { success = false });
    }

    // GET: /Subscription/PaymentResult
    public IActionResult PaymentResult(bool success)
    {
        ViewBag.Success = success;
        return View();
    }
}
