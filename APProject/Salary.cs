using System;
using System.Windows.Forms;
using FontAwesome.Sharp;
using Npgsql;

namespace APProject
{
    public partial class Salary : Form
    {
        public Salary()
        {
            InitializeComponent();
            utoid(Login.f.tb.Text);
        }

        // ── Load employee data from username ───────────────────────────────
        private void utoid(string username)
        {
            try
            {
                using (NpgsqlConnection con = DBHelper.GetConnection())
                {
                    con.Open();
                    string q = "SELECT id, salary, status FROM account WHERE username = @u";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(q, con))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        using (NpgsqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                textBox3.Text = dr["id"].ToString();
                                textBox1.Text = dr["salary"].ToString();

                                if (dr["status"].ToString().ToUpper() == "CONTRACT")
                                    radioButton2.Checked = true;
                                else
                                    radioButton1.Checked = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void Salary_Load(object sender, EventArgs e) { }

        // ── Calculate salary ───────────────────────────────────────────────
        private void iconButton1_Click(object sender, EventArgs e)
        {
            int     employeeId    = Convert.ToInt32(textBox3.Text);
            decimal perDaySalary  = Convert.ToDecimal(textBox1.Text);
            int     selectedMonth = dateTimePicker1.Value.Month;
            int     selectedYear  = dateTimePicker1.Value.Year;

            CalculateSalary(employeeId, perDaySalary, selectedMonth, selectedYear);
        }

        public void CalculateSalary(int employeeId, decimal perDaySalary, int month, int year)
        {
            try
            {
                using (NpgsqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();

                    // PostgreSQL uses EXTRACT(MONTH FROM ...) instead of MONTH()
                    string query = @"
                        SELECT COUNT(*) AS Present
                        FROM attendance
                        WHERE id = @EmployeeID
                          AND status = 'Present'
                          AND EXTRACT(MONTH FROM attendancedate) = @Month
                          AND EXTRACT(YEAR  FROM attendancedate) = @Year";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        cmd.Parameters.AddWithValue("@Month",      month);
                        cmd.Parameters.AddWithValue("@Year",       year);

                        int presentDays = Convert.ToInt32(cmd.ExecuteScalar());
                        textBox5.Text = presentDays.ToString();

                        decimal totalSalary = presentDays * perDaySalary;
                        textBox6.Text = totalSalary.ToString("F2");

                        MessageBox.Show(
                            $"Employee ID: {employeeId}\n" +
                            $"Present Days: {presentDays}\n" +
                            $"Total Salary for {month}/{year}: {totalSalary:F2}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }

        // ── Navigate back to Attendance form ───────────────────────────────
        private void iconButton2_Click(object sender, EventArgs e)
        {
            Attendence f1 = new Attendence();
            f1.Show();
            this.Hide();
        }
    }
}
