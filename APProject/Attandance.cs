using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace APProject
{
    public partial class Attendence : Form
    {
        public Attendence()
        {
            InitializeComponent();
            utoid(Login.f.tb.Text);
        }

        // ── Load employee id from username ─────────────────────────────────
        private void utoid(string username)
        {
            try
            {
                using (NpgsqlConnection con = DBHelper.GetConnection())
                {
                    con.Open();
                    string q = "SELECT id FROM account WHERE username = @u";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(q, con))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        using (NpgsqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                                textBox1.Text = dr["id"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        // ── Mark attendance as Present ─────────────────────────────────────
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (NpgsqlConnection con = DBHelper.GetConnection())
                {
                    con.Open();

                    // Check if already marked Present today
                    string checkQ = "SELECT 1 FROM attendance " +
                                    "WHERE id = @id AND attendancedate = @today AND status = 'Present' LIMIT 1";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(checkQ, con))
                    {
                        cmd.Parameters.AddWithValue("@id",    int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@today", DateTime.Today);
                        object res = cmd.ExecuteScalar();
                        if (res != null)
                        {
                            MessageBox.Show("Attendance already marked as Present.");
                            return;
                        }
                    }

                    // Try to UPDATE existing Absent row first
                    string updateQ = "UPDATE attendance SET status = 'Present' " +
                                     "WHERE id = @id AND attendancedate = @today AND status = 'Absent'";
                    int rows;
                    using (NpgsqlCommand cmd = new NpgsqlCommand(updateQ, con))
                    {
                        cmd.Parameters.AddWithValue("@id",    int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@today", DateTime.Today);
                        rows = cmd.ExecuteNonQuery();
                    }

                    if (rows > 0)
                    {
                        MessageBox.Show("Attendance marked as Present successfully!");
                        return;
                    }

                    // No row existed yet – INSERT a new Present record
                    string insertQ = "INSERT INTO attendance (id, attendancedate, status) " +
                                     "VALUES (@id, @today, 'Present') " +
                                     "ON CONFLICT (id, attendancedate) DO UPDATE SET status = 'Present'";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertQ, con))
                    {
                        cmd.Parameters.AddWithValue("@id",    int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@today", DateTime.Today);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Attendance marked as Present successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        // ── View attendance history ────────────────────────────────────────
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Rows.Clear();

                using (NpgsqlConnection con = DBHelper.GetConnection())
                {
                    con.Open();
                    string q = "SELECT id, attendancedate, status FROM attendance WHERE id = @id ORDER BY attendancedate";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(q, con))
                    {
                        cmd.Parameters.AddWithValue("@id", int.Parse(textBox1.Text));
                        using (NpgsqlDataReader dr = cmd.ExecuteReader())
                        {
                            int c = 0;
                            while (dr.Read())
                            {
                                dataGridView1.Rows.Add(dr["id"], dr["attendancedate"], dr["status"]);
                                c++;
                            }
                            textBox3.Text = c.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // ── Navigate to Salary form ────────────────────────────────────────
        private void button3_Click(object sender, EventArgs e)
        {
            Salary salary = new Salary();
            salary.Show();
            this.Hide();
        }
    }
}
