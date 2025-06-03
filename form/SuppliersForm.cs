using System;
using System.Linq;
using System.Windows.Forms;
using warehousesystem.data;
using warehousesystem.models;

namespace warehousesystem.forms
{
    public partial class SuppliersForm : Form
    {
        private readonly AppDbcontext dbcontext;
        private Supplier selectedSupplier;

        public SuppliersForm()
        {
            InitializeComponent();
            dbcontext = new AppDbcontext();
        }

        private void SuppliersForm_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            RefreshSupplierGrid();
            ClearForm();
        }

        private void ClearForm()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            comboBox1.SelectedIndex = -1;
            selectedSupplier = null;
        }

        private void RefreshSupplierGrid()
        {
            dataGridView1.DataSource = dbcontext.Suppliers.Select(s => new
            {
                s.SupplierID,
                s.Name,
                s.Phone,
                s.Fax,
                s.Mobile,
                s.Email,
                s.Website
            }).ToList();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadSuppliers()
        {
            var suppliers = dbcontext.Suppliers.ToList();

            comboBox1.DataSource = null;
            comboBox1.DataSource = suppliers;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "SupplierID";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1) return;

            selectedSupplier = comboBox1.SelectedItem as Supplier;

            if (selectedSupplier != null)
            {
                textBox1.Text = selectedSupplier.Name;
                textBox2.Text = selectedSupplier.Phone;
                textBox3.Text = selectedSupplier.Fax;
                textBox4.Text = selectedSupplier.Mobile;
                textBox5.Text = selectedSupplier.Email;
                textBox6.Text = selectedSupplier.Website;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;

            var newSupplier = new Supplier
            {
                Name = textBox1.Text,
                Phone = textBox2.Text,
                Fax = textBox3.Text,
                Mobile = textBox4.Text,
                Email = textBox5.Text,
                Website = textBox6.Text
            };

            dbcontext.Suppliers.Add(newSupplier);
            dbcontext.SaveChanges();

            MessageBox.Show("Supplier added successfully.");
            RefreshSupplierGrid();
            LoadSuppliers();
            ClearForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedSupplier == null)
            {
                MessageBox.Show("Please select a supplier to update.");
                return;
            }

            if (!ValidateFields()) return;

            selectedSupplier.Name = textBox1.Text;
            selectedSupplier.Phone = textBox2.Text;
            selectedSupplier.Fax = textBox3.Text;
            selectedSupplier.Mobile = textBox4.Text;
            selectedSupplier.Email = textBox5.Text;
            selectedSupplier.Website = textBox6.Text;

            dbcontext.SaveChanges();
            MessageBox.Show("Supplier updated successfully.");
            RefreshSupplierGrid();
            LoadSuppliers();
            ClearForm();
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return false;
            }

            return true;
        }
    }
}

