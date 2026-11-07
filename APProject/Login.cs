using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace APProject
{
    public partial class Login : Form
    {
        public TextBox tb;
        public static Login f;

        public Login()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
            tb = textBox1;
            f  = this;
            At();
        }

        bool isvalid = false;

        // ── Register button (go to Form1 / employee registration) ──────────
        private void button1_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
            this.Hide();
        }

        // ── Auto-insert today's attendance rows (Absent) if not yet done ───
        private void At()
        {
            string attendanceDate = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                using (NpgsqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();

                    // Check if attendance for today already exists
                    string checkQ = "SELECT 1 FROM attendance WHERE attendancedate = @AttendanceDate LIMIT 1";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(checkQ, conn))
                    {
                        cmd.Parameters.AddWithValue("@AttendanceDate", DateTime.Parse(attendanceDate));
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            return; // already done today
                    }

                    // Get all employee IDs
                    string empQ = "SELECT id FROM account";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(empQ, conn))
                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        var ids = new System.Collections.Generic.List<string>();
                        while (dr.Read())
                            ids.Add(dr["id"].ToString());
                        dr.Close();

                        foreach (string id in ids)
                            insert(id, attendanceDate);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void insert(string employeeId, string attendanceDate)
        {
            try
            {
                using (NpgsqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string q = "INSERT INTO attendance (id, attendancedate, status) " +
                               "VALUES (@EmployeeId, @AttendanceDate, @Status) " +
                               "ON CONFLICT (id, attendancedate) DO NOTHING";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId",    int.Parse(employeeId));
                        cmd.Parameters.AddWithValue("@AttendanceDate", DateTime.Parse(attendanceDate));
                        cmd.Parameters.AddWithValue("@Status",        "Absent");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Login button ───────────────────────────────────────────────────
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) &&
                string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please Enter Username and Password");
                return;
            }

            try
            {
                using (NpgsqlConnection con = DBHelper.GetConnection())
                {
                    con.Open();
                    string q = "SELECT * FROM account WHERE username = @u AND password = @p";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(q, con))
                    {
                        cmd.Parameters.AddWithValue("@u", textBox1.Text);
                        cmd.Parameters.AddWithValue("@p", textBox2.Text);

                        using (NpgsqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                MessageBox.Show("Login Successful");
                                string designation = dr["designation"].ToString().ToUpper();

                                if (designation == "ADMIN" || designation == "CEO")
                                {
                                    menu f1 = new menu();
                                    f1.Show();
                                }
                                else
                                {
                                    Attendence f1 = new Attendence();
                                    f1.Show();
                                }
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid Username and Password");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // ── Show / hide password ───────────────────────────────────────────
        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (!isvalid)
            {
                textBox2.PasswordChar = '\0';
                isvalid = true;
            }
            else
            {
                textBox2.PasswordChar = '*';
                isvalid = false;
            }
        }
    }
}
