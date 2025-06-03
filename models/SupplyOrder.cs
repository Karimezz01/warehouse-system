using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehousesystem.models
{
   public class SupplyOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplyOrderID { get; set; }

        [Required(ErrorMessage = "id is required")]
        [MaxLength(50)]
        public string OrderNumber { get; set; }

        [Required(ErrorMessage = "date is reauired.")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = ".")]
        public int WarehouseID { get; set; } 

        [Required(ErrorMessage = ".")]
        public int SupplierID { get; set; }

        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; }

        [ForeignKey("SupplierID")]
        public Supplier Supplier { get; set; }

        public virtual ICollection<SupplyOrderDetail> SupplyOrderDetails { get; set; }

        public SupplyOrder()
        {
            SupplyOrderDetails = new HashSet<SupplyOrderDetail>();
        }
    }
}
