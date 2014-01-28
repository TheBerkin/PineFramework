using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PineFramework
{
    public class CogBytecode
    {
        public readonly byte[] CompiledCode;
        public readonly int[] LabelEntryPoints;

        private const uint MagicNumber = 0x50434243;

        public CogBytecode(byte[] code, int[] labels)
        {
            this.CompiledCode = code;
            this.LabelEntryPoints = labels;
        }

        public void Save(string path)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
            {
                writer.Write(MagicNumber);
                writer.Write(this.LabelEntryPoints.Length);
                for(int i = 0; i < this.LabelEntryPoints.Length; i++)
                {
                    writer.Write(this.LabelEntryPoints[i]);
                }
                writer.Write(this.CompiledCode.Length);
                writer.Write(this.CompiledCode);                
            }
        }

        public static CogBytecode FromFile(string path)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                if (reader.ReadUInt32() != MagicNumber)
                {
                    throw new InvalidDataException("Cog load failed: Magic number did not match.");
                }

                int ptrCount = reader.ReadInt32();
                int[] ptrs = new int[ptrCount];

                for(int i = 0; i < ptrCount; i++)
                {
                    ptrs[i] = reader.ReadInt32();
                }

                int codeLength = reader.ReadInt32();
                byte[] code = reader.ReadBytes(codeLength);

                return new CogBytecode(code, ptrs);
            }
        }
    }
}
