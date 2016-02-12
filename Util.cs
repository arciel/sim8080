using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sim85
{
    static class Util
    {
        public static byte GetBitN(ushort word, int n)
        {
            // n is 0..15
            return (byte)((word >> n) & 1);
        }
        public static void SetBitN(ref ushort word, int n)
        {
            //n is 0..15
            word |= (ushort)(1 << n);
        }
        public static void UnsetBitN(ref ushort word, int n)
        {
            word &= (ushort)~(1 << n);
        }
        public static byte LowByte(ushort word)
        {
            return (byte)(word & 0x00FF);
        }
        public static byte HighByte(ushort word)
        {
            return (byte)((word & 0xFF00) >> 8);
        }
        public static ushort MakeWord(byte hi, byte lo)
        {
           return (ushort)((hi << 8)|lo);
        }

        public static void DebugLog(string log, params ushort[] mem)
        {

        }

        

    
    }
}
