using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Enum;

namespace WebApplication1.Models
{
    [Table("UserAuthBank")]
    public class UserAuthBank
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public EAccessLevel AccessLevel { get; set; }
        public bool IsEmailValidate { get; set; } = false;
        public string ValidateCode { get; set; }        
    }
}
