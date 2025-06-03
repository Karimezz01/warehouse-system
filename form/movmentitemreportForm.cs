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

namespace warehousesystem.forms
{
    public partial class movmentitemreportForm : Form
    {
        private readonly AppDbcontext dbcontext = new AppDbcontext();
        public movmentitemreportForm()
        {
            InitializeComponent();
        }

        private void movmentitemreportForm_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = dbcontext.Items.ToList();
            comboBox1.DisplayMember = "ItemName";
            comboBox1.ValueMember = "ItemId";
            comboBox1.SelectedIndex = -1;

            dateTimePicker1.Value = DateTime.Now.AddMonths(-1);
            dateTimePicker2.Value = DateTime.Now;

            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MovementDate", HeaderText = "Date" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MovementType", HeaderText = "Type" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemName", HeaderText = "Item" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Quantity" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RelatedEntity", HeaderText = "Related Party / Location" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderOrTransferNumber", HeaderText = "Ref. No." });

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
            DateTime startDate = dateTimePicker1.Value.Date;
            DateTime endDate = dateTimePicker2.Value.Date.AddDays(1).AddTicks(-1);

            if (startDate > endDate)
            {
                MessageBox.Show("Start Date cannot be after End Date.", "Date Range Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var allMovements = new List<dynamic>(); // Changed from List<object> to List<dynamic>

                var supplyMovements = dbcontext.SupplyOrderDetails
                                        .Include(soi => soi.SupplyOrder)
                                            .ThenInclude(so => so.Warehouse)
                                        .Include(soi => soi.SupplyOrder)
                                            .ThenInclude(so => so.Supplier)
                                        .Include(soi => soi.Item)
                                        .Where(soi => soi.ItemID == selectedItemId &&
                                                      soi.SupplyOrder.OrderDate >= startDate &&
                                                      soi.SupplyOrder.OrderDate <= endDate)
                                        .Select(soi => new
                                        {
                                            MovementDate = soi.SupplyOrder.OrderDate,
                                            MovementType = "Supply In",
                                            ItemName = soi.Item.Name,
                                            Quantity = soi.Quantity,
                                            RelatedEntity = $"From: {soi.SupplyOrder.Supplier.Name} To: {soi.SupplyOrder.Warehouse.Name}",
                                            OrderOrTransferNumber = soi.SupplyOrder.OrderNumber
                                        })
                                        .ToList();
                allMovements.AddRange(supplyMovements);

                var disbursementMovements = dbcontext.DisbursementOrderDetails
                                            .Include(doi => doi.DisbursementOrder)
                                                .ThenInclude(d => d.Warehouse)
                                            .Include(doi => doi.DisbursementOrder)
                                                .ThenInclude(d => d.Supplier)
                                            .Include(doi => doi.Item)
                                            .Where(doi => doi.ItemID == selectedItemId &&
                                                          doi.DisbursementOrder.OrderDate >= startDate &&
                                                          doi.DisbursementOrder.OrderDate <= endDate)
                                            .Select(doi => new
                                            {
                                                MovementDate = doi.DisbursementOrder.OrderDate,
                                                MovementType = "Disbursement Out",
                                                ItemName = doi.Item.Name,
                                                Quantity = doi.Quantity,
                                                RelatedEntity = $"From: {doi.DisbursementOrder.Warehouse.Name} To: {doi.DisbursementOrder.Supplier.Name}",
                                                OrderOrTransferNumber = doi.DisbursementOrder.OrderNumber
                                            })
                                            .ToList();
                allMovements.AddRange(disbursementMovements);

                var transferMovements = dbcontext.TransferItems
                                        .Include(it => it.Item)
                                        .Include(it => it.SourceWarehouse)
                                        .Include(it => it.DestinationWarehouse)
                                        .Where(it => it.ItemID == selectedItemId &&
                                                      it.TransferDate >= startDate &&
                                                      it.TransferDate <= endDate)
                                        .Select(it => new
                                        {
                                            MovementDate = it.TransferDate,
                                            MovementType = "Transfer",
                                            ItemName = it.Item.Name,
                                            Quantity = it.Quantity,
                                            RelatedEntity = $"From: {it.SourceWarehouse.Name} To: {it.DestinationWarehouse.Name}",
                                            OrderOrTransferNumber = $"Transfer ID: {it.TransferID}"
                                        })
                                        .ToList();
                allMovements.AddRange(transferMovements);

                var sortedMovements = allMovements.OrderBy(m => m.MovementDate).ToList();

                dataGridView1.DataSource = sortedMovements;

                if (!sortedMovements.Any())
                {
                    MessageBox.Show("No inventory movements found for the selected item in the specified date range.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading movement report: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
