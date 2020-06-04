using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInvader_Core.MMU
{
    public class MCUD : MCU
    {                       
        public override void Initialize()
        {
            buffer = new byte[1][];
            buffer[0] = new byte[64 * 1024];
        }

        public override bool LoadMultipleFiles(string[] path)
        {
            bool response = true;

            try
            {
                long offset = 0;
                for (int count = 0; count < path.Length; count++)
                {
                    FileInfo finfo = new FileInfo(path[count]);
                    LoadFileInMemoryAt(path[count], offset);
                    offset += finfo.Length;
                }
            }
            catch
            {
                response = false;
            }

            return response;
        }

        public override bool LoadDataInMemoyAt(byte[] data, int offset)
        {
            bool respose = true;

            try
            {
                for (int count = offset; count < data.Length + offset; count++)
                {
                    buffer[0][count] = data[count - offset];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return respose;
        }

        public override bool LoadFileInMemoryAt(string path, long offset)
        {
            bool response = true;

            try
            {
                long startingAddress = offset;
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    int numBytesToRead = (int)stream.Length;
                    if (numBytesToRead > 0)
                    {
                        do
                        {
                            byte value = (byte)stream.ReadByte();
                            buffer[0][startingAddress] = value;

                            numBytesToRead -= 1;
                            startingAddress += 1;
                        } while (numBytesToRead > 0);
                    }
                }
            }
            catch
            {
                response = false;
            }

            return response;
        }

        public override byte[] GetVideoRam()
        {
            return null;
        }

        public override byte ReadByte(ushort address)
        {
            return buffer[0][address];            
        }

        public override void WriteByte(ushort address, byte data)
        {
            buffer[0][address] = data;
        }        
    }
}
