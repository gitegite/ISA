﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA
{
    class Program
    {
        static List<Register> _registers = new List<Register>();
        static List<Instruction> _instructionList = new List<Instruction>();
        static int _numClockCycle = 0;
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to ISA");
            Console.WriteLine("----------------------------------------------");
            var input = "";

            do
            {
                input = Console.ReadLine();
                if (input == "end")
                    break;
                input = input.Replace("r", "R");
                var instructionSplitted = input.Split(' ').ToList();
                var opp = instructionSplitted[0];
                var op = GetOperator(opp);
                var count = instructionSplitted.Count;
                Register destinationR = new Register(instructionSplitted[1], 0);
                Instruction instruction = new Instruction();

                //add r1 r2
                //add r1 100
                if (count == 3)
                {
                    if (instructionSplitted[2].Contains('R'))
                    {
                        Register sourceR = new Register(instructionSplitted[2], 0);
                        instruction = new Instruction(op, destinationR, sourceR);
                    }
                    else
                    {
                        instruction = new Instruction(op, destinationR, value: Convert.ToInt32(instructionSplitted[2]));
                    }
                }
                //add r1 r2 r3
                //add r1 r2 100
                if (count == 4)
                {
                    Register sourceR = new Register(instructionSplitted[2], 0);
                    if (instructionSplitted[3].Contains('R'))
                    {
                        Register valueR = new Register(instructionSplitted[3], 0);
                        instruction = new Instruction(op, destinationR, sourceR, valueR);
                    }
                    else
                    {
                        instruction = new Instruction(op, destinationR, sourceR, value: Convert.ToInt32(instructionSplitted[3]));
                    }
                }

                ExecuteInstruction(instruction);
                //Console.WriteLine($"Debug:\n{instruction}");

                //foreach (var item in _registers)
                //{
                //    Console.WriteLine(item.Name + ":" + item.Value + ",");
                //}
                _instructionList.Add(instruction);

            } while (input != "end");
            PrintInstructions();
            PrintRegistersValue();
            PrintCPI();
            PrintExecution();
            Console.ReadLine();
        }

        private static void PrintExecution()
        {
            var tabAmount = 1;
            Func<string, int, string> increaseTab = (x, amount) => { return x.PadLeft(amount, '\t'); };

            FindRAW();

            PrintPipelineHeader();
            _instructionList.ForEach(x =>
            {
                var stage = increaseTab("\t", tabAmount);
                foreach (var item in x.Stage)
                {
                    stage += item + "\t";
                }
                Console.WriteLine($"{_instructionList.IndexOf(x) + 1}.\t {x}{stage}");
                tabAmount++;
            });
        }

        private static void FindRAW()
        {

            foreach (var currentInstruction in _instructionList.Skip(1).ToList())
            {
                var prevInstruction = _instructionList[_instructionList.IndexOf(currentInstruction) - 1];
                if (currentInstruction.SourceRegister != null && prevInstruction.DestinationRegister.Name == currentInstruction.SourceRegister.Name)
                {
                    //stall
                    if(prevInstruction.Operator == Operator.MOV)
                    {
                        currentInstruction.Stage.Insert(2, "ST");

                    }
                    //forwarding
                    else
                    {

                    }
                }
            }

        }

        private static void PrintPipelineHeader(int numStall = 0)
        {
            _numClockCycle = 5 + _instructionList.Count - 1 + numStall;
            var header = "No.\tInstruction\t";
            for (int i = 0; i < _numClockCycle; i++)
            {
                header += (i + 1) + "\t";
            }
            Console.WriteLine(header);
        }

        private static void PrintCPI()
        {
            decimal cpi = _instructionList.Sum(x => Convert.ToDecimal(GetClockCycle(x.Operator))) / Convert.ToDecimal(_instructionList.Count);
            Console.WriteLine($"CPI = {cpi}");
        }

        private static void PrintInstructions()
        {

            Console.WriteLine("PC\tInstrctions\t\t\t\t\tClock Cycle");
            _instructionList.ForEach(x =>
            {

                //add r1 10
                if (x.SourceRegister == null)
                {
                    Console.WriteLine($"PC[{_instructionList.IndexOf(x)}]->\t{x.Operator} {x.DestinationRegister} {x.Value.ToString() }=> {x.DestinationRegister.Address} {"".PadLeft(5, '0')} {Convert.ToString(x.Value, 2).PadLeft(16, '0')}" + "\t\t\t" + GetClockCycle(x.Operator));

                }
                //add r1 r2
                //add r1 r2 r3
                else
                {
                    var val = x.ValueRegister?.Name ?? x.Value.ToString();
                    if (val != "0")
                    {
                        val = " " + val + " ";
                    }
                    Console.WriteLine($"PC[{_instructionList.IndexOf(x)}]->\t{x.Operator} {x.DestinationRegister} {x.SourceRegister.Name}{val}=> {x.DestinationRegister.Address} {x.SourceRegister.Address} {Convert.ToString(x.ValueRegister?.Value ?? x.Value, 2).PadLeft(16, '0')}" + "\t\t" + GetClockCycle(x.Operator));
                }
            });
        }
        private static void PrintRegistersValue()
        {
            _registers.ForEach(x =>
            {
                Console.WriteLine($"{x.Name}--> {x.Value} [{Convert.ToString(x.Value, 2).PadLeft(16, '0')}]");
            });

        }
        private static string GetClockCycle(Operator op)
        {
            switch (op)
            {
                case Operator.MOV:
                    return "1";

                case Operator.ADD:
                    return "2";
                case Operator.SUB:
                    return "2";
                case Operator.MUL:
                    return "3";
                case Operator.DIV:
                    return "5";
                default:
                    return "";
            }
        }
        private static void Multiply(Instruction instruction)
        {
            if (!IsExistedInRegistersPool(instruction.DestinationRegister))
            {
                _registers.Add(instruction.DestinationRegister);
            }
            var destR = GetRegister(instruction.DestinationRegister.Name);

            //add r1 10
            if (instruction.SourceRegister == null)
            {
                destR.Value *= instruction.Value;
            }
            else
            {
                var sourceR = GetRegister(instruction.SourceRegister.Name);
                //add r1 r2 r3
                if (instruction.ValueRegister != null)
                {
                    var valueR = GetRegister(instruction.ValueRegister.Name);
                    destR.Value *= sourceR.Value + valueR.Value;
                }
                //add r1 r2 10
                else
                {
                    destR.Value *= sourceR.Value + instruction.Value;
                }
            }
        }

        private static void Divide(Instruction instruction)
        {
            if (!IsExistedInRegistersPool(instruction.DestinationRegister))
            {
                _registers.Add(instruction.DestinationRegister);
            }
            var destR = GetRegister(instruction.DestinationRegister.Name);

            //add r1 10
            if (instruction.SourceRegister == null)
            {
                destR.Value /= instruction.Value;
            }
            else
            {
                var sourceR = GetRegister(instruction.SourceRegister.Name);
                //add r1 r2 r3
                if (instruction.ValueRegister != null)
                {
                    var valueR = GetRegister(instruction.ValueRegister.Name);
                    destR.Value /= sourceR.Value + valueR.Value;
                }
                //add r1 r2 10
                else
                {
                    destR.Value /= sourceR.Value + instruction.Value;
                }
            }
        }

        private static void Add(Instruction instruction)
        {
            if (!IsExistedInRegistersPool(instruction.DestinationRegister))
            {
                _registers.Add(instruction.DestinationRegister);
            }
            var destR = GetRegister(instruction.DestinationRegister.Name);

            //add r1 10
            if (instruction.SourceRegister == null)
            {
                destR.Value += instruction.Value;
            }
            else
            {
                var sourceR = GetRegister(instruction.SourceRegister.Name);
                //add r1 r2 r3
                if (instruction.ValueRegister != null)
                {
                    var valueR = GetRegister(instruction.ValueRegister.Name);
                    destR.Value += sourceR.Value + valueR.Value;
                }
                //add r1 r2 10
                else
                {
                    destR.Value += sourceR.Value + instruction.Value;
                }
            }
        }
        private static void Move(ref Instruction instruction)
        {
            if (!IsExistedInRegistersPool(instruction.DestinationRegister))
            {
                _registers.Add(instruction.DestinationRegister);
            }

            var destR = GetRegister(instruction.DestinationRegister.Name);
            if (instruction.SourceRegister == null)
            {
                destR.Value = instruction.Value;
            }
            else
            {
                var sourceR = GetRegister(instruction.SourceRegister.Name);
                destR.Value = sourceR == null ? destR.Value : sourceR.Value;
            }
        }

        private static bool IsExistedInRegistersPool(Register register)
        {
            return _registers.Any(x => x.Name == register.Name);
        }
        private static Register GetRegister(string name)
        {
            if (_registers.Any(x => x.Name == name))
            {
                return _registers.Single(x => x.Name == name);
            }
            else
            {
                return null;
            }
        }
        private static void AssignRegistersAddress(Instruction instruction)
        {
            instruction.DestinationRegister.Address = Convert.ToString(Convert.ToInt32(instruction.DestinationRegister.Name.Replace("R", "")), 2).PadLeft(5, '0');
            if (instruction.SourceRegister != null)
            {
                instruction.SourceRegister.Address = Convert.ToString(Convert.ToInt32(instruction.SourceRegister.Name.Replace("R", "")), 2).PadLeft(5, '0');

            }
            if (instruction.ValueRegister != null)
            {
                instruction.ValueRegister.Address = Convert.ToString(Convert.ToInt32(instruction.ValueRegister.Name.Replace("R", "")), 2).PadLeft(5, '0');

            }
        }
        private static Operator GetOperator(string input)
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
        private static void ExecuteInstruction(Instruction instruction)
        {
            switch (instruction.Operator)
            {
                case Operator.MOV:
                    Move(ref instruction);
                    break;
                case Operator.LOAD:
                    break;
                case Operator.STORE:
                    break;
                case Operator.ADD:
                    Add(instruction);
                    break;
                case Operator.SUB:
                    Sub(instruction);
                    break;
                case Operator.MUL:
                    Multiply(instruction);
                    break;
                case Operator.DIV:
                    Divide(instruction);
                    break;
                case Operator.JMP:
                    break;
                case Operator.CMP:
                    break;
                default:
                    break;
            }
        }

        private static void Sub(Instruction instruction)
        {
            if (!IsExistedInRegistersPool(instruction.DestinationRegister))
            {
                _registers.Add(instruction.DestinationRegister);
            }
            var destR = GetRegister(instruction.DestinationRegister.Name);

            //add r1 10
            if (instruction.SourceRegister == null)
            {
                destR.Value -= instruction.Value;
            }
            else
            {
                var sourceR = GetRegister(instruction.SourceRegister.Name);
                //add r1 r2 r3
                if (instruction.ValueRegister != null)
                {
                    var valueR = GetRegister(instruction.ValueRegister.Name);
                    destR.Value -= sourceR.Value + valueR.Value;
                }
                //add r1 r2 10
                else
                {
                    destR.Value -= sourceR.Value + instruction.Value;
                }
            }
        }
    }
}
