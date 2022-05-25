using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class UCountry
    {
        public int ID { get; set; }
        public string? Country { get; set; }

        public string? City { get; set; }

        public virtual ICollection<Client>? Client { get; set; }
    }
}
