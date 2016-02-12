using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sim85
{   
    /*
     A dumb assembler for 8080.
     * 
     * Supports every opcode which the emulator can emulate,
     * i.e. arithmatical, logic, loads, stores and branches.
     * Does not support EI, DI, IN, OUT and RST.
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
     */


    class Assembler
    {
    }
}
