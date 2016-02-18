using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Simulator for a bare Intel 8080.
 * Contains an assembler and an execution engine.
 
 * * The assembler is simple and stupid, and only supports a couple of 
 * directives - DATA, DATASZ and LABELs.
 * 
 * The execution engine supports most documented opcodes. Comes with
 * PRINTB, INPUTB, PRINTSZ and INPUTSZ for basic I/O.
 * 
 * USAGE : sim80.exe --assemble program.s program.bin
 *       : sim80.exe --run program.bin
 * 
 */

namespace sim85
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2 || args.Length != 3)
                printHelp();
            switch(args[0])
            {
                case "--run":
                    break;
                case "--assemble":
                    break;
                default:
                    printHelp();
            }
            CPU c = new CPU("test.bin");
            while (!c.IsHalt)
                c.ExecuteOpcode();
            Console.WriteLine("CPU Halt!");
        }
    }
}
