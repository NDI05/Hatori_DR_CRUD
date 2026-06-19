using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PraktikumADO
{
    public partial class Form2 : Form
    {

        DAL dbLogic = new DAL();
        SqlDataAdapter da;
        DataTable dtMahasiswa;
        DataTable dtProdi;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dtpTanggalMasuk.Format = DateTimePickerFormat.Custom;
            dtpTanggalMasuk.CustomFormat = "yyyy-MM-dd";
            dtpTanggalMasuk.ShowUpDown = true;
            dtpTanggalMasuk.MinDate = new DateTime(2000, 1, 1);
            dtpTanggalMasuk.MaxDate = DateTime.Today;

            cmbProdi.DropDownStyle = ComboBoxStyle.DropDownList;

            btnCetak.Enabled = false;
            try
            {
                dtProdi = dbLogic.getProdi();
                cmbProdi.DataSource = dtProdi;
                cmbProdi.DisplayMember = "namaprodi";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Load Data:" + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dtMahasiswa = dbLogic.getDataRekap(cmbProdi.Text, dtpTanggalMasuk.Value);

                dataGridView1.DataSource = dtMahasiswa;

                if (dtMahasiswa.Rows.Count > 0)
                {
                    btnCetak.Enabled = true;
                }
                else
                {
                    btnCetak.Enabled = false;
                    MessageBox.Show("Data tidak ditemukan");
                }
            }
            catch (Exception ex)
            {
            MessageBox.Show("Gagal Load data: " + ex.Message);
            }
        }
    public string Nama {  get; set; }
    public string JenisKelamin { get; set; }
    public string Alamat { get; set; }
    public string NamaProdi { get; set; }
    public DateTime TanggalDaftar { get; set; }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            Form3 fm3 = new Form3(cmbProdi.Text.ToString(), dtpTanggalMasuk.Value);
            fm3.Show();
            this.Hide();
        }
    }


}

