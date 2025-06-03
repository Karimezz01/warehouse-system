using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehousesystem.models
{
  public  class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = " name is required.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        [MaxLength(50)]
        public string Fax { get; set; }

        [MaxLength(50)]
        public string Mobile { get; set; }

        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "invalid email")]
        public string Email { get; set; }

        [MaxLength(255)]
        public string Website { get; set; }
    }
}
    
   