using Microsoft.SqlServer.Server;
using SInvader_Core.i8080;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace i8080_Disassembler.BusinessLayer
{
    public class BusinessManager
    {     
        byte[] buffer = new byte[64 * 1024];
        Instructions _instructions = Instructions.Get();

        public bool LoadFileInMemoryAt(string path, long offset)
        {
            bool response = true;

            try
            {
                long startingAddress = offset;
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    int numBytesToRead = (int)stream.Length;
                    buffer = new byte[numBytesToRead + offset];

                    if (numBytesToRead > 0)
                    {
                        do
                        {
                            byte value = (byte)stream.ReadByte();
                            buffer[startingAddress] = value;

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

            //PUTTING A RET TO RETURN FROM BDOS CALL
            buffer[0x05] = 0xC9;

            return response;
        }

        public void Disassembly(string inputFullPath, string outputFullPath, int offset)
        {
            int programCounter = offset;
                
            if (LoadFileInMemoryAt(inputFullPath, offset))
            {
                using (StreamWriter sw = new StreamWriter(outputFullPath))
                {
                    for (int count = offset; count < buffer.Length; count++)
                    {
                        byte opcode = buffer[count];
                        Instruction instruction;

                        if (_instructions.TryGetValue(opcode, out instruction))
                        {
                            string parsedInstruction = ParseMnemonic(instruction, count);

                            switch (instruction.length)
                            {
                                case 1:                                    
                                    sw.WriteLine("{0:X4}    {1:X2}             {2}", count, buffer[count], parsedInstruction);
                                    break;
                                case 2:                                    
                                    sw.WriteLine("{0:X4}    {1:X2} {2:X2}          {3}", count,  buffer[count], buffer[count + 1], parsedInstruction);
                                    count += 1;
                                    break;
                                case 3:
                                    sw.WriteLine("{0:X4}    {1:X2} {2:X2} {3:X2}       {4}", count, buffer[count], buffer[count + 1], buffer[count + 2], parsedInstruction);
                                    count += 2;
                                    break;
                            }                            
                        }
                        else
                        {
                            throw new Exception("Unimplemented instruction");
                        }
                    }
                }
            }
        }

        public string ParseMnemonic(Instruction instruction, int count)
        {
            string response = instruction.mnemonic;

            List<string> splitted = instruction.mnemonic.Split(',').ToList();

            if (splitted.Count > 1)
            {
                if (splitted.Contains("d8"))
                    response = String.Format("{0},{1:X2}", splitted[0], buffer[count + 1]);
                else if (splitted.Contains("a16"))
                    response = String.Format("{0},{1:X4}", splitted[0], (buffer[count + 2] << 8 | buffer[count + 1]));
            }
            else
            {
                if (instruction.mnemonic.Contains("d8"))
                    response = instruction.mnemonic.Replace("d8", buffer[count + 1].ToString("X2"));
                else if (instruction.mnemonic.Contains("a16"))              
                    response = instruction.mnemonic.Replace("a16", ((buffer[count + 2] << 8) | buffer[count + 1]).ToString("X4"));                               
            }
            return response;
        }
    }
}


