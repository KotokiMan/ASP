using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Enum;

namespace WebApplication1.Models
{
    [Table("BankOperationHistory")]
    public class BankOperationHistiry
    {
        public int Id { get; set; }
        public string? NumberAccountRecepient { get; set; }
        public string? NumberAccountSending { get; set; }
        public EOperationType operationType { get; set; }
        public double TransferSum { get; set; }
    }
}
