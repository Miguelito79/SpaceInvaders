using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.AccessControl;
using System.Security.Cryptography;
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
        private string spaceInvadersHash = "7446E0994117596DE5206519E693F8875FF3455E0BE121D5CB975C3BCC224C4E";

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

        public override bool LoadFile(string fullPath)
        {            
            bool response = false;

            try
            {
                FileInfo fileInfo = new FileInfo(fullPath);
                if (fileInfo.Extension == ".zip")
                {
                    Dictionary<string, byte[]> entries = UnzipFile(fullPath);

                    string[] orderedEntries = { "invaders.h", "invaders.g",
                                                "invaders.f", "invaders.e" };

                    response = true;
                    for (int count = 0, offset = 0; count < orderedEntries.Length; count++, offset += 2048)
                    {
                        if(!LoadDataInMemoyAt(entries[orderedEntries[count]], offset))
                        {
                            response = false;
                            break;
                        }
                    }

                    response = VerifyLoadedContent();
                }
                else
                {
                    response = LoadFileInMemoryAt(fullPath, 0);
                }
            }
            catch (Exception ex)
            {
                response = false;
            }

            return response;
        }

        private bool VerifyLoadedContent()
        {
            bool response = false;

            using (SHA256Managed sha256 = new SHA256Managed())
            {
                byte[] hash = sha256.ComputeHash(buffer[0]);
                string hashHex = BitConverter.ToString(hash).Replace("-", "");
                response = hashHex == spaceInvadersHash;
            }
            
            return response;
        }

        private Dictionary<string, byte[]> UnzipFile(string fullPath)
        {
            Dictionary<string, byte[]> entries = new Dictionary<string, byte[]>();

            using (FileStream fs = new FileStream(fullPath, FileMode.Open))
            {
                ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Read);
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    using (Stream entryStream = entry.Open())
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            entryStream.CopyTo(memoryStream);
                            entries.Add(entry.Name, memoryStream.ToArray());
                        }
                    }
                }
            }

            return entries;
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


        /// <summary>
        /// Load input data in memory at specific offset
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Read byte from memory address specified in input
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
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
            else if (address >= 0x4000 && address < 0x4400)
                response = buffer[1][address - 0x4000];

            //VRAM MIRROR
            else if (address >= 0x4400 && address < 0X6000)
                response = buffer[2][address - 0x4400];            

            return response;
        }

        /// <summary>
        /// Write byte into to the memory address specified in input
        /// </summary>
        /// <param name="address">Memory address data will be written</param>
        /// <param name="data">Data to be write in memory</param>
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

            //else
            //    throw new Exception("Out of range when writing memory address");
        }
    }
}
