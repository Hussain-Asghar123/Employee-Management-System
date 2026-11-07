using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace APProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true;
            comboBox1.SelectedIndex = 0;
            textBox10.PasswordChar = '*';
            textBox11.PasswordChar = '*';
        }

        bool isvalid  = false;
        bool isvalid1 = false;

        // ── Unused label / groupBox stubs (kept for designer compatibility) ─
        private void label3_Click(object sender, EventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { }
        private void groupBox3_Enter(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }

        // ── Validate form fields & username uniqueness ─────────────────────
        private bool IsValidData()
        {
            try
            {
                using (NpgsqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string q = "SELECT 1 FROM account WHERE username = @Username LIMIT 1";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", textBox12.Text);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            MessageBox.Show("Username already exists", "Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Error: " + ex.Message);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox7.Text) ||
                string.IsNullOrWhiteSpace(textBox8.Text) ||
                string.IsNullOrWhiteSpace(textBox12.Text))
            {
                MessageBox.Show("Name and username are required", "Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox1.Text) &&
                string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Contact detail required", "Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox10.Text) ||
                textBox10.Text != textBox11.Text)
            {
                MessageBox.Show("Passwords must match and cannot be empty", "Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox3.Text) &&
                string.IsNullOrWhiteSpace(textBox4.Text) &&
                string.IsNullOrWhiteSpace(textBox5.Text) &&
                string.IsNullOrWhiteSpace(textBox6.Text) &&
                string.IsNullOrWhiteSpace(textBox9.Text))
            {
                MessageBox.Show("Address is required", "Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        // ── Create employee account ────────────────────────────────────────
        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsValidData())
                return;

            try
            {
                string address = $"{textBox3.Text},{textBox4.Text},{textBox5.Text},{textBox6.Text},{textBox9.Text}";
                string gender  = radioButton1.Checked ? radioButton1.Text : radioButton2.Text;

                string q = "INSERT INTO account " +
                           "(fname, lname, gender, marrige_status, dob, email, phno, " +
                           " password, address, username, designation, status, salary) " +
                           "VALUES (@fn, @ln, @gn, @ms, @dob, @em, @ph, " +
                           "        @pw, @ad, @un, 'employee', 'contract', 50000)";

                using (NpgsqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@fn",  textBox7.Text);
                        cmd.Parameters.AddWithValue("@ln",  textBox8.Text);
                        cmd.Parameters.AddWithValue("@gn",  gender);
                        cmd.Parameters.AddWithValue("@ms",  comboBox1.SelectedItem?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@dob", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@em",  textBox1.Text);
                        cmd.Parameters.AddWithValue("@ph",  textBox2.Text);
                        cmd.Parameters.AddWithValue("@pw",  textBox10.Text);
                        cmd.Parameters.AddWithValue("@ad",  address);
                        cmd.Parameters.AddWithValue("@un",  textBox12.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Account created successfully", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e) { }

        // ── Password visibility toggles ────────────────────────────────────
        private void iconButton2_Click(object sender, EventArgs e)
        {
            if (!isvalid1)
            {
                textBox11.PasswordChar = '\0';
                isvalid1 = true;
            }
            else
            {
                textBox11.PasswordChar = '*';
                isvalid1 = false;
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (!isvalid)
            {
                textBox10.PasswordChar = '\0';
                isvalid = true;
            }
            else
            {
                textBox10.PasswordChar = '*';
                isvalid = false;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Hide();
        }
    }
}
