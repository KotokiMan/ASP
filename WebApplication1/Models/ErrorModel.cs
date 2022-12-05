using Org.BouncyCastle.Asn1.Ocsp;

namespace WebApplication1.Models
{
    public class ErrorModel
    {
        public string ErrorMessage { get; set; }

        public bool IsEmpty => !string.IsNullOrEmpty(ErrorMessage);
    }
}
