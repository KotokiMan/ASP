using WebApplication1.Enum;

namespace WebApplication1.Models
{
    public class DtoReg
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
        public string Code { get; set; }
        public EAccessLevel Role { get; set; }
    }
}
