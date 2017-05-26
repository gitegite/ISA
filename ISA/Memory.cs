using System.Linq;

namespace ISA
{
    public class Memory
    {
        public int Value { get; set; }
        public string Name { get; set; }

        public Memory()
        {

        }

        public Memory(string name,int value)
        {
            //Values = Enumerable.Repeat<int>(5, 1000).ToArray();
            Value = value;
            Name = name;
        }
        public override string ToString()
        {
            return $"({Name})";
        }
    }
}