using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehousesystem.models
{
  public  class DisbursementOrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DisbursementOrderDetailID { get; set; }

        [Required]
        public int DisbursementOrderID { get; set; } 

        [Required]
        public int ItemID { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        
        [ForeignKey("DisbursementOrderID")]
        public DisbursementOrder DisbursementOrder { get; set; }

        [ForeignKey("ItemID")]
        public Item Item { get; set; }
    
}
}
