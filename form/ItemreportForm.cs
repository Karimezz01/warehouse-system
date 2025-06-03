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
    public partial class ItemreportForm: Form
    { private readonly AppDbcontext dbcontext = new AppDbcontext();

        public ItemreportForm()
        {
            InitializeComponent();
        }

        private void ItemreportForm_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = dbcontext.Items.ToList();
            comboBox1.DisplayMember = "ItemName";
            comboBox1.ValueMember = "ItemId";
            comboBox1.SelectedIndex = -1;

            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemName", HeaderText = "Item" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "WarehouseName", HeaderText = "Warehouse" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierName", HeaderText = "Supplier" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Quantity" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductionDate", HeaderText = "Production Date" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ExpiryDate", HeaderText = "Expiry Date" });

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an item first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedItemId = (int)comboBox1.SelectedValue;

            try
            {
                var reportData = dbcontext.Inventory
                                    .Include(i => i.Warehouse)
                                    .Include(i => i.Item)
                                    .Include(i => i.Supplier)
                                    .Where(i => i.ItemID == selectedItemId)
                                    .Select(i => new
                                    {
                                        ItemName = i.Item.Name,
                                        WarehouseName = i.Warehouse.Name,
                                        SupplierName = i.Supplier.Name,
                                        Quantity = i.Quantity,
                                        ProductionDate = i.ProductionDate.HasValue ? i.ProductionDate.Value.ToString("MM/dd/yyyy") : "N/A",
                                        ExpiryDate = i.ExpiryDate.HasValue ? i.ExpiryDate.Value.ToString("MM/dd/yyyy") : "N/A"
                                    })
                                    .ToList();

                dataGridView1.DataSource = reportData;

                if (!reportData.Any())
                {
                    MessageBox.Show("No inventory records found for the selected item.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}