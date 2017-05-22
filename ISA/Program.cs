using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to ISA");
            Console.WriteLine("----------------------------------------------");
            var input = "";
            List<Instruction> instructionList = new List<Instruction>();

            do
            {
                input = Console.ReadLine();
                var instructionSplitted = input.Split(' ');
                var operetor = instructionSplitted[0];
                var operandSplitted = instructionSplitted[1].Split(',');



            } while (input != "exit");

        }
        private Operator GetOperator(string input)
        {
            switch (input.ToLower())
            {
                case "mov":
                    return Operator.MOV;
                case "sub":
                    return Operator.SUB;
                case "mul":
                    return Operator.MUL;
                case "add":
                    return Operator.ADD;
                case "store":
                    return Operator.STORE;
                case "load":
                    return Operator.LOAD;
            }
            return Operator.JMP;
        }
    }
}
