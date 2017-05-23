using System;

namespace ISA
{
    public class Register
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

        public Register(string name, int value = 0)
        {
            Name = name;
            Value = value;
            Address = Convert.ToString(Convert.ToInt32(Name.Replace("R", "")), 2).PadLeft(5, '0');          
        }
        public override string ToString()
        {
            return Name;
        }
    }
}