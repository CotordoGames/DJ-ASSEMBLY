using System;
using System.Diagnostics;
using DJASM;


namespace DJASM
{
    class Program
    {
        

        public enum InstructionType
        {
            loadI, //loads immediate value. (register, value)
            storeI, //stores immediate value. (address, value)
            SwapR, //loads a registers value into another register. (register, register)
            storeR, //stores the value of a register in memory. (register, address)
            loadA, // loads the value of an adress into the selected register
            jump, //jumps to the specified line
            jumpZ, //jumps to the specified line if (addr) is 0
            jumpL, //jumps id less than (addr)
            jumpG, //jumps if greater than (addr)
            jumpE, //jumps if equal to (addr)
            Add, //adds 2 numbers (addr, addr, out)
            Sub, //subtracts a from b (addr, addr)
            delay, //delays instruction exectution by x ticks
            NoOp
        }

        struct Instruction
        {
            public InstructionType type;
            public List<string> arguments;

            public Instruction(InstructionType Type, List<string> args)
            {
                type = Type;
                arguments = args;
            }
        }
        static List<Instruction> ParseDJFile(string file)
        {
            var instructions = new List<Instruction>();
            var lines = File.ReadAllLines(file);

            foreach(var rawline in lines)
            {
                string line = rawline.Split(';')[0].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var tokens = line.Split(' ').ToList();

                string command = tokens[0].ToLower();

                InstructionType type = command switch
                {
                    "loadi" => InstructionType.loadI,
                    "loadr" => InstructionType.SwapR,
                    "storei" => InstructionType.storeI,
                    "storer" => InstructionType.storeR,
                    "jump" => InstructionType.jump,
                    "jumpz" => InstructionType.jumpZ,
                    "add" => InstructionType.Add,
                    "loada" => InstructionType.loadA,
                    "delay" => InstructionType.delay,
                    _ => InstructionType.NoOp,
                };
                instructions.Add(new Instruction(type, tokens));
            }
            return instructions;
        }

        static void ExecuteInstructions(List<Instruction> instructions, byte[] RAM)
        {
            int printaddr = 16383;
            byte r1 = 0;
            byte r2 = 0;
            int idx = 0;
            while(idx < instructions.Count)
            {
                var instr = instructions[idx];
                var parts = instr.arguments;

                switch(instr.type)
                {
                    case InstructionType.loadI:
                        if (parts[1].Contains("1"))
                        {
                            r1 = Convert.ToByte(parts[2]);

                        }
                        else if (parts[1].Contains("2"))
                        {
                            r2 = Convert.ToByte(parts[2]);
                        }
                        idx++;
                        break;

                    case InstructionType.SwapR:
                        if (parts[1].Contains("1"))
                        {
                            r1 = r2;
                        }
                        else if (parts[1].Contains("2"))
                        {
                            r2 = r1;
                        }
                        idx++;
                        break;

                    case InstructionType.NoOp:
                        idx++;
                        break;

                    case InstructionType.storeI:
                        RAM[Convert.ToUInt16(parts[1])] = Convert.ToByte(parts[2]);
                        if (RAM[0] != 0)
                        {
                            Console.Write((char)RAM[printaddr]);
                        }
                        idx++;
                        break;

                    case InstructionType.storeR:
                        if (parts[1].Contains("1"))
                        {
                            RAM[Convert.ToUInt16(parts[2])] = r1;
                        }
                        else if (parts[1].Contains("2"))
                        {
                            RAM[Convert.ToUInt16(parts[2])] = r2;
                        }
                        if (RAM[printaddr] != 0)
                        {
                            Console.Write((char)RAM[printaddr]);
                        }
                        idx++;
                        break;

                    case InstructionType.jump:
                        idx = Convert.ToUInt16(parts[1]);
                        break;

                    case InstructionType.jumpZ:
                        if (Convert.ToUInt16(parts[1]) == 0)
                        {
                            idx = Convert.ToUInt16(parts[1]);
                        }
                        else
                        {
                            idx++;
                        }
                        break;

                    case InstructionType.Add:
                        RAM[Convert.ToInt16(parts[3])] = Convert.ToByte(Convert.ToByte(parts[1]) + Convert.ToByte(parts[2]));
                        idx++;
                        break;

                    case InstructionType.loadA:
                        if (parts[1].Contains("1"))
                        {
                            r1 = RAM[Convert.ToUInt16(parts[2])];
                        }
                        else if (parts[1].Contains("2"))
                        {
                           r2 = RAM[Convert.ToUInt16(parts[2])];
                        }
                        if (RAM[printaddr] != 0)
                        {
                            Console.Write((char)RAM[printaddr]);
                        }
                        idx++;
                        break;

                    case InstructionType.delay:
                        Thread.Sleep(Convert.ToUInt16(parts[1]));
                        idx++;
                        break;
                }
                WindowCreation.UpdateScreen();


            }
        }

        public static byte[] RAM = new byte[65536];
        public static void Main()
        {
            

            string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "source.dj");

            List<Instruction> Instructions = ParseDJFile(FilePath);

            WindowCreation.CreateWindow();

            ExecuteInstructions(Instructions, Program.RAM);
        }
    }
}