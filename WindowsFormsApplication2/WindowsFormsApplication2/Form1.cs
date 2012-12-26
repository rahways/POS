using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;



namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        //public Form1 form_1 = new Form1(this);
        public Form2 form_2 = null;
        public Form3 form_3 = null;
        public Form4 form_4 = null;
        public static List<Form> list = null;

        //Thread Message_Thread = new Thread(Process_Message);
        public List<char> Rx_Buffer = new List<char>();

        public DataTable RFID_Price = new DataTable();
        public DataTable Card_Details = new DataTable();
        public int byte_gap = 0;

        public string Message_Buffer;
        //public String clone_string;
        public bool Message_Received = false;
        public string Received_Scale_ID;
        public string Received_Message_ID;
        public int Received_Message_Bytes;
        public string Received_Card_ID;
        public float Received_Weight;
        public List<RFID_Card> Card = new List<RFID_Card>();
        public List<Scale_Entity> Scales = new List<Scale_Entity>();
        public BindingSource bsource = new BindingSource();
        public bool Header_Received = false;
        List<string> temp_file = new List<string>();
        public List<string> config_file_lines = new List<string>(); //File.ReadAllLines(@"E:\Moshe_Project\test_POS\Debug\config.txt");
        public List<string> DI_List_file_lines = new List<string>(); //File.ReadAllLines(@"E:\Moshe_Project\test_POS\Debug\DI_list.csv");
        int i = 0;
        DataRow[] row_RFID_Price = new DataRow[100];
        SerialPort Coordinator_Port = new SerialPort();
        SerialPort Printer_Port = new SerialPort();
        //public string[] Item_Description = {   "Tomato",
        //                                       "Potato",
        //                                       "Onion",
        //                                       "Garlic",
        //                                       "Ginger"
        //                                   };
        
            
        public Form1()
        {
            list = new List<Form>();
            InitializeComponent();
            Initialize_Form();
            Init_toolstrip();
        }

        private void Initialize_Form()
        {
            
            Read_Configuration_Files();
            //temp[0] = "1234567898765432" + "," + "Chillies" + "," + "2.9087";
            //temp_file.Add(temp[0]);
            //temp_file.Insert(0, "Zigbee_ID,Item_ID,Unit_Price");
            //File.WriteAllLines(@"E:\Moshe_Project\test_POS\Debug\DI_list.csv", temp_file);
            //string RFID = DI_List_file_lines[1].Substring(0, 16);
            
            DefineMainWindowTableColumns();
            DefineDetailsWindowTableColumns();
            Initialize_Scales();
            Initialize_Cards();

            //#region For Testing
            //row_RFID_Price[i] = RFID_Price.NewRow();
            //row_RFID_Price[i]["Card ID"] = 0;
            //row_RFID_Price[i]["Total Price"] = 2.345;
            //RFID_Price.Rows.Add(row_RFID_Price[i++]);
            //row_RFID_Price[i] = RFID_Price.NewRow();
            //row_RFID_Price[i]["Card ID"] = 1;
            //row_RFID_Price[i]["Total Price"] = 3.456; 
            //RFID_Price.Rows.Add(row_RFID_Price[i]);
            //#endregion
            bsource.DataSource = RFID_Price;
            dataGridView1.DataSource = bsource;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Grocery stGrocery = new Grocery();
            //RFID_Card temp = new RFID_Card();
            if (e.ColumnIndex != 1)
            {
                return;
            }
            Show_details(dataGridView1.CurrentCell.Value.ToString());
        }

        private void Show_details(string card_id)
        {
            int j, card_index;
            DataRow grocery_item_row;

            form_2 = new Form2(this);
            list.Add(form_2);

            timer1.Stop();
            Card_Details.Rows.Clear();
            //card_id = dataGridView1.CurrentCell.Value.ToString();
            card_index = findCardIndex(card_id);
            if (card_index != -1)
            {
                for (j = 0; j < Card[card_index].Grocery_Item.Count; j++)
                {
                    grocery_item_row = Card_Details.NewRow();
                    grocery_item_row["S.No."] = j + 1;
                    grocery_item_row["Item Description"] = Card[card_index].Grocery_Item[j].ID;
                    grocery_item_row["Weight"] = Card[card_index].Grocery_Item[j].Weight.ToString();
                    grocery_item_row["Unit Price"] = Card[card_index].Grocery_Item[j].Unit_Price.ToString();
                    grocery_item_row["Total"] = (Card[card_index].Grocery_Item[j].Weight * Card[card_index].Grocery_Item[j].Unit_Price).ToString();
                    Card_Details.Rows.Add(grocery_item_row);
                }
                form_2.dataGridView2.DataSource = Card_Details;
                form_2.Text = card_id;
                //form
                form_2.Show();
            }

        }
        private int findCardIndex(string card_id)
        {
            for (int j = 0; j < Card.Count; j++)
            {
                if (Card[j].ID == card_id)
                {
                    return j;
                }
            }
            return -1;
        }
        private void configureCOMPortsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form_3 = new Form3(this);
            list.Add(form_3);
            form_3.comboBox1.DataSource = SerialPort.GetPortNames();
            form_3.comboBox2.DataSource = SerialPort.GetPortNames();
            form_3.Show();
        }

        //private int find_RFID_Index(string RFID_Card_Number)
        //{
        //    int j;
        //    for (j = 0; j < RFID.Total_Elements; j++)
        //    {
        //        if (RFID.Card[j].number.ToString() == RFID_Card_Number)
        //        {
        //            return j;
        //        }
        //    }
        //    return -1;
        //}
        public bool Configure_ports (string CoordinatorPort, string PrinterPort)
        {
            Coordinator_Port.PortName = CoordinatorPort;
            if (form_3.comboBox2.Enabled == true)
            {
                Printer_Port.PortName = PrinterPort;
            }
            
            if (Coordinator_Port.IsOpen == false)
            {
                Coordinator_Port.BaudRate = 115200;
                Coordinator_Port.DataBits = 8;
                Coordinator_Port.Parity = Parity.None;
                Coordinator_Port.StopBits = StopBits.One;
                Coordinator_Port.ReceivedBytesThreshold = 1;
                Coordinator_Port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                Coordinator_Port.Open();
                //Message_Thread.IsBackground = true;
                //Message_Thread.Start();
                toolStripStatusLabel1.Text = ("Coordinator: " + CoordinatorPort);
                toolStripStatusLabel1.ForeColor = Color.Green;
                toolStripStatusLabel1.BorderStyle = Border3DStyle.Sunken;
            }
            else
            {
                MessageBox.Show(CoordinatorPort.ToString() + " is already open");
            }

            if (form_3.comboBox2.Enabled == true)
            {
                if (Printer_Port.IsOpen == false)
                {
                    Printer_Port.BaudRate = 115200;
                    Printer_Port.DataBits = 8;
                    Printer_Port.Parity = Parity.None;
                    Printer_Port.StopBits = StopBits.One;
                    Printer_Port.Open();
                    //MessageBox.Show(PrinterPort.ToString() + " has been configured successfully");
                    //return true;
                    toolStripStatusLabel2.Text = ("Printer: " + PrinterPort);
                    toolStripStatusLabel2.ForeColor = Color.RoyalBlue;
                    toolStripStatusLabel2.BorderStyle = Border3DStyle.Sunken;
                    //toolStripLabel2.Text = ("Printer:" + PrinterPort);
                }
                else
                {
                    MessageBox.Show(PrinterPort.ToString() + " is already open");
                }
            }
            if (Coordinator_Port.IsOpen == true || Printer_Port.IsOpen == true)
            {
                this.disconnectAllPortsToolStripMenuItem.Enabled = true;
                this.configureCOMPortsToolStripMenuItem.Enabled = false;
            }
            if (form_3.comboBox2.Enabled == true)
            {
                if (Coordinator_Port.IsOpen == true && Printer_Port.IsOpen == true)
                {
                    //Initialize_Form();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (Coordinator_Port.IsOpen == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //return true;
        }

        private void disconnectAllPortsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Coordinator_Port.Close();
            Printer_Port.Close();
            toolStripStatusLabel1.Text = "Coordinator";
            toolStripStatusLabel1.ForeColor = Color.Black;
            toolStripStatusLabel1.BorderStyle = Border3DStyle.Raised;
            //toolStripLabel1.Text = "Coordinator:";
            toolStripStatusLabel2.Text = "Printer:";
            toolStripStatusLabel2.ForeColor = Color.Black;
            toolStripStatusLabel2.BorderStyle = Border3DStyle.Raised;
            //toolStripLabel2.Text = "Printer:";
            configureCOMPortsToolStripMenuItem.Enabled = true;
            disconnectAllPortsToolStripMenuItem.Enabled = false;
        }
        public void Init_toolstrip()
        {
            disconnectAllPortsToolStripMenuItem.Enabled = false;
            toolStripStatusLabel1.Text = "Coordinator:";
            toolStripStatusLabel1.ForeColor = Color.Black;
            toolStripStatusLabel1.BorderStyle = Border3DStyle.Raised;
            //toolStripLabel1.Text = "Coordinator:\t";
            toolStripStatusLabel2.Text = "Printer:";
            toolStripStatusLabel2.ForeColor = Color.Black;
            toolStripStatusLabel2.BorderStyle = Border3DStyle.Raised;
            //toolStripLabel2.Text = "Printer:";
        }
        private void DefineMainWindowTableColumns()
        {
            RFID_Price.Columns.Add("S.No.", typeof(int));
            RFID_Price.Columns.Add("Card ID", typeof(string));
            RFID_Price.Columns.Add("Total Price", typeof(float));
            //RFID_Price.Columns["Card ID"].ReadOnly = true;
            //RFID_Price.Columns["Total Price"].ReadOnly = true;
        }
        public void DefineDetailsWindowTableColumns()
        {
            Card_Details.Columns.Add("S.No.");
            Card_Details.Columns.Add("Item Description");
            Card_Details.Columns.Add("Weight");
            Card_Details.Columns.Add("Unit Price");
            Card_Details.Columns.Add("Total");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Coordinator_Port.IsOpen == true)
            {
                Coordinator_Port.Close();
            }
            if (Printer_Port.IsOpen == true)
            {
                Printer_Port.Close();
            }
            Application.Exit();
        }

        private void scalesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            form_4 = new Form4(this);
            list.Add(form_4);
            form_4.Show();
        }
        private void Initialize_Scales()
        {
            Scale_Entity temp_Scale = new Scale_Entity();
            for (int j = 1; j < DI_List_file_lines.Count; j++)
            {
                temp_Scale.ID = DI_List_file_lines[j].Substring(0, 16).ToUpper();
                int Grocery_name_length = DI_List_file_lines[j].IndexOfAny(new char[] { ',' }, 17);
                temp_Scale.Grocery_Item = DI_List_file_lines[j].Substring(17, Grocery_name_length - 17);
                temp_Scale.Unit_price = (float)Convert.ToDouble(DI_List_file_lines[j].Substring(Grocery_name_length + 1));
                Scales.Add(temp_Scale);
            }
        }

        private void Initialize_Cards()
        {
            for (int j = 3; j < config_file_lines.Count; j++)
            {
                Card.Add(new RFID_Card { Total_Amount = 0.0f, ID = config_file_lines[j].ToUpper() , Grocery_Item = new List<Grocery>()});
            }
        }

        private void Read_Configuration_Files()
        {
            string[] temp = File.ReadAllLines(@"DI_list.csv");
            for (int j = 0; j < temp.Length; j++)
            {
                DI_List_file_lines.Add(temp[j]);
            }
            temp = File.ReadAllLines(@"config.txt");
            for (int j = 0; j < temp.Length; j++)
            {
                config_file_lines.Add(temp[j]);
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string read_buffer;
            char [] read_array;
            read_array = sp.ReadExisting().ToCharArray();

            if (byte_gap > 3)
            {
                Rx_Buffer.Clear();
                Header_Received = false;
            }
            byte_gap = 0;
            
            Rx_Buffer.AddRange(read_array);

            if (Rx_Buffer.Count < 2)
            {
                return;
            }
            if (Header_Received == false)
            {
                if (Rx_Buffer[0] == '\r' && Rx_Buffer[1] == '\n')
                {
                    Header_Received = true;
                }
            }

            if (Header_Received == true && Rx_Buffer.Count > 3)
            {

                if (Rx_Buffer[Rx_Buffer.Count - 1] == '\n' && Rx_Buffer[Rx_Buffer.Count - 2] == '\r')
                {
                    //Rx_Buffer.CopyTo(read_array);
                    
                    read_buffer = new string(Rx_Buffer.ToArray());
                    Message_Buffer = read_buffer;
                    //Rx_Buffer.
                    Rx_Buffer.Clear();
                    Header_Received = false;
                    Message_Received = true;
                    Process_Message();
                }
                else if (Rx_Buffer.Count > 100)
                {
                    Rx_Buffer.Clear();
                }
            }
        
        }

        public void Process_Message()
        {
            int character_index;
            int startIndex = 0; // for testing
            string temp_str;
            List <char> id = new List <char>();
            if (true)
            {
                if (Message_Received == true)
                {
                    Received_Card_ID = "";
                    Received_Message_ID = "";
                    Received_Scale_ID = "";
                    Received_Weight = 0.0f;
                    Received_Message_Bytes = 0;

                    Message_Received = false;
                    //Message_Buffer.GetRange()
                    startIndex = Message_Buffer.IndexOf('U');
                    if (startIndex == -1)
                    {
                        return;
                    }
                    if ( Message_Buffer.Substring(startIndex, 6) == "UCAST:")
                    {
                        startIndex = Message_Buffer.IndexOf(':');
                        if (startIndex == -1)
                        {
                            return;
                        }
                        Received_Scale_ID = Message_Buffer.Substring(startIndex + 1, 16).ToUpper();
                        startIndex = Message_Buffer.IndexOf(',');
                        if (startIndex == -1)
                        {
                            return;
                        }
                        Received_Message_Bytes = Convert.ToInt16(Message_Buffer.Substring(startIndex + 1, 2), 16);
                        character_index = Message_Buffer.IndexOf('=');
                        if (character_index == -1)
                        {
                            return;
                        }
                        Received_Message_ID = Message_Buffer.Substring(character_index + 1, 7);
                        if (Received_Message_ID == "BT+GPRC")
                        {
                            int j;
                            for (j = 0; j < Scales.Count; j++)
                            {
                                if (Scales[j].ID == Received_Scale_ID)
                                {
                                    break;
                                }
                            }
                            if (j < Scales.Count)
                            {
                                string tx_message = "AT+UCAST:" + Received_Scale_ID + "=" + Received_Message_ID + Convert.ToUInt32((Scales[j].Unit_price * 100.0)).ToString("D") + "\r";
                                Coordinator_Port.WriteLine(tx_message);
                            }
                            
                        }
                        else if (Received_Message_ID == "BT+GBAL")
                        {
                            id = Message_Buffer.Substring(character_index + 9, Received_Message_Bytes - 8).ToList();
                            int i = 0;
                            while (i < id.Count)
                            {
                                
                                if (i > 0 && (i+1) % 3 == 0)
                                {
                                    if (id[i] != '-')
                                    {
                                        id.Insert(i - 2, '0');
                                    }
                                }
                                i++;
                            }
                            if (id.Count < 14)
                            {
                                id.Insert(12, '0');
                            }
                            id.RemoveAll(item => item == '-');
                            Received_Card_ID = new string(id.ToArray()).ToUpper();
                            int j;
                            for (j = 0; j < Card.Count; j++)
                            {
                                if (Card[j].ID == Received_Card_ID)
                                {
                                    break;
                                }
                            }
                            if (j < Card.Count)
                            {
                                string tx_message = "AT+UCAST:" + Received_Scale_ID + "=" + Received_Message_ID + Convert.ToUInt32((Card[j].Total_Amount * 100.0)).ToString() + "\r";
                                Coordinator_Port.WriteLine(tx_message);
                            }
                            
                        }
                        else if (Received_Message_ID == "BT+PCON")
                        {
                            //Predicate <char>
                            id = Message_Buffer.Substring(character_index + 9, Message_Buffer.LastIndexOf('+') - character_index - 9).ToList();
                            int i = 0;
                            while (i < id.Count)
                            {

                                if (i > 0 && (i + 1) % 3 == 0)
                                {
                                    if (id[i] != '-')
                                    {
                                        id.Insert(i - 2, '0');
                                    }
                                }
                                i++;
                            } 
                            id.RemoveAll(item => item == '-');
                            Received_Card_ID = new string(id.ToArray()).ToUpper();
                            Received_Weight = (float)Convert.ToDouble(Message_Buffer.Substring(Message_Buffer.LastIndexOf('+') + 1));
                            //Form1 form1 = new Form1();
                            Update_Card();
                            int j;
                            for (j = 0; j < Card.Count; j++)
                            {
                                if (Card[j].ID == Received_Card_ID)
                                {
                                    break;
                                }
                            }
                            if (j < Card.Count)
                            {
                                string tx_message = "AT+UCAST:" + Received_Scale_ID + "=" + Received_Message_ID + Convert.ToUInt32((Card[j].Total_Amount * 100.0)).ToString() + "\r";
                                Coordinator_Port.WriteLine(tx_message);
                            }
                            
                        }
                    }
                }
            }
        }

        private bool Update_Card()
        {
            int i = 0, j = 0;
            RFID_Card temp_Card = new RFID_Card();
            //temp_Card.Grocery_Item = new List<Grocery>();
            Grocery sttemp_Grocery = new Grocery();
            string Received_Grocery_Item = "";
            float temp_UnitPrice = 0.0f;

            for (i = 0; i < Scales.Count; i++)
            {
                if (Received_Scale_ID.ToUpper() == Scales[i].ID)
                {
                    Received_Grocery_Item = Scales[i].Grocery_Item;
                    temp_UnitPrice = Scales[i].Unit_price;
                    break;
                }
            }

            //sttemp_Grocery = {};
            i = 0;
            if (Received_Grocery_Item != "")
            {
                while (i < Card.Count)
                {
                    if (Card[i].ID == Received_Card_ID.ToUpper())
                    {
                        while (j < Card[i].Grocery_Item.Count)
                        {
                            if (Card[i].Grocery_Item[j].ID == Received_Grocery_Item)
                            {
                                // Update the item present already on the card
                                sttemp_Grocery = Card[i].Grocery_Item[j];
                                sttemp_Grocery.Weight += Received_Weight / 1000.0f;
                                Card[i].Grocery_Item.RemoveAt(j);
                                Card[i].Grocery_Item.Insert(j, sttemp_Grocery);
                                Card[i].Total_Amount += Received_Weight / 1000.0f * temp_UnitPrice;
                                return true;
                            }
                            j++;
                        }
                        // If the Grocery item does not already exist on the card
                        if (j == Card[i].Grocery_Item.Count)
                        {
                            // Add the Grocery item to the existing list on the card
                            sttemp_Grocery.ID = Received_Grocery_Item;
                            sttemp_Grocery.Weight = Received_Weight / 1000.0f;
                            sttemp_Grocery.Unit_Price = temp_UnitPrice;
                            Card[i].Grocery_Item.Add(sttemp_Grocery);
                            Card[i].Total_Amount = sttemp_Grocery.Weight * sttemp_Grocery.Unit_Price;
                            return true;
                        }
                    }
                    i++;
                }
                if (i == Card.Count)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return false;
 
        }

        public void Update_Table()
        {
            int j = 0, k = 1;
            //Form1 temp_form = new Form1();
            DataRow temp_row;
            //RFID_Price.Columns["Card ID"].ReadOnly = false;
            //RFID_Price.Columns["Total Price"].ReadOnly = false;
            RFID_Price.Rows.Clear();
            while (j < Card.Count)
            {
                if (Card[j].Total_Amount != 0.0f)
                {
                    temp_row = RFID_Price.NewRow();
                    temp_row["S.No."] = k++;
                    temp_row["Card ID"] = Card[j].ID.ToUpper();
                    temp_row["Total Price"] = Card[j].Total_Amount;
                    RFID_Price.Rows.Add(temp_row);
                }
                j++;
            }
            //RFID_Price.Columns["Card ID"].ReadOnly = true;
            //RFID_Price.Columns["Total Price"].ReadOnly = true;
            //dataGridView1.Refresh();
            //dataGridView1.DataSource = RFID_Price;
        }

        private void dataGridView1_DataSourceChanged(object sender, EventArgs e)
        {
            //dataGridView1.DataSource = typeof(List<>);
            //dataGridView1.DataSource = RFID_Price;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            byte_gap++;
            Update_Table();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Show_details(textBox1.Text);
            textBox1.Clear();
        }
    }
}
