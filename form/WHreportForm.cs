using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using warehousesystem.data;

namespace warehousesystem.forms
{
    public partial class WHreportForm: Form
    {
private readonly AppDbcontext dbcontext = new AppDbcontext();
        public WHreportForm()
        {
            InitializeComponent();
        }

        private void WHreportForm_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = dbcontext.Warehouses.ToList();
            comboBox1.DisplayMember = "WarehouseName";
            comboBox1.ValueMember = "WarehouseId";
            comboBox1.SelectedIndex = -1;

            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "WarehouseName", HeaderText = "Warehouse" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemName", HeaderText = "Item" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierName", HeaderText = "Supplier" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Quantity" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductionDate", HeaderText = "Production Date" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ExpiryDate", HeaderText = "Expiry Date" });

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }

      

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a warehouse first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedWarehouseId = (int)comboBox1.SelectedValue;

            try
            {
                var reportData = dbcontext.Inventory
                                    .Include(i => i.Warehouse)
                                    .Include(i => i.Item)
                                    .Include(i => i.Supplier)
                                    .Where(i => i.WarehouseID == selectedWarehouseId)
                                    .Select(i => new
                                    {
                                        WarehouseName = i.Warehouse.Name,
                                        ItemName = i.Item.Name,
                                        SupplierName = i.Supplier.Name,
                                        Quantity = i.Quantity,
                                        ProductionDate = i.ProductionDate.HasValue ? i.ProductionDate.Value.ToString("MM/dd/yyyy") : "N/A",
                                        ExpiryDate = i.ExpiryDate.HasValue ? i.ExpiryDate.Value.ToString("MM/dd/yyyy") : "N/A"
                                    })
                                    .ToList();

                dataGridView1.DataSource = reportData;

                if (!reportData.Any())
                {
                    MessageBox.Show("No inventory records found for the selected warehouse.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        
    }
    }
}