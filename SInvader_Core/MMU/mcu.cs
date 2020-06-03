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
    public class MCU
    {
        byte[] rom = new byte[8192];  //From 0000 to 1fff        
        byte[] ram = new byte[1024];  //from 2000 to 23ff        
        byte[] vram = new byte[7168]; //From 2400 to 3fff        

        internal void Initialize()
        {
            
        }        

        public bool LoadMultipleFiles(string[] path)
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

        public bool LoadDataInMemoyAt(byte[] data, int offset)
        {
            bool respose = true;

            try
            {
                for (int count = offset; count < data.Length + offset; count++)
                {
                    rom[count] = data[count - offset];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return respose;
        }

        public bool LoadFileInMemoryAt(string path, long offset)
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
                            rom[startingAddress] = value;

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

        /// <summary>
        /// Get a copuy of video to display in screen
        /// </summary>
        /// <returns></returns>
        public byte[] GetVideoRam()
        {
            byte[] response = new byte[7168];
            Array.Copy(vram, 0x0, response, 0, 7168);
            return response;
        }

        public byte ReadByte(ushort address)
        {
            byte response = 0;

            if (address < 0x2000)
                response = rom[address];
            //RAM
            else if (address >= 0x2000 && address < 0x2400)
                response = ram[address - 0x2000];

            else if (address >= 0x2400 && address < 0x4000)
                response = vram[address - 0x2400];

            //RAM MIRROR
            //else if (address >= 0x4000 && address < 0x4400)
            //    response = ram[address - 0x4000];

            //VRAM MIRROT
            //else if (address >= 0x4400 && address < 0X6000)
            //    response = vram[address - 0x4400];

            else
                throw new Exception("Out of range when reading memory address");

            return response;
        }

        public void WriteByte(ushort address, byte data)
        {
            if (address >= 0x2000 && address < 0x2400)
                ram[address - 0x2000] = data;

            else if (address >= 0x2400 && address < 0x4000)
                vram[address - 0x2400] = data;

            //RAM MIRROR
            else if (address >= 0x4000 && address < 0x4400)
                ram[address - 0x4000] = data;

            //VRAM MIRROR
            else if (address >= 0x4400 && address < 0X6000)
                vram[address - 0x4400] = data;

            //else
            //    throw new Exception("Out of range when writing on memory address");
        }
    }
}
