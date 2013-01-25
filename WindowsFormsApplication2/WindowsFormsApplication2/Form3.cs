using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form3 : Form
    {
        public string Coordinator_port, Printer_port;
        public Form1 form_1 = null;
        public Form3(Form1 form_1)
        {
            InitializeComponent();
            this.form_1 = form_1;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Coordinator_port = comboBox1.SelectedItem.ToString();
            //comboBox1.Text = selection_1;
            //MessageBox.Show(Coordinator_port);
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Printer_port = comboBox2.SelectedItem.ToString();
            //comboBox2.Text = selection_2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Coordinator_port = comboBox1.Text;
            Printer_port = comboBox2.Text;
            //Printer_port = null;
            if (Coordinator_port == Printer_port)
            {
                MessageBox.Show("Cannot assign same ports to both devices", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (this.form_1.Configure_ports(Coordinator_port, Printer_port) == true)
                {
                    this.Close();
                }
            }
            
        }
    }

}
