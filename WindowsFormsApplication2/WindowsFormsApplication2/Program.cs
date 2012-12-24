using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Data;

namespace WindowsFormsApplication2
{

    //public struct RFID_Card_Entity
    //{
    //    public float Total_Amount;
    //    public string ID;
    //    public List<Grocery> Grocery_Item;
    //    public struct Grocery
    //    {
    //        public string ID;
    //        public float Weight;
    //        public float Unit_price;
    //        public Grocery(string _id, float _weight, float _unit_price)
    //        {
    //            ID = _id;
    //            Weight = _weight;
    //            Unit_price = _unit_price;
    //        }
    //    };
    //    public float total()
    //    {
    //        int index = 0;
    //        while (index < this.Grocery_Item.Count)
    //        {
    //            this.Total_Amount += this.Grocery_Item[index].Weight * this.Grocery_Item[index].Unit_price;
    //            index++;
    //        }
    //        return this.Total_Amount;
    //    }
    //};


    public struct Scale_Entity
    {
        public string ID;
        public string Grocery_Item;
        public float Unit_price;
    }
    //public List<RFID_Card_Entity> Card = new List<RFID_Card_Entity>();

    
    static class Program
    {
        //public Form1 Main_Form = new Form1();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        
        static void Main()
        {
            
           //Thread Message_Thread = new Thread(new ThreadStart(Form1.Process_Message));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
