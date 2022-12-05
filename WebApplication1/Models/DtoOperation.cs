using WebApplication1.Enum;

namespace WebApplication1.Models
{
    public class DtoOperation
    {
        public string NumberAccountRecepient { get; set; }
        public string NumberAccountSending { get; set; }
        public EOperationType? OperationType { get; set; }
        public double TransferSum { get; set; }
        public double MyBalance { get; set; }
    }
}
