using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInvader_Core.MMU
{
    /// <summary>
    /// MCUR: Memory control unit for release of "Space Invaders"
    /// This class provide memory access specific for the emulation of SpaceInvaders
    /// </summary>
    class MCUR : MCU
    {              
        public MCUR()
        {
            buffer = new byte[3][];
            buffer[0] = new byte[8192]; //ROM  From 0000 to 1fff        
            buffer[1] = new byte[1024]; //RAM  from 2000 to 23ff        
            buffer[2] = new byte[7168]; //VRAM From 2400 to 3fff        
        }        

        public override void Clear()
        {
            Array.Clear(buffer[0], 0, buffer[0].Length);
            Array.Clear(buffer[1], 0, buffer[1].Length);
            Array.Clear(buffer[2], 0, buffer[2].Length);
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

        public override byte ReadByte(ushort address)
        {
            byte response = 0;

            if (address < 0x2000)
                response = buffer[0][address];
            
            else if (address >= 0x2000 && address < 0x2400)
                response = buffer[1][address - 0x2000];

            else if (address >= 0x2400 && address < 0x4000)
                response = buffer[2][address - 0x2400];

            //RAM MIRROR
            //else if (address >= 0x4000 && address < 0x4400)
            //    response = ram[address - 0x4000];

            //VRAM MIRROR
            //else if (address >= 0x4400 && address < 0X6000)
            //    response = vram[address - 0x4400];

            else
                throw new Exception("Out of range when reading memory address");

            return response;
        }

        public override void WriteByte(ushort address, byte data)
        {
            //Writing on RAM
            if (address >= 0x2000 && address < 0x2400)
                buffer[1][address - 0x2000] = data;

            //Writing on VRAM
            else if (address >= 0x2400 && address < 0x4000)
                buffer[2][address - 0x2400] = data;

            //RAM MIRROR
            else if (address >= 0x4000 && address < 0x4400)
                buffer[1][address - 0x4000] = data;

            //VRAM MIRROR
            else if (address >= 0x4400 && address < 0X6000)
                buffer[2][address - 0x4400] = data;

            else
                throw new Exception("Out of range when writing memory address");
        }
    }
}
