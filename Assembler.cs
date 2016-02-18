using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace sim85
{   
    /*
     A dumb assembler for 8080.
     * 
     * Supports every opcode which the emulator can emulate,
     * i.e. arithmatical, logic, loads, stores and branches.
     * Does not support EI, DI, RST and some other stuff.
     * 
     * Produces a flat binary file.
     * 
     * Supports directives for LABELs, DATA8, DATA16 and DATASZ.
     * 
     * Will evaluate constants for all immediate mode operands.
     * For example, 
     *      MVI B, 3*(10+5) 
     *      will be resolved to MVI B, 3F
     * All constants are assumed hex.
     * Large constants will be truncated to lower 8 bits.
     * 
     * Each line must be of the form
     * 
     * LABEL: INST R0, R1
     * 
     * Labels are optional.
     * 
     */


    class Assembler
    {
        Dictionary<string, byte> operands;
        Dictionary<string, int> opcode_size;
        Dictionary<string, ushort> SymbolTable;
        ushort LC = 0;
        string[] asmsrc;
        byte[] gencode;

        Assembler(string program)
        {
            operands = new Dictionary<string, byte>();
            operands["A"] = 7;
            operands["B"] = 0;
            operands["C"] = 1;
            operands["D"] = 2;
            operands["E"] = 3;
            operands["H"] = 4;
            operands["L"] = 5;
            operands["M"] = 6;
            operands["BC"] = 0;
            operands["DE"] = 1;
            operands["HL"] = 2;
            operands["SP"] = 3;
            operands["NZ"] = 0;
            operands["Z"] = 1;
            operands["NC"] = 2;
            operands["C"] = 3;
            operands["PO"] = 4;
            operands["PE"] = 5;
            operands["P"] = 6;
            operands["M"] = 7;
            asmsrc = File.ReadAllLines(program);
        }
        //steps thru the source file, and builds
        //up the symbol table.
        void PassOne()
        {
            foreach (string line in asmsrc)
            {
                line.
            }
        }

        //8080 opcodes can be a maximum of 3bytes in length.
        //This packs the 3bytes into a 4byte Uint32.
        UInt32 Assemble(string istr)
        {
            throw new SystemException("");
        }


    }
}
