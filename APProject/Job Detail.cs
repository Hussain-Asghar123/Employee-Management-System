using System;
using System.Windows.Forms;
using Npgsql;

namespace APProject
{
    public partial class Job_Detail : Form
    {
        public Job_Detail()
        {
            InitializeComponent();
        }

        // ── Search employee by ID ──────────────────────────────────────────
        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter an employee ID.");
                return;
            }

            try
            {
                using (NpgsqlConnection con = DBHelper.GetConnection())
                {
                    con.Open();
                    string q = "SELECT fname, lname, designation, salary, status FROM account WHERE id = @id";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(q, con))
                    {
                        cmd.Parameters.AddWithValue("@id", int.Parse(textBox1.Text));
                        using (NpgsqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                textBox2.Text = dr["fname"].ToString() + " " + dr["lname"].ToString();
                                textBox3.Text = dr["designation"].ToString();
                                textBox4.Text = dr["salary"].ToString();

                                if (dr["status"].ToString().ToUpper() == "CONTRACT")
                                    radioButton2.Checked = true;
                                else
                                    radioButton1.Checked = true;
                            }
                            else
                            {
                                MessageBox.Show("Employee not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ── Update employee job details ────────────────────────────────────
        private void button2_Click(object sender, EventArgs e)
        {
            string status = radioButton1.Checked ? radioButton1.Text : radioButton2.Text;

            try
            {
                using (NpgsqlConnection con = DBHelper.GetConnection())
                {
                    con.Open();
                    string q = "UPDATE account SET designation = @des, salary = @sal, status = @st WHERE id = @id";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(q, con))
                    {
                        cmd.Parameters.AddWithValue("@des", textBox3.Text);
                        cmd.Parameters.AddWithValue("@sal", decimal.Parse(textBox4.Text));
                        cmd.Parameters.AddWithValue("@st",  status);
                        cmd.Parameters.AddWithValue("@id",  int.Parse(textBox1.Text));
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Record Updated");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        // ── Delete employee ────────────────────────────────────────────────
        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this employee?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes)
                return;

            try
            {
                using (NpgsqlConnection con = DBHelper.GetConnection())
                {
                    con.Open();
                    string q = "DELETE FROM account WHERE id = @id";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(q, con))
                    {
                        cmd.Parameters.AddWithValue("@id", int.Parse(textBox1.Text));
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Record Deleted");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Job_Detail_Load(object sender, EventArgs e) { }

        // ── Navigate back to menu ──────────────────────────────────────────
        private void button1_Click(object sender, EventArgs e)
        {
            menu m = new menu();
            m.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
