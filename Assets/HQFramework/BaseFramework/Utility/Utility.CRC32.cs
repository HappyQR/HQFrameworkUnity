using System.Text;

namespace HQFramework
{
    public partial class Utility
    {
        public static class CRC32
        {
            private static readonly uint[] Crc32Table;

            static CRC32()
            {
                Crc32Table = new uint[256];
                const uint polynomial = 0xEDB88320;
                for (uint i = 0; i < 256; i++)
                {
                    uint crc = i;
                    for (uint j = 8; j > 0; j--)
                    {
                        if ((crc & 1) == 1)
                        {
                            crc = (crc >> 1) ^ polynomial;
                        }
                        else
                        {
                            crc >>= 1;
                        }
                    }
                    Crc32Table[i] = crc;
                }
            }

            public static uint ComputeCrc32(string input)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                uint crc = 0xFFFFFFFF;
                foreach (byte b in bytes)
                {
                    byte tableIndex = (byte)(((crc) & 0xFF) ^ b);
                    crc = Crc32Table[tableIndex] ^ (crc >> 8);
                }
                return ~crc;
            }
        }
    }
}
