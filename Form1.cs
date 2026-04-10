using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace PraktikumADO
{
    public partial class Form1 : Form
    {

        private readonly SqlConnection conn;
        private readonly string connectionString = "Data Source=XBOOK_B14\\SQLEXPRESS;Initial Catalog=DBAkademikADO;Integrated Security=True";
        public Form1()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
        }

        private void ConnectDatabase()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    MessageBox.Show("Koneksi berhasil!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi gagal: " + ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConnectDatabase();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add("NIM", "NIM");
                dataGridView1.Columns.Add("Nama", "Nama");
                dataGridView1.Columns.Add("JenisKelamin", "Jenis Kelamin");
                dataGridView1.Columns.Add("TanggalLahir", "Tanggal Lahir");
                dataGridView1.Columns.Add("Alamat", "Alamat");
                dataGridView1.Columns.Add("KodeProdi", "KodeProdi");

                string query = "SELECT * FROM Mahasiswa";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dataGridView1.Rows.Add(
                        reader["NIM"].ToString(),
                        reader["Nama"].ToString(),
                        reader["JenisKelamin"].ToString(),
                        Convert.ToDateTime(reader["TanggalLahir"]).ToShortDateString(),
                        reader["alamat"].ToString(),
                        reader["KodeProdi"].ToString()
                        );
                }

                reader.Close();
            }


            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (textBox1.Text == "")
                {
                    MessageBox.Show("NIM tidak boleh kosong.");
                    return;
                }
                if (textBox5.Text == "")
                {
                    MessageBox.Show("Nama tidak boleh kosong.");
                    return;
                }
                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Jenis Kelamin harus dipilih.");
                    return;
                }
                if (textBox3.Text == "")
                {
                    MessageBox.Show("Alamat tidak boleh kosong.");
                    return;
                }
                if (textBox2.Text == "")
                {
                    MessageBox.Show("Kode Prodi tidak boleh kosong.");
                    return;
                }
                string query = "INSERT INTO Mahasiswa (NIM, Nama, JenisKelamin, TanggalLahir, Alamat, KodeProdi) VALUES (@NIM, @Nama, @JenisKelamin, @TanggalLahir, @Alamat, @KodeProdi)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NIM", textBox1.Text);
                cmd.Parameters.AddWithValue("@Nama", textBox5.Text);
                cmd.Parameters.AddWithValue("@JenisKelamin", comboBox1.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@TanggalLahir", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@Alamat", textBox3.Text);
                cmd.Parameters.AddWithValue("@KodeProdi", textBox2.Text);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Data berhasil ditambahkan!");
                    
                    button2.PerformClick(); // Refresh data grid view
                }
                else
                {
                    MessageBox.Show("Gagal menambahkan data.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
