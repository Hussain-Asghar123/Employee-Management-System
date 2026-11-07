using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APProject
{
    public partial class menu : Form
    {
        public menu()
        {
            InitializeComponent();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            Attendence f = new Attendence();
            f.Show();
            this.Hide();

        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            Job_Detail f = new Job_Detail();
            f.Show();
            this.Hide();
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            Login f = new Login();
            f.Show();
            this.Hide();

        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            Salary f = new Salary();
            f.Show();
            this.Hide();
        }
    }
}
