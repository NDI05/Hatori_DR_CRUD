using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PraktikumADO
{
    internal class DAL
    {
        static string connectionString = "Data Source=XBOOK_B14\\SQLEXPRESS;Initial Catalog=DBAkademikADO;Integrated Security=True";

        public string GetConnectionString()
        {
            return connectionString;
        }
        public void SimpanLog(string pesan)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO LogError VALUES(GETDATE(), @Pesan)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Pesan", pesan);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        SqlConnection conn = new SqlConnection(connectionString);
        SqlDataAdapter da;
        DataTable dtMahasiswa;
        DataTable dtProdi;
        public int CountMhs()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParam);

                    cmd.ExecuteNonQuery();
                    return Convert.ToInt32(outputParam.Value);
                }
            }
        }

        public DataTable GetMhs()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        dtMahasiswa = new DataTable();
                        da.Fill(dtMahasiswa);

                        return dtMahasiswa;
                    }
                }
            }
        }

        public void InsertMhs(string nim, string nama, string alamat, string jenisKelamin, DateTime tanggalLahir, string kodeProdi, byte[] foto)
        {
            SqlConnection conn = new SqlConnection(connectionString);

            conn.Open();

            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_InsertMahasiswa", conn, trans);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@pNIM", nim);
                cmd.Parameters.AddWithValue("@pNama", nama);
                cmd.Parameters.AddWithValue("@pJenisKelamin", jenisKelamin);
                cmd.Parameters.AddWithValue("@pTanggalLahir", tanggalLahir);
                cmd.Parameters.AddWithValue("@pAlamat", alamat);
                cmd.Parameters.AddWithValue("@pKodeProdi", kodeProdi);
                cmd.Parameters.AddWithValue("@pTanggalDaftar", DateTime.Now);
                cmd.Parameters.AddWithValue("@pFoto", foto);

                cmd.ExecuteNonQuery();

                SqlCommand cmdLog = new SqlCommand(@"INSERT INTO LogAktivitas(aktivitas,waktu)
                VALUES (@aktivitas, GETDATE())", conn, trans);

                cmdLog.Parameters.AddWithValue("@aktivitas", "INSERT: " + nim);
                cmdLog.ExecuteNonQuery();
                trans.Commit();
                MessageBox.Show("Data berhasil disimpan!");
            }
            catch (SqlException ex)
            {
                trans.Rollback();
                SimpanLog("ROLLBACK INSERT: " + ex.Message);
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                SimpanLog("GENERAL ERROR: " + ex.Message);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }        
        public void UpdateMhs(string nim, string nama, string alamat, string jenisKelamin, DateTime tanggalLahir, string kodeProdi, byte[] foto)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("sp_UpdateMahasiswa", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NIM", nim);
                    cmd.Parameters.AddWithValue("@Nama", nama);
                    cmd.Parameters.AddWithValue("@JenisKelamin", jenisKelamin);
                    cmd.Parameters.AddWithValue("@TanggalLahir", tanggalLahir);
                    cmd.Parameters.AddWithValue("@Alamat", alamat);
                    cmd.Parameters.AddWithValue("@KodeProdi", kodeProdi);
                    cmd.Parameters.AddWithValue("@pFoto", foto);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteMhs(string nim)
        {   
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", connection);
                cmd.Parameters.AddWithValue("@NIM", nim);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }
        public void resetData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"IF OBJECT_ID('dbo.Mahasiswa_Backup') IS NOT NULL 
                                    BEGIN 
                                        DELETE FROM dbo.Mahasiswa; 
                                        INSERT INTO dbo.Mahasiswa SELECT * FROM dbo.Mahasiswa_Backup; 
                                    END";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            
        }
        public void testInject(string nim)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // SIMULASI FATAL: Injeksi via String Concatenation. JANGAN gunakan ini di production.
                string query = "UPDATE Mahasiswa SET Nama= 'HACKED' WHERE NIM='" + nim + "'";
                SqlCommand cmd = new SqlCommand(query, connection);

                connection.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Update Berhasil");
            }
        }
        public DataTable GetMhsByNIM(string nim)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_GetMahasiswaByNIM", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@pNIM", nim);
            da = new SqlDataAdapter(cmd);

            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);

            return dtMahasiswa;
        }

        public DataTable getProdi()
        {
            if (conn.State == ConnectionState.Closed)
                { conn.Open(); }
            SqlCommand cmd = new SqlCommand("SELECT namaprodi FROM ProgramStudi", conn);
            da = new SqlDataAdapter(cmd);
            dtProdi = new DataTable();
            da.Fill(dtProdi);
            return dtProdi;
        }

        public DataTable getDataRekap(string prodi, DateTime tanggalMasuk)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("sp_Report", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inProdi", prodi);
            cmd.Parameters.AddWithValue("@inTglMsuk", tanggalMasuk.Year);
            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);
            return dtMahasiswa;
        }

        public DataTable getDataChartByTahun(DateTime thMasuk)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("sp_DashBoardByTahun", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inTglMsuk", thMasuk.Year);
            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);
            return dtMahasiswa;
        }



        public DataTable getAllDataChart()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("sp_DashBoard", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);
            return dtMahasiswa;
        }
    }
}
