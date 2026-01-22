using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Server.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string? _smtpHost;
    private readonly int _smtpPort;
    private readonly string? _smtpUsername;
    private readonly string? _smtpPassword;
    private readonly string? _fromEmail;
    private readonly string? _fromName;
    private readonly bool _enableSsl;
    private readonly string? _baseUrl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        _smtpHost = configuration["Email:SmtpHost"];
        _smtpPort = configuration.GetValue<int>("Email:SmtpPort", 587);
        _smtpUsername = configuration["Email:SmtpUsername"];
        _smtpPassword = configuration["Email:SmtpPassword"];
        _fromEmail = configuration["Email:FromEmail"] ?? configuration["Email:SmtpUsername"];
        _fromName = configuration["Email:FromName"] ?? "GrowingTales";
        _enableSsl = configuration.GetValue<bool>("Email:EnableSsl", true);
        _baseUrl = configuration["AppSettings:BaseUrl"] ?? configuration["Email:BaseUrl"] ?? "https://localhost:5001";
    }

    public bool IsConfigured()
    {
        return !string.IsNullOrWhiteSpace(_smtpHost) 
            && !string.IsNullOrWhiteSpace(_smtpUsername) 
            && !string.IsNullOrWhiteSpace(_smtpPassword);
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
    {
        if (!IsConfigured())
        {
            _logger.LogWarning("Email service is not configured. Email not sent to {Email}", toEmail);
            return false;
        }

        try
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = _enableSsl,
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail!, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml,
                Priority = MailPriority.Normal
            };

            message.To.Add(toEmail);

            await client.SendMailAsync(message);
            
            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string email)
    {
        var resetUrl = $"{_baseUrl}/Account/ResetPassword?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(email)}";
        
        var subject = "Đặt lại mật khẩu - GrowingTales";
        var body = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Đặt lại mật khẩu</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0; font-size: 28px;'>
            <i class='bi bi-book' style='margin-right: 10px;'></i>GrowingTales
        </h1>
    </div>
    
    <div style='background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e0e0e0;'>
        <h2 style='color: #667eea; margin-top: 0;'>Đặt lại mật khẩu</h2>
        
        <p>Xin chào,</p>
        
        <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản <strong>{email}</strong> của bạn.</p>
        
        <p>Vui lòng nhấn vào nút bên dưới để đặt lại mật khẩu:</p>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{resetUrl}' 
               style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                      color: white; 
                      padding: 15px 30px; 
                      text-decoration: none; 
                      border-radius: 5px; 
                      display: inline-block; 
                      font-weight: bold;
                      box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                Đặt lại mật khẩu
            </a>
        </div>
        
        <p>Hoặc copy và dán link sau vào trình duyệt:</p>
        <p style='background: #fff; padding: 15px; border-radius: 5px; border: 1px solid #ddd; word-break: break-all;'>
            <a href='{resetUrl}' style='color: #667eea;'>{resetUrl}</a>
        </p>
        
        <p style='color: #666; font-size: 14px;'>
            <strong>Lưu ý:</strong> Link này chỉ có hiệu lực trong <strong>24 giờ</strong>. 
            Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.
        </p>
        
        <hr style='border: none; border-top: 1px solid #e0e0e0; margin: 30px 0;'>
        
        <p style='color: #999; font-size: 12px; margin: 0;'>
            Email này được gửi tự động, vui lòng không trả lời.<br>
            © 2024 GrowingTales. All rights reserved.
        </p>
    </div>
</body>
</html>";

        return await SendEmailAsync(toEmail, subject, body, isHtml: true);
    }
}
