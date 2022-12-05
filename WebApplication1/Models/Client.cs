using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    [Table("Client")]
    public class Client
    {
        public int ID { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]        
        public string? LastName { get; set; }
        [Required]
        public string? SecondName { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public string? IIN { get; set; }
        
        
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public int? CountryInfoKey { get; set; }
        [ForeignKey("CountryInfoKey")]
        public virtual UCountry? Country { get; set; }
        public virtual Account? Account { get; set; }
        public int UserAuthBankId { get; set; }
    }
}
