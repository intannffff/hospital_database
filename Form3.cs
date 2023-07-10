using MySqlConnector;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Hospital
{
    public partial class Form3 : Form
    {
        private Form2 form2;

        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.form2 = new Form2();
            form2.Show();
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                UserID = "root",
                Password = "root",
                Database = "hospital",
            };

            using var con = new MySqlConnection(builder.ConnectionString);
            con.Open();
            MySqlCommand dataCommand = new MySqlCommand("SELECT * FROM daftar_pasien", con);
            MySqlDataReader dataReader = dataCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataGridView1.DataSource = dataTable;
            con.Close();
        }

        private void search_data_pasien(String keyword)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                UserID = "root",
                Password = "root",
                Database = "hospital",
            };

            using var con = new MySqlConnection(builder.ConnectionString);

            con.Open();
            MySqlCommand dataCommand = new MySqlCommand("SELECT * FROM daftar_pasien WHERE `Nama Lengkap` LIKE @keyword", con);
            dataCommand.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
            MySqlDataReader dataReader = dataCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataGridView1.DataSource = dataTable;
            con.Close();
        }

        private void load_data_pasien()
        {

            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                UserID = "root",
                Password = "root",
                Database = "hospital",
            };

            using var con = new MySqlConnection(builder.ConnectionString);
            con.Open();
            MySqlCommand dataCommand = new MySqlCommand("SELECT * FROM daftar_pasien", con);
            MySqlDataReader dataReader = dataCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataGridView1.DataSource = dataTable;
            con.Close();
        }
        private void clear_form()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            dateTimePicker1.ResetText();
            radioButton1.Checked = true;
            radioButton2.Checked = false;
        }

        private void load_single_pasien(int patient_id)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                UserID = "root",
                Password = "root",
                Database = "hospital",
            };

            using var con = new MySqlConnection(builder.ConnectionString);
            con.Open();
            MySqlCommand dataCommand = new MySqlCommand("SELECT * FROM patients WHERE id = @patient_id", con);
            dataCommand.Parameters.AddWithValue("@patient_id", patient_id);
            MySqlDataReader dataReader = dataCommand.ExecuteReader();

            if (dataReader.HasRows == true)
            {
                dataReader.Read();
                textBox1.Text = dataReader.GetString(dataReader.GetOrdinal("fullname"));
                textBox2.Text = dataReader.GetString(dataReader.GetOrdinal("nik"));
                var fulldate = dataReader.GetDateOnly("birth_date").ToString("yyyy-MM-dd").Split("-");

                dateTimePicker1.Value = new DateTime(Int32.Parse(fulldate[0]), Int32.Parse(fulldate[1]), Int32.Parse(fulldate[2]));
                textBox4.Text = dataReader.GetString(dataReader.GetOrdinal("birth_place"));
                if (dataReader.GetString(dataReader.GetOrdinal("sex")) == "M")
                {
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                }
                else
                {
                    radioButton1.Checked = false;
                    radioButton2.Checked = true;
                }

                textBox5.Text = dataReader.GetString(dataReader.GetOrdinal("occupation"));
            }
            else
            {
                MessageBox.Show("Data Tidak Ditemukan", "Perhatian");
            }
            con.Close();
        }

        private void drop_single_pasien(int patient_id)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                UserID = "root",
                Password = "root",
                Database = "hospital",
            };

            using var con = new MySqlConnection(builder.ConnectionString);

            con.Open();
            MySqlCommand dataCommand = new MySqlCommand("DELETE FROM patients WHERE id = @patient_id", con);
            try
            {
                dataCommand.Parameters.AddWithValue("@patient_id", patient_id);
                dataCommand.ExecuteNonQuery();
                MessageBox.Show("Berhasil menghapus data", "Berhasil");
            }
            catch (Exception e)
            {
                MessageBox.Show("Gagal menghapus data. Pasien pernah melakukan pemeriksaan (checkup).", "Perhatian");
            }
            con.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            search_data_pasien(textBox6.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            clear_form();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var rowIndex = dataGridView1.CurrentCell.RowIndex;
            var patient_id = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();
            textBox3.Text = patient_id;
            load_single_pasien(Int16.Parse(patient_id));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                UserID = "root",
                Password = "root",
                Database = "hospital",
            };

            using var con = new MySqlConnection(builder.ConnectionString);

            var nama_lengkap = textBox1.Text;
            var nik = textBox2.Text;
            var tgl_lahir = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            var tmp_lahir = textBox4.Text;
            var pekerjaan = textBox5.Text;

            var gender = "";
            bool isChecked = radioButton1.Checked;
            if (isChecked)
            {
                gender = "M";
            }
            else
            {
                gender = "F";
            }

            con.Open();
            MySqlCommand dataCommand;
            if (textBox3.Text == "")
            {
                dataCommand = new MySqlCommand("INSERT INTO patients (fullname, nik, birth_date, birth_place, sex, occupation) VALUE (@fullname, @nik, @birth_date, @birth_place, @sex, @occupation)", con);
            }
            else
            {
                dataCommand = new MySqlCommand("UPDATE patients SET fullname = @fullname, nik = @nik, birth_date = @birth_date, birth_place = @birth_place, sex = @sex, occupation = @occupation WHERE id = @patient_id", con);
                dataCommand.Parameters.AddWithValue("@patient_id", textBox3.Text);
            }
            dataCommand.Parameters.AddWithValue("@fullname", nama_lengkap);
            dataCommand.Parameters.AddWithValue("@nik", nik);
            dataCommand.Parameters.AddWithValue("@birth_date", tgl_lahir);
            dataCommand.Parameters.AddWithValue("@birth_place", tmp_lahir);
            dataCommand.Parameters.AddWithValue("@sex", gender);
            dataCommand.Parameters.AddWithValue("@occupation", pekerjaan);
            int affected_rows = dataCommand.ExecuteNonQuery();
            if (affected_rows > 0)
            {
                MessageBox.Show("Berhasil menyimpan data", "Informasi");
            }
            con.Close();
            clear_form();
            load_data_pasien();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            var answer = MessageBox.Show("Anda yakin ingin menghapus data ini?", "Perhatian", MessageBoxButtons.YesNo);
            if (answer == DialogResult.Yes)
            {
                var rowIndex = dataGridView1.CurrentCell.RowIndex;
                var patient_id = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();
                drop_single_pasien(Int16.Parse(patient_id));
                load_data_pasien();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Membuat komponen DataTable
            // Dimulai dari baris di bawah, komponen pada dataGridView1 dipindahkan ke komponen baru yaitu DataTable pada variabel dt
            DataTable dt = new DataTable();
            // Menambahkan kolom dataGridView1 ke DataTable
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                dt.Columns.Add(col.Name);
            }

            // Menambahkan baris/record data dari dataGridView1 ke DataTable
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataRow dRow = dt.NewRow();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    dRow[cell.ColumnIndex] = cell.Value;
                }
                dt.Rows.Add(dRow);
            }

            // Membuat sebuah file Excel di memory
            using (XLWorkbook wb = new XLWorkbook())
            {
                // Menambahkan worksheet ke file Excel
                wb.Worksheets.Add(dt, "Sheet1");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    // Membuat save file dialog untuk menentukan tempat penyimpanan file
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel Files|*.xlsx";
                    saveFileDialog.FileName = "Rekap Data Pasien.xlsx";

                    // Melakukan cek apakah user menekan tombol simpan
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Menyimpan file dengan nama dan lokasi yang dipilih oleh user
                        File.WriteAllBytes(saveFileDialog.FileName, stream.ToArray());
                        MessageBox.Show("File saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}