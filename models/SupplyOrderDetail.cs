using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehousesystem.models
{
   public class SupplyOrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplyOrderDetailID { get; set; }

        [Required]
        public int SupplyOrderID { get; set; }

        [Required]
        public int ItemID { get; set; } 

        [Required(ErrorMessage = " .")]
        [Column(TypeName = "decimal(18,2)")] 
        public decimal Quantity { get; set; }

        public DateTime? ProductionDate { get; set; } 
        public DateTime? ExpiryDate { get; set; } 

        
        [ForeignKey("SupplyOrderID")]
        public SupplyOrder SupplyOrder { get; set; }

        [ForeignKey("ItemID")]
        public Item Item { get; set; }
    }

}
