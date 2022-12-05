using MimeKit;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApplication1.Helper
{
    public class RegHelper
    {
        public bool IsEmail(string Email)
        {
            return Regex.IsMatch(Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        public string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        public void EmailValidate(string email, string code)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Bank","alexandernegutarov@yandex.ru"));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = "Код подтверждения";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = code;
            message.Body = bodyBuilder.ToMessageBody();


            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.yandex.ru", 465, true);
                client.Authenticate("alexandernegutarov@yandex.ru", "+Sashaneg27+");
                client.Send(message);
                client.Disconnect(true);
            }

        }
    }
}
