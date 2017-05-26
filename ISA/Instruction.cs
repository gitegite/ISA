using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA
{
    public class Instruction
    {
        public Operator Operator { get; set; }
        public Register DestinationRegister { get; set; }
        public Register SourceRegister { get; set; }
        public Register ValueRegister { get; set; }
        public int Value { get; set; }
        public Memory Memory { get; set; }
        public List<string> Stage { get; set; }

        public Instruction() : base()
        {

        }

        public Instruction(Operator operand, Register dest, Register source = null, Register valueR = null, int value = 0,Memory memory = null)
        {
            Memory = memory;
            Operator = operand;
            DestinationRegister = dest;
            SourceRegister = source;
            ValueRegister = valueR;
            Value = value;
            Stage = new List<string> { "IF", "ID", "EX", "MEM", "WB" };
        }
        public override string ToString()
        {
            var value = Value == 0 ? "" : Value.ToString();
            if (SourceRegister != null)
                return $"{Operator} {DestinationRegister} {SourceRegister} {ValueRegister?.Name ?? value}";
            else if(Memory != null)
                return $"{Operator} {DestinationRegister} {Memory}";
            else
                return $"{Operator} {DestinationRegister} {Value}";

        }
    }
}
