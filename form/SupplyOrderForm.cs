using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using warehousesystem.data;
using warehousesystem.models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace warehousesystem.forms
{
    public partial class SupplyOrderForm : Form
    {
        private readonly AppDbcontext dbcontext = new AppDbcontext();
        public SupplyOrderForm()
        {
            InitializeComponent();
        }

        private void SupplyOrderForm_Load(object sender, EventArgs e)
        {
            // تعبئة ComboBoxes
            comboBox1.DataSource = dbcontext.Warehouses.ToList();
            comboBox1.DisplayMember = "WarehouseName";
            comboBox1.ValueMember = "WarehouseId";    
            comboBox1.SelectedIndex = -1;

            comboBox2.DataSource = dbcontext.Suppliers.ToList();
            comboBox2.DisplayMember = "SupplierName"; 
            comboBox2.ValueMember = "SupplierId";    
            comboBox2.SelectedIndex = -1;

            comboBox3.DataSource = dbcontext.Items.ToList();
            comboBox3.DisplayMember = "ItemName"; 
            comboBox3.ValueMember = "ItemId";     
            comboBox3.SelectedIndex = -1;
           dataGridView1.Columns.Clear(); 
            dataGridView1.Columns.Add("ItemId", "Item ID");
            dataGridView1.Columns.Add("ItemName", "Item Name");
            dataGridView1.Columns.Add("Quantity", "Quantity");
            dataGridView1.Columns.Add("ProductionDate", "Production Date");
            dataGridView1.Columns.Add("ExpiryDate", "Expiry Date"); 
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; 
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false; 
            dataGridView1.ReadOnly = true; 

            
            GenerateNewOrderNumber();
            ClearForm();
        }

        private void GenerateNewOrderNumber()
        {
            textBox1.Text = $"SO-{DateTime.Now.ToString("yyyyMMdd-HHmmss")}";
        }

  
     

        private void ClearForm() // 
        {
            textBox1.Clear(); 
            textBox2.Clear(); 
            
             textBox3.Clear(); 

            dateTimePicker1.Value = DateTime.Now; // تاريخ الأمر
            dateTimePicker2.Value = DateTime.Now; // تاريخ الإنتاج
            dateTimePicker3.Value = DateTime.Now; // تاريخ انتهاء الصلاحية

            comboBox1.SelectedIndex = -1; // 
            comboBox2.SelectedIndex = -1; 
            comboBox3.SelectedIndex = -1;

            checkBox1.Checked = false; 

            dataGridView1.Rows.Clear(); 
            GenerateNewOrderNumber(); 
        }

      

     

        private void button1_Click_1(object sender, EventArgs e)
        {  if (comboBox3.SelectedIndex == -1 || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Please select an item and enter a quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var item = (Item)comboBox3.SelectedItem; 
            decimal quantity;

            if (!decimal.TryParse(textBox2.Text, out quantity) || quantity <= 0) {
                MessageBox.Show("Please enter a valid positive quantity.", "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime productionDate = dateTimePicker2.Value.Date;
            DateTime? expiryDate = checkBox1.Checked ? (DateTime?)null : dateTimePicker3.Value.Date;

            if (!checkBox1.Checked && productionDate >= expiryDate)
            {
                MessageBox.Show("Production date must be before the expiry date.", "Date Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dataGridView1.Rows.Add(
                item.ItemID,
                item.Name,
                quantity,
                productionDate.ToString("MM/dd/yyyy"), // تنسيق التاريخ للعرض
                expiryDate?.ToString("MM/dd/yyyy") ?? "N/A" 
            );

            // مسح حقول إدخال الصنف
            comboBox3.SelectedIndex = -1;
            textBox2.Clear();
            dateTimePicker2.Value = DateTime.Now; // إعادة ضبط تاريخ الإنتاج
            dateTimePicker3.Value = DateTime.Now; // إعادة ضبط تاريخ انتهاء الصلاحية
            checkBox1.Checked = false; // إلغاء تحديد "بدون تاريخ انتهاء صلاحية"

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please Enter an order number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboBox1.SelectedIndex == -1) // التحقق من اختيار المخزن
            {
                MessageBox.Show("Please select a warehouse.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboBox2.SelectedIndex == -1) // التحقق من اختيار المورد
            {
                MessageBox.Show("Please select a supplier.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool hasOrderItems = dataGridView1.Rows.Cast<DataGridViewRow>().Any(r => !r.IsNewRow);
            if (!hasOrderItems)
            {
                MessageBox.Show("Please add at least one item to the order.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // الحصول على بيانات رأس الأمر
            string orderNumber = textBox1.Text.Trim();
            int warehouseId = (int)comboBox1.SelectedValue;
            int supplierId = (int)comboBox2.SelectedValue;

            
            bool orderExists = dbcontext.SupplyOrders.Any(o => o.OrderNumber == orderNumber);
            if (orderExists)
            {
                MessageBox.Show("This order number already exists. Please use a unique order number.", "Duplicate Order Number", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var order = new SupplyOrder
            {
                OrderNumber = orderNumber,
                OrderDate = dateTimePicker1.Value.Date,
                WarehouseID = warehouseId,
                SupplierID = supplierId,
             
                SupplyOrderDetails = new List<SupplyOrderDetail>() // تهيئة قائمة التفاصيل
            };

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue; 

                var itemId = Convert.ToInt32(row.Cells["ItemId"].Value);
                var quantity = Convert.ToDecimal(row.Cells["Quantity"].Value);
                var productionDate = DateTime.Parse(row.Cells["ProductionDate"].Value.ToString());

                string expiryString = row.Cells["ExpiryDate"].Value.ToString();
                DateTime? expiryDate = expiryString == "N/A" ? (DateTime?)null : DateTime.Parse(expiryString);

                var orderItem = new SupplyOrderDetail
                {
                    ItemID = itemId,
                    Quantity = quantity,
                    ProductionDate = productionDate,
                    ExpiryDate = expiryDate
                };
                order.SupplyOrderDetails.Add(orderItem);

                var existingInventory = dbcontext.Inventory.FirstOrDefault(inv =>
                     inv.WarehouseID == warehouseId &&
                     inv.ItemID == itemId &&
                     inv.SupplierID == supplierId &&
                     inv.ProductionDate == productionDate &&
                     inv.ExpiryDate == expiryDate
                );

                if (existingInventory != null)
                {
                    existingInventory.Quantity += quantity;

                }
                else
                {
                
                    dbcontext.Inventory.Add(new Inventory
                    {
                        WarehouseID = warehouseId,
                        ItemID = itemId,
                        SupplierID = supplierId,
                        Quantity = quantity,
                        ProductionDate = productionDate,
                        ExpiryDate = expiryDate,

                    });
                }
            }

            try
            {
                dbcontext.SupplyOrders.Add(order);
                dbcontext.SaveChanges();

                MessageBox.Show("Order Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving order: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            dateTimePicker3.Enabled = !checkBox1.Checked;
        }
    }
}
