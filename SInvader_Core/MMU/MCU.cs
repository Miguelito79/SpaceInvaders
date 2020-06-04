using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SInvader_Core.MMU
{
    public abstract class MCU
    {
        protected byte[][] buffer;

        public abstract void Initialize();

        public abstract bool LoadMultipleFiles(string[] path);

        public abstract bool LoadDataInMemoyAt(byte[] data, int offset);

        public abstract bool LoadFileInMemoryAt(string path, long offset);

        public abstract byte ReadByte(ushort address);

        public abstract void WriteByte(ushort address, byte data);

        public abstract byte[] GetVideoRam();
    }
}
        
        
