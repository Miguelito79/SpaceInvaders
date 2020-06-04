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
    /// <summary>
    /// Abstract class for memory management in DEBUG and RELEASE mode
    /// </summary>
    public abstract class MCU
    {
        public byte[][] buffer;

        public abstract void Clear();

        public abstract bool LoadMultipleFiles(string[] path);

        public abstract bool LoadDataInMemoyAt(byte[] data, int offset);

        public abstract bool LoadFileInMemoryAt(string path, long offset);

        public abstract byte ReadByte(ushort address);

        public abstract void WriteByte(ushort address, byte data);        
    }
}
        
        
