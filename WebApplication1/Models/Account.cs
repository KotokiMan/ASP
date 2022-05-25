using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string? AccountNumber { get; set; }
        public double Balance { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]

        public virtual Client? Client { get; set; }
    }
}
