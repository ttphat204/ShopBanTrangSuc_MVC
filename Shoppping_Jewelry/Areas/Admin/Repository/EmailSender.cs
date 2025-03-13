using System.Net;
using System.Net.Mail;

namespace Shoppping_Jewelry.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, // bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("trantanphat08012004@gmail.com", "iaqkxxjfgkefblxd")
            };

            // Khởi tạo MailMessage riêng biệt
            var mailMessage = new MailMessage
            {
                From = new MailAddress("trantanphat08012004@gmail.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true // Đảm bảo bật HTML
            };
            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }
    }
}