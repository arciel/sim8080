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
            byte b = 0xAA;

            Console.WriteLine("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n",b, b >> 0, b >> 1, b >> 2, b >> 3, b >> 4, b >> 5, b >> 6, b >> 7, b >> 8);
            Console.Read();
        }
    }
}
