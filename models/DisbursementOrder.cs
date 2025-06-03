using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehousesystem.models
{
    public class DisbursementOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DisbursementOrderID { get; set; }

        [Required(ErrorMessage = "number is required")]
        [MaxLength(50)]
        public string OrderNumber { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public int WarehouseID { get; set; }
        [Required]
        public int SupplierID { get; set; }
        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; }

        [ForeignKey("SupplierID")]
        public Supplier Supplier { get; set; }

        public virtual ICollection<DisbursementOrderDetail> DisbursementOrderDetails { get; set; }

        public DisbursementOrder()
        {
            DisbursementOrderDetails = new HashSet<DisbursementOrderDetail>();
        }
    
}
}
