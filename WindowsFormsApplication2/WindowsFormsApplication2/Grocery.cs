using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication2
{
    public class Grocery
    {
        string _ID;
        float _Weight;
        float _Unit_Price;

        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public float Weight
        {
            get { return _Weight; }
            set { _Weight = value; }
        }

        public float Unit_Price
        {
            get { return _Unit_Price; }
            set { _Unit_Price = value; }
        }
    }
}
