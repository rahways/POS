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
    public partial class Form2 : Form
    {
        public Form1 form_1 = null;
        //public static Form2 Form2Ref { get; private set; }
        public Form2(Form1 form_1)
        {
            
            InitializeComponent();
            //Form2Ref = this;
            this.form_1 = form_1;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            form_1.timer1.Start();
        }


    }
}
