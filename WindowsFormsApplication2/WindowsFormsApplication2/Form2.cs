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

        private void print_invoice_Click(object sender, EventArgs e)
        {
            print_invoice_func();
            clear_card_record();
        }

        private void print_invoice_func()
        {
            float total_weight = 0.0f, total_amount = 0.0f;
            form_1.Printer_Port.WriteLine("\rFRUIT CENTER\rKroonstraat 18\r2018 Antwerpen");
            form_1.Printer_Port.WriteLine("\r\r\r\r");
            form_1.Printer_Port.WriteLine("item     $/kg     kg     $\r");
            form_1.Printer_Port.WriteLine("------------------------------\r");
            for (int i = 0; i < form_1.Card_Details.Rows.Count; i++)
            {
                form_1.Printer_Port.WriteLine(form_1.Card_Details.Rows[i]["Item Description"].ToString());
                form_1.Printer_Port.WriteLine("    "+form_1.Card_Details.Rows[i]["Unit Price"]);
                form_1.Printer_Port.WriteLine("    " + form_1.Card_Details.Rows[i]["Weight"]);
                total_weight += Convert.ToSingle(form_1.Card_Details.Rows[i]["Weight"]);
                form_1.Printer_Port.WriteLine("    " + form_1.Card_Details.Rows[i]["Total"] + "\r");
                total_amount += Convert.ToSingle(form_1.Card_Details.Rows[i]["Total"]);
            }
            form_1.Printer_Port.WriteLine("------------------------------\r");
            form_1.Printer_Port.WriteLine("items     kg     total\r");
            form_1.Printer_Port.WriteLine("------------------------------\r");
            form_1.Printer_Port.WriteLine(form_1.Card_Details.Rows.Count.ToString() + "    ");
            form_1.Printer_Port.WriteLine(total_weight.ToString() + "    ");
            form_1.Printer_Port.WriteLine(total_amount.ToString() + "\r");
            form_1.Printer_Port.WriteLine("------------------------------\r");
            form_1.Printer_Port.WriteLine("Thank you\r");
        }

        private void clear_card_record()
        {
            int card_index = form_1.findCardIndex(this.Text);
            form_1.Card[card_index].Grocery_Item.Clear();
            form_1.Card[card_index].Total_Amount = 0.0f;
        }
    }
}
