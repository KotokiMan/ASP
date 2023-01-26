using Microsoft.CodeAnalysis.CSharp.Syntax;
using MimeKit;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static WebApplication1.Helper.RegHelper;

namespace WebApplication1.Helper
{
    public class RegHelper
    {
        private static int lengthPass { get { return lenght < 8 ? 8 : lenght; } }
        public static int lenght { get; set; }
        const string Digits = "0123456789";
        const string Alphabet = "abcdefghijklmnopqrstuvwxyz";
        const string Symbols = " ~`@#$%^&*()_+-=[]{};'\\:\"|,./<>?";

        [Flags]
        public enum PasswordChars
        {
            Digits = 0b0001,
            Alphabet = 0b0010,
            Symbols = 0b0100
        }

        public string CreateRandomPassword(int l = 8)
        {
            PasswordChars passwordChars = (PasswordChars)7;
            var random = new Random();
            var resultPassword = new StringBuilder(lengthPass);
            var passwordCharSet = string.Empty;
            if (passwordChars.HasFlag(PasswordChars.Alphabet))
            {
                passwordCharSet += Alphabet + Alphabet.ToUpper();
            }
            if (passwordChars.HasFlag(PasswordChars.Digits))
            {
                passwordCharSet += Digits;
            }
            if (passwordChars.HasFlag(PasswordChars.Symbols))
            {
                passwordCharSet += Symbols;
            }
            for (var i = 0; i < lengthPass; i++)
            {
                resultPassword.Append(passwordCharSet[random.Next(0, passwordCharSet.Length)]);
            }
            resultPassword.Append("PoZ5*");
            return resultPassword.ToString();
        }
        public bool IsEmail(string Email)
        {
            return Regex.IsMatch(Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
        public bool IsPass(string Pass)
        {
            return Regex.IsMatch(Pass, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$");
        }

        public string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }

        public void EmailValidate(string email, string code, string pass = "")
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Bank","alexandernegutarov@yandex.ru"));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = "Код подтверждения";
            var bodyBuilder = new BodyBuilder();
            if(pass=="")
            {
                bodyBuilder.HtmlBody = code;
            }
            else
            {
                bodyBuilder.HtmlBody = code + "\nYour password:\t"+pass;
            }
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
