using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UserList
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string SecondName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string IIN { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }
        public string? AccountNumber { get; set; }
        public double? Balance { get; set; }
    }
}
