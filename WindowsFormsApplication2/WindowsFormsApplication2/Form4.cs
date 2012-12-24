using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class Form4 : Form
    {
        DataTable Scale_Table = new DataTable();
        DataTable RFID_Card_Table = new DataTable();
        public Form1 form_1 = null;
        public Form4(Form1 form_1)
        {
            InitializeComponent();
            this.form_1 = form_1;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            Load_Scale_Table();
            Load_RFID_Card_Table();
        }

        private void Load_Scale_Table()
        {
            tabControl1.Visible = true;
            tabControl3.Visible = false;
            Scale_Table.Columns.Add("S.No.");
            Scale_Table.Columns.Add("Scale ID");
            Scale_Table.Columns.Add("Grocery Associated");
            Scale_Table.Columns.Add("Unit Price");
            
            for (int i = 0; i < form_1.Scales.Count; i++)
            {
                Scale_Table.Rows.Add();
                Scale_Table.Rows[i]["S.No."] = (i + 1).ToString();
                Scale_Table.Rows[i]["Scale ID"] = form_1.Scales[i].ID;
                Scale_Table.Rows[i]["Grocery Associated"] = form_1.Scales[i].Grocery_Item;
                Scale_Table.Rows[i]["Unit Price"] = form_1.Scales[i].Unit_price;
            }
            Scale_Table.Columns["S.No."].ReadOnly = true;
            Scale_Table.Columns["Scale ID"].ReadOnly = true;
            Scale_Table.Columns["Grocery Associated"].ReadOnly = true;
            Scale_Table.Columns["Unit Price"].ReadOnly = true;

            dataGridView1.DataSource = Scale_Table;
        }

        private void Load_RFID_Card_Table()
        {
            RFID_Card_Table.Columns.Add("S.No.");
            RFID_Card_Table.Columns.Add("RFID Card");
            for (int i = 0; i < form_1.Card.Count; i++)
            {
                RFID_Card_Table.Rows.Add();
                RFID_Card_Table.Rows[i]["S.No."] = (i + 1).ToString();
                RFID_Card_Table.Rows[i]["RFID Card"] = form_1.Card[i].ID;
            }
            RFID_Card_Table.Columns["S.No."].ReadOnly = true;
            RFID_Card_Table.Columns["RFID Card"].ReadOnly = true;

            dataGridView2.DataSource = RFID_Card_Table;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            int j;
            bool scale_found = false;
            Scale_Entity temp_Scale = new Scale_Entity();
            if (textBox1.Text == "")
            {
                MessageBox.Show("Scale ID should be a non-zero value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            temp_Scale.ID = textBox1.Text.ToUpper();
            if (radioButton2.Checked == false)
            {
                if (textBox2.Text == "" || textBox3.Text == "")
                {
                    MessageBox.Show("Grocery name and unit price should be non-zero values", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                temp_Scale.Grocery_Item = textBox2.Text;
                temp_Scale.Unit_price = (float)Convert.ToDouble(textBox3.Text);
            }

            for (j = 0; j < form_1.Scales.Count; j++)
            {
                if (form_1.Scales[j].ID == temp_Scale.ID)
                {
                    if (radioButton2.Checked == true)
                    {
                        form_1.Scales.RemoveAt(j);
                        form_1.DI_List_file_lines.RemoveAt(j + 1);
                        //File.WriteAllLines(@"E:\Moshe_Project\test_POS\Debug\DI_list.csv", form_1.DI_List_file_lines);
                        MessageBox.Show("The Scale with ID: " + temp_Scale.ID + " has been removed successfully.", "Remove", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        form_1.Scales.RemoveAt(j);
                        form_1.Scales.Insert(j, temp_Scale);
                        form_1.DI_List_file_lines.RemoveAt(j + 1);
                        form_1.DI_List_file_lines.Insert(j + 1, temp_Scale.ID + ',' + temp_Scale.Grocery_Item + ',' + temp_Scale.Unit_price.ToString());
                        //File.WriteAllLines(@"E:\Moshe_Project\test_POS\Debug\DI_list.csv", form_1.DI_List_file_lines);
                        MessageBox.Show("The Scale with ID: " + temp_Scale.ID + " has been modified successfully.", "Modify", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    File.WriteAllLines(@"E:\Moshe_Project\test_POS\Debug\DI_list.csv", form_1.DI_List_file_lines);
                    scale_found = true;
                    break;
                }
            }
            if (scale_found == false)
            {
                if (radioButton2.Checked == true)
                {
                    MessageBox.Show("The Scale with ID: " + temp_Scale.ID + " does not exist.", "Remove", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    form_1.Scales.Add(temp_Scale);
                    form_1.DI_List_file_lines.Add(temp_Scale.ID + ',' + temp_Scale.Grocery_Item + ',' + temp_Scale.Unit_price.ToString());
                    File.WriteAllLines(@"E:\Moshe_Project\test_POS\Debug\DI_list.csv", form_1.DI_List_file_lines);
                    MessageBox.Show("The Scale with ID: " + temp_Scale.ID + " has been successfully added.", "Add", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Visible = false;
            textBox3.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Visible = true;
            textBox3.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Text == "Details")
            {
                Scale_Table.Columns["S.No."].ReadOnly = false;
                Scale_Table.Columns["Scale ID"].ReadOnly = false;
                Scale_Table.Columns["Grocery Associated"].ReadOnly = false;
                Scale_Table.Columns["Unit Price"].ReadOnly = false;

                for (int i = 0; i < form_1.Scales.Count; i++)
                {
                    if (Scale_Table.Rows.Count <= i)
                    {
                        Scale_Table.Rows.Add();
                    }
                    Scale_Table.Rows[i]["S.No."] = (i + 1).ToString();
                    Scale_Table.Rows[i]["Scale ID"] = form_1.Scales[i].ID;
                    Scale_Table.Rows[i]["Grocery Associated"] = form_1.Scales[i].Grocery_Item;
                    Scale_Table.Rows[i]["Unit Price"] = form_1.Scales[i].Unit_price;
                }

                while (Scale_Table.Rows.Count > form_1.Scales.Count)
                {
                    Scale_Table.Rows.RemoveAt(Scale_Table.Rows.Count - 1);
                }
                Scale_Table.Columns["S.No."].ReadOnly = true;
                Scale_Table.Columns["Scale ID"].ReadOnly = true;
                Scale_Table.Columns["Grocery Associated"].ReadOnly = true;
                Scale_Table.Columns["Unit Price"].ReadOnly = true;

            }
            if (e.TabPage.Text == "Add/Remove")
            {
                radioButton1.Checked = true;
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            tabControl1.Visible = false;
            tabControl3.Visible = true;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            tabControl3.Visible = false;
            tabControl1.Visible = true;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int j;
            bool card_found = false;
            RFID_Card temp_Card = new RFID_Card();
            //temp_Card.Grocery_Item = new List<Grocery>();

            if (textBox4.Text == "")
            {
                MessageBox.Show("Card ID should be a non-zero value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            temp_Card.ID = textBox4.Text.ToUpper();

            for (j = 0; j < form_1.Card.Count; j++)
            {
                if (form_1.Card[j].ID == temp_Card.ID)
                {
                    if (radioButton7.Checked == true)
                    {
                        form_1.Card.RemoveAt(j);
                        form_1.config_file_lines.RemoveAt(j + 3);
                        File.WriteAllLines(@"E:\Moshe_Project\test_POS\Debug\config.txt", form_1.config_file_lines);
                        MessageBox.Show("The Card with ID: " + temp_Card.ID + " has been removed successfully.", "Remove", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("The Card with ID: " + temp_Card.ID + " already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                    card_found = true;
                    break;
                }
            }
            if (card_found == false)
            {
                if (radioButton7.Checked == true)
                {
                    MessageBox.Show("The Card with ID: " + temp_Card.ID + " does not exist.", "Remove", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    temp_Card.Total_Amount = 0.0f;
                    temp_Card.Grocery_Item.Clear();
                    form_1.Card.Add(temp_Card);
                    form_1.config_file_lines.Add(temp_Card.ID);
                    File.WriteAllLines(@"E:\Moshe_Project\test_POS\Debug\config.txt", form_1.config_file_lines);
                    MessageBox.Show("The Card with ID: " + temp_Card.ID + " has been added successfully.", "Add", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            textBox4.Text = "";
        }

        private void tabControl3_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Text == "Details")
            {
                RFID_Card_Table.Columns["S.No."].ReadOnly = false;
                RFID_Card_Table.Columns["RFID Card"].ReadOnly = false;

                for (int i = 0; i < form_1.Card.Count; i++)
                {
                    if (RFID_Card_Table.Rows.Count <= i)
                    {
                        RFID_Card_Table.Rows.Add();
                    }
                    RFID_Card_Table.Rows[i]["S.No."] = (i + 1).ToString();
                    RFID_Card_Table.Rows[i]["RFID Card"] = form_1.Card[i].ID;
                }

                while (RFID_Card_Table.Rows.Count > form_1.Card.Count)
                {
                    RFID_Card_Table.Rows.RemoveAt(RFID_Card_Table.Rows.Count - 1);
                }
                RFID_Card_Table.Columns["S.No."].ReadOnly = true;
                RFID_Card_Table.Columns["RFID Card"].ReadOnly = true;
            }
            if (e.TabPage.Text == "Add/Remove")
            {
                radioButton6.Checked = true;
                textBox4.Text = "";
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int CurrentColumIndex = dataGridView1.HitTest(e.X, e.Y).ColumnIndex;
                if (CurrentColumIndex == 3)
                {
                    contextMenuStrip1.Show(dataGridView1, new Point(e.X, e.Y));
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Scale_Table.Columns["Unit Price"].ReadOnly = false;
            dataGridView1.Columns["Unit Price"].ReadOnly = false;
        }
    }
}
