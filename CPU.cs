using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

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
        public bool IsHalt;
                
        public CPU(string program)
        {
            Memory = new byte[(int)Math.Pow(2,16)];
            PSW = BC = DE = HL = SP = PC = 0;
            IsHalt = false;
            FileStream fs = new FileStream(program, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            br.Read(Memory, 0, (int)fs.Length);
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
             
       public void ExecuteOpcode()
        {
           byte opcode = Memory[PC];
           ushort t = 0x0;
           int i = 0;

           //MOVs
           if((opcode >> 6) == 1)
           {
               byte src = (byte)(opcode & 0x07);
               byte dest = (byte)((opcode & 0x38) >> 3);
               WriteRegIndex(dest, ReadRegIndex(src));
               PC++;
           }
           //MVIs
           else if((opcode >> 6) == 0 && (opcode & 0x07) == 0x06)
           {
               WriteRegIndex(opcode >> 3, Memory[PC + 1]);
               PC += 2;
           }
           //ADDs
           else if(0x80 <= opcode && opcode <= 0x87)
           {
               if (A + ReadRegIndex(opcode & 0x07) > 255) C = true;
               A += ReadRegIndex(opcode & 0x07);
               PC += 1;
           }
           //ADCs
           else if(0x88 <= opcode && opcode <= 0x8F)
           {
               i = C ? 1 : 0;
               if (A + i + ReadRegIndex(opcode & 0x07) > 255) C = true;
               A += (byte)i;
               A += ReadRegIndex(opcode & 0x07);
               PC += 1;
           }
           //SUBs
           else if(0x90 <= opcode && opcode <= 0x97)
           {

           }
           //ANAs
           else if(0xA0 <= opcode && opcode <= 0xA7)
           {

           }
           //XRAs
           else if(0xA8 <= opcode && opcode <= 0xAF)
           {

           }
           //ORAs
           else if(0xB0 <= opcode && opcode <= 0xB7)
           {

           }
           //CMPs
           else if(0xB8 <= opcode && opcode <= 0xBF)
           {

           }
           switch(opcode)
           {
               case 0x0: //NOP
                   IsHalt = true;
                   break;
               //JUMPS
               case 0xC3: //JMP Addr
                   PC = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                   break;
               case 0xC2: //JNZ Addr
                   if (Z)
                       PC += 3;
                   else
                       PC = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                    break;
               case 0xCA: //JZ Addr
                    if (!Z)
                       PC += 3;
                   else
                       PC = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                   break;
               case 0xF2: //JP Addr
                    PC = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                    break;
               case 0xE9: //PCHL
                    PC = HL;
                    break;
               //Immediate Loads (LXI)
               case 0x01: //LXI B, Imm
                    BC = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                    PC += 3;
                    break;
               case 0x11: //LXI D, Imm
                    DE = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                    PC += 3;
                    break;
               case 0x21: //LXI H, Imm
                    HL = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                    PC += 3;
                    break;
               case 0x31: //LXI SP, Imm
                    SP = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                    PC += 3;
                    break;
                //Loads and Stores
               case 0x0A: //LDAX B
                    A = Memory[BC];
                    PC++;
                    break;
               case 0x1A: //LDAX D
                    A = Memory[DE];
                    PC++;
                    break;
               case 0x2A: //LHLD Addr
                   t = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                   HL = Util.MakeWord(Memory[t + 1], Memory[t]);
                   PC += 3;
                   break;
               case 0x3A: //LDA Addr
                   A = Memory[Util.MakeWord(Memory[PC + 2], Memory[PC + 1])];
                   PC += 3;
                   break;
               case 0x02: //STAX B
                   Memory[BC] = A;
                   PC++;
                   break;
               case 0x12: //STAX D
                   Memory[DE] = A;
                   PC++;
                   break;
               case 0x22: //SHLD Addr
                   t = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                   Memory[t] = HL;
                   PC += 3;
                   break;
               case 0x32: //STA Addr
                   t = Util.MakeWord(Memory[PC + 2], Memory[PC + 1]);
                   Memory[t] = A;
                   PC += 3;
                //Override IN and OUT to print and get from Console.
                //IN and OUT's byte param specifies registers (0) or 
                //strings (1). 
               case 0xDB: //IN
                   break;
               case 0xD3: //OUT
                   Console.WriteLine("{0:X}", A);
                   break;
               //Immediate mode arithmetic
               case 0xCE: //ACI
                   
           }
        }

       void AdjustFlags()
       {
           Z = A == 0; //Adjust zero flag
           S = 1 == (A >> 7); //Adjust sign flag
           byte a = A;
           uint c = 0;
           for (c = 0; a!=0; c++)
               a &= (byte)(a - 1);
           P = 0 == (c & 1); //Adjust parity flag;
       }
    }
}
