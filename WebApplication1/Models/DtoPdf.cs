using WebApplication1.Enum;

namespace WebApplication1.Models
{
    public class DtoPdf
    {
        public string AccountNumber { get; set; }
        public string AccountNumberSend { get; set; }
        public double Summa { get; set; }
        public EOperationType OperationType { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string FullName => LastName + " " + Name + " " + SecondName;
        public double balance { get; set; }
    }
}
