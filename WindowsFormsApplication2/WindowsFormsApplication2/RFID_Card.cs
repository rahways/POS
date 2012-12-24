using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication2
{
    public class RFID_Card
    {
        //float _Total_Amount;
        //string _ID;
        //int index;
        //List<Grocery> _Grocery_Item = new List<Grocery>();

        public float Total_Amount
        {
            get;
            set;
        }

        public string ID
        {
            get;
            set;
        }

        public List<Grocery> Grocery_Item
        {
            get;
            set;
        }
    }
}
