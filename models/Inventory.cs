using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehousesystem.models
{
 public   class Inventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InventoryID { get; set; }

        [Required]
        public int WarehouseID { get; set; }

        [Required]
        public int ItemID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        public DateTime? ProductionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [Required] 
        public int SupplierID { get; set; }

        
        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; }

        [ForeignKey("ItemID")]
        public Item Item { get; set; }

        [ForeignKey("SupplierID")]
        public Supplier Supplier { get; set; }
    }


}
