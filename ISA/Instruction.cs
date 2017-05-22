using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA
{
    public class Instruction
    {
        public Operator Operand { get; set; }
        public Register Register1 { get; set; }
        public Register Register2 { get; set; }
        public Register Register3 { get; set; }
        public int Value { get; set; }

        public Instruction(Operator operand, Register register1, Register register2 = null, Register register3 = null, int value = 0)
        {
            Operand = operand;
            Register1 = register1;
            Register2 = register2;
            Register3 = register3;
            Value = value;
        }
    }
}
