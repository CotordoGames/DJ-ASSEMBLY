using System;
using System.Diagnostics;
using DJASM;
using Raylib_cs;


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
            AddA, //adds 2 numbers (addr, addr, out)
            Sub, //subtracts a from b (addr, addr)
            delay, //delays instruction exectution by x ticks
            ResolutionShift, //shifts the resolution by 1, giving an isometric perspection shift
            DrawScreen,
            LoadDerefA,
            JumpE,
            AddReg,
            AddI,
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
            var labels = new Dictionary<string, int>();
            var lines = File.ReadAllLines(file);

            int currentidx = 0;

            foreach(var rawline in lines)
            {
                string line = rawline.Split(';')[0].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;



                if (line.EndsWith(":"))
                {
                    string lname = line.TrimEnd(':');
                    labels[lname] = currentidx;
                    continue;
                }



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
                    "adda" => InstructionType.AddA,
                    "loada" => InstructionType.loadA,
                    "delay" => InstructionType.delay,
                    "rshift" => InstructionType.ResolutionShift,
                    "draw" => InstructionType.DrawScreen,
                    "loadref" => InstructionType.LoadDerefA,
                    "jumpe" => InstructionType.JumpE,
                    "addr" => InstructionType.AddReg,
                    _ => InstructionType.NoOp,
                };
                instructions.Add(new Instruction(type, tokens));
                currentidx++;
            }

            for(int i = 0; i < instructions.Count; i++)
            {
                var instr = instructions[i];
                if ((instr.type == InstructionType.jump || instr.type == InstructionType.jumpZ) && labels.ContainsKey(instr.arguments[1]))
                {
                    instr.arguments[1] = labels[instr.arguments[1]].ToString();
                    instructions[i] = instr;
                }
            }



            return instructions;
        }

        static void ExecuteInstructions(List<Instruction> instructions, byte[] RAM)
        {
            int printaddr = 65535;
            byte r1 = 0;
            byte r2 = 0;
            int idx = 0;
            byte userin = 0b00000000;
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
                        RAM[Convert.ToInt32(parts[1])] = Convert.ToByte(parts[2]);
                        if (RAM[0] != 0)
                        {
                            Console.Write((char)RAM[printaddr]);
                        }
                        idx++;
                        break;

                    case InstructionType.storeR:
                        if (parts[1].Contains("1"))
                        {
                            RAM[Convert.ToInt32(parts[2])] = r1;
                        }
                        else if (parts[1].Contains("2"))
                        {
                            RAM[Convert.ToInt32(parts[2])] = r2;
                        }
                        if (RAM[printaddr] != 0)
                        {
                            Console.Write((char)RAM[printaddr]);
                        }
                        idx++;
                        break;

                    case InstructionType.jump:
                        idx = Convert.ToInt32(parts[1]);
                        break;

                    case InstructionType.AddA:
                        RAM[Convert.ToInt16(parts[3])] = Convert.ToByte(RAM[Convert.ToInt32(parts[1])] + RAM[Convert.ToInt32(parts[2])]);
                        idx++;
                        break;

                    case InstructionType.loadA:
                        if (parts[1].Contains("1"))
                        {
                            r1 = RAM[Convert.ToInt32(parts[2])];
                        }
                        else if (parts[1].Contains("2"))
                        {
                           r2 = RAM[Convert.ToInt32(parts[2])];
                        }
                        if (RAM[printaddr] != 0)
                        {
                            Console.Write((char)RAM[printaddr]);
                        }
                        idx++;
                        break;

                    case InstructionType.delay:
                        Thread.Sleep(Convert.ToInt32(parts[1]));
                        idx++;
                        break;


                    case InstructionType.jumpZ:
                        if (RAM[Convert.ToInt16(parts[1])] == 0)
                        {
                            idx = Convert.ToInt16(parts[1]);
                        }
                        else
                        {
                            idx++;
                        }
                        break;

                    case InstructionType.ResolutionShift:
                        WindowCreation.winw = Convert.ToInt16(parts[1]);
                        idx++;
                        break;

                    case InstructionType.DrawScreen:
                        WindowCreation.UpdateScreen();
                        idx++ ; break;

                    case InstructionType.LoadDerefA:
                        if (Convert.ToInt16(parts[1]) == 1)
                        {
                            r1 = RAM[RAM[Convert.ToInt16(parts[2])]];
                        }
                        else if (Convert.ToInt16(parts[1]) == 2)
                        {
                            r2 = RAM[RAM[Convert.ToInt16(parts[2])]];
                        }
                        idx++ ; break;

                    case InstructionType.JumpE:
                        if (Convert.ToInt32(parts[3]) == 1 && RAM[Convert.ToInt16(parts[1])] == r1)
                        {
                            idx = Convert.ToInt32(parts[2]);
                        }
                        else if (Convert.ToInt32(parts[3]) == 2 && RAM[Convert.ToInt16(parts[1])] == r2)
                        {
                            idx = Convert.ToInt32(parts[2]);
                        }
                        else
                        {
                            idx++;
                        }
                        break;

                    case InstructionType.AddReg:
                        RAM[Convert.ToInt16(parts[1])] = Convert.ToByte(r1 + r2);
                        idx++;
                        break;

                    case InstructionType.AddI:
                        RAM[Convert.ToInt16(parts[3])] = Convert.ToByte(parts[1] + parts[2]);
                        idx++;
                        break;


                }

                if (Raylib.IsKeyDown(KeyboardKey.Z))
                {
                    userin = (byte)(userin | 0b10000000);
                }
                else if (Raylib.IsKeyDown(KeyboardKey.X))
                {
                    userin = (byte)(userin | 0b01000000);
                }
                else if (Raylib.IsKeyDown(KeyboardKey.Left))
                {
                    userin = (byte)(userin | 0b00100000);
                }
                else if (Raylib.IsKeyDown(KeyboardKey.Right))
                {
                    userin = (byte)(userin | 0b00010000);
                }
                else if (Raylib.IsKeyDown(KeyboardKey.Up))
                {
                    userin = (byte)(userin | 0b00001000);
                }
                else if (Raylib.IsKeyDown(KeyboardKey.Down))
                {
                    userin = (byte)(userin | 0b00000100);
                }



                RAM[12289] = userin;


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