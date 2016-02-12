using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 *  Simulates the Intel 8080/8085 CPU. 
 *  Data regarding the CPU (architecture, opcode listings, etc)
 *  has been taken from the Intel Programmer's Manual for
 *  8080/8085. This does not support any interrupts.
 *  The opcode decoder has been written from the data in 8080_specs.txt
 *  
*/
namespace sim85
{
    class CPU
    {
        //CPU Registers:
        //PSW = Flags:Accum
        //Flags = Sign Zero x Aux x Parity x Carry
        //The x's are unused(?) bits.
        ushort PSW;
        bool S
        {
            get { return Util.GetBitN(PSW,15) == 1 ? true : false;}
            set { if (value) Util.SetBitN(ref PSW, 15); else Util.UnsetBitN(ref PSW, 15); }
        }
        bool Z
        {
            get { return Util.GetBitN(PSW, 14) == 1 ? true : false; }
            set { if (value) Util.SetBitN(ref PSW, 14); else Util.UnsetBitN(ref PSW, 14); }
        }
        bool P
        {
            get { return Util.GetBitN(PSW, 10) == 1 ? true : false; }
            set { if (value) Util.SetBitN(ref PSW, 10); else Util.UnsetBitN(ref PSW, 10); }
        }
        bool C
        {
            get { return Util.GetBitN(PSW, 8) == 1 ? true : false; }
            set { if (value) Util.SetBitN(ref PSW, 8); else Util.UnsetBitN(ref PSW, 8); }
        }
        byte A
        {
            get { return (byte)(PSW & 0x00FF); }
            set { PSW &= 0xFF00; PSW |= value; }
        }
        ushort BC;
        ushort DE;
        ushort HL;
        ushort SP;
        ushort PC;
        //Memory map. Although in this case, no hardware is simulated
        //and we load program code from a flat binary file into location
        //0x00 and start executing.
        byte[] Memory;
        bool IsHalt;
        CPU()
        {
            Memory = new byte[2^16];
            PSW = BC = DE = HL = SP = PC = 0;
            IsHalt = false;
        }

        void WriteRegIndex(int index, byte value)
        {
            switch(index)
            {
                case 0: BC |= (ushort)(value << 8); break;
                case 1: BC |= (byte)(value); break;
                case 2: DE |= (ushort)(value << 8); break;
                case 3: DE |= (byte)(value); break;
                case 4: HL |= (ushort)(value << 8); break;
                case 5: HL |= (byte)(value); break;
                case 6: Memory[HL] = value; break;
                case 7: A = value; break;
                default: Util.DebugLog("WriteRegIdx : Wrong Index!"); break;
            }
        }

        byte ReadRegIndex(int index)
        {
            byte b = 0;
            switch (index)
            {
                case 0: b = Util.HighByte(BC); ; break;
                case 1: b = Util.LowByte(BC); break;
                case 2: b = Util.HighByte(DE); break;
                case 3: b = Util.LowByte(BC); break;
                case 4: b = Util.HighByte(HL); break;
                case 5: b = Util.LowByte(HL); break;
                case 6: b = Memory[HL]; break;
                case 7: b = A; break;
                default: Util.DebugLog("ReadRegIdx : Wrong Index!"); break;
            }
            return b;
        }
        /*
        void ProcessNextOpcode()
        {
            byte opcode = Memory[PC];
            //Process 1-byte, 0-arg opcodes
            switch(opcode)
            {
                case 0x0: 
                    Util.DebugLog("NOP");
                    PC += 1;
                    break;
                case 0x2F:
                    Util.DebugLog("CMA");
                    A = (byte)~A;
                    //Why do we need this?
                    PC += 1;
                    break;
                case 0x3F:
                    Util.DebugLog("CMC");
                    C = !C;
                    PC += 1;
                    break;
                case 0x76:
                    Util.DebugLog("HALT");
                    IsHalt = true;
                    PC += 1;
                    break;
                case 0xE9:
                    Util.DebugLog("PCHL");
                    PC = HL;
                    break;
                case 0x17:
                    //FIXME
                    Util.DebugLog("RAL");
                    ushort CA = (ushort)(PSW & 0x01FF);
                    CA <<= 1;
                    break;
                case 0xC9:
                    Util.DebugLog("RET");
                    PC = Memory[SP];
                    SP++;
                    break;
                case 0xF9:
                    Util.DebugLog("SPHL");
                    SP = HL;
                    PC += 1;
                    break;
                case 0xEB:
                    Util.DebugLog("XCHG");
                    ushort t = DE;
                    DE = HL;
                    HL = t;
                    PC += 1;
                    break;
                case 0xE3:
                    Util.DebugLog("XTHL");
                    ushort tt = HL;
                    HL = Util.MakeWord(Memory[SP + 1], Memory[SP]);
                    //The stack grows top to bottom, so SP + 1 comes 
                    //before SP
                    Memory[SP] = Util.LowByte(tt);
                    Memory[SP + 1] = Util.HighByte(tt);
                    PC += 1;
                    break;
                default:
                    //Util.DebugLog("Unknown/Unsupported Opcode.");
                    break;
            }
            //Process 1-byte, 1-arg opcodes
            switch(opcode & )
            
        }
        */
       
       void ExecuteOpcode()
        {
           byte opcode = Memory[PC];
           ushort t = 0x0;

           //MOVs
           if(opcode >> 6 == 1)
           {
               byte dest = (byte)(opcode & 0x07);
               byte src = (byte)((opcode & 0x38) >> 3);
               WriteRegIndex(dest, ReadRegIndex(src));
               PC++;
           }
           //Immediate MOVes
           if(0 == opcode >> 6)
           {
               WriteRegIndex(opcode >> 3, Memory[PC + 1]);
               PC += 2;
           }
           switch(opcode)
           {
               //JUMPS
               case 0xC3:
                   PC = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                   break;
               case 0xC2:
                   if (Z)
                       PC += 3;
                   else
                       PC = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                    break;
               case 0xCA:
                    if (!Z)
                       PC += 3;
                   else
                       PC = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                   break;
               case 0xF2:
                    PC = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                    break;
               case 0xE9:
                    PC = HL;
                    break;
               //Immediate Loads (LXI)
               case 0x01:
                    BC = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                    PC += 3;
                    break;
               case 0x11:
                    DE = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                    PC += 3;
                    break;
               case 0x21:
                    HL = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                    PC += 3;
                    break;
               case 0x31:
                    SP = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                    PC += 3;
                    break;
                //Loads and Stores
               case 0x0A:
                    A = Memory[BC];
                    PC++;
                    break;
               case 0x1A:
                    A = Memory[DE];
                    PC++;
                    break;
               case 0x2A:
                   t = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                   HL = Util.MakeWord(Memory[t + 1], Memory[t]);
                   PC += 3;
                   break;
               case 0x3A:
                   A = Memory[Util.MakeWord(Memory[PC + 2], Memory[PC + 1])];
                   PC += 3;
                   break;
               case 0x02:
                   Memory[BC] = A;
                   PC++;
                   break;
               case 0x12:
                   Memory[DE] = A;
                   PC++;
                   break;
               case 0x22:
                   t = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                   Memory[t] = A;
                   PC += 3;
                   break;
               //Immediate mode arithmetic
               case 0xC6:
                   A += Memory[PC + 1];
                   break;
                case 0x
                   


           }
        }

       void AdjustFlags()
       {
           Z = A == 0; //Adjust zero flag
           S = 1 == (A >> 6); //Adjust sign flag
           byte a = A;
           uint c = 0;
           for (c = 0; a!=0; c++)
               a &= (byte)(a - 1);
           P = 0 == (c & 1); //Adjust parity flag;
       }
       void AdjustCarrySum(byte a)
       {

       }

    }
}
