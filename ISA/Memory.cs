using System.Linq;

namespace ISA
{
    public class Memory
    {
        public int[] Values { get; set; }
        public string Name { get; set; }
        public Memory()
        {
            Values = Enumerable.Repeat<int>(5, 1000).ToArray();
        }
        public Memory(string name)
        {
            Values = Enumerable.Repeat<int>(5, 1000).ToArray();
            Name = name;
        }
    }
}