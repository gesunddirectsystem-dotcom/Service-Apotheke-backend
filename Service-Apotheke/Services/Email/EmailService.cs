using System.Net;
using System.Net.Mail;

namespace Service_Apotheke.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendConfirmationEmail(string toEmail, string confirmationCode)
        {
            // إعدادات الإيميل من appsettings.json
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);

            // إعداد SmtpClient
            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = enableSsl
            };

            // إنشاء رسالة البريد
            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, "Service Apotheke"),
                Subject = "Confirm Your Email - Service Apotheke",
                IsBodyHtml = true, // مهم عشان HTML يشتغل
                Body = $@"
        <html>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <style>
                body {{ font-family: Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 0; }}
                .container {{ max-width: 600px; margin: 40px auto; background: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1); }}
                .header {{ font-size: 24px; font-weight: bold; color: #333333; margin-bottom: 20px; text-align: center; }}
                .message {{ font-size: 16px; color: #555555; line-height: 1.5; }}
                .code {{ font-size: 20px; font-weight: bold; color: #007bff; background: #f0f8ff; padding: 10px 15px; border-radius: 5px; display: inline-block; margin: 20px 0; }}
                .footer {{ font-size: 12px; color: #888888; margin-top: 30px; text-align: center; }}
                @media only screen and (max-width: 600px) {{
                    .container {{ padding: 15px; }}
                    .header {{ font-size: 20px; }}
                    .code {{ font-size: 18px; }}
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>Welcome to Service Apotheke!</div>
                <div class='message'>
                    Thank you for registering. Please use the confirmation code below to verify your email address:
                </div>
                <div class='code'>{confirmationCode}</div>
                <div class='message'>
                    If you did not sign up for this account, please ignore this email.
                </div>
                <div class='footer'>© {DateTime.UtcNow.Year} Service Apotheke. All rights reserved.</div>
            </div>
        </body>
        </html>"
            };

            // إضافة المستلم
            mailMessage.To.Add(toEmail);

            // إرسال البريد
            await client.SendMailAsync(mailMessage);
        }


        public async Task SendApplicationNotification(string toEmail, string pharmacistName, DateTime shiftDate)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = enableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = "New Shift Application - Service Apotheke",
                Body = $"Pharmacist {pharmacistName} has applied for your shift on {shiftDate:yyyy-MM-dd}.\n\nPlease log in to your account to review the application.",
                IsBodyHtml = false
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
