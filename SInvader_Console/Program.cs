using SInvader_Core;
using SInvader_Core.MMU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInvader_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Emulator emulator = Emulator.Instance;

            /* Instatiating Memory control unit for testing purpose */
            emulator.MCU = new MCUD();
            /* Emulating BDOS Call  to write on console screen the result of test*/
            emulator.MCU.WriteByte(0x05, 0xC9);
            /* Modifying program counter because all test start at 0x100 address*/
            emulator.CPU.PC = 0x100;

            //Starting the emulator in the main thread
            //emulator.PerformMultipleStep(@"C:\Test\spaceinvaders\i8080_Disassembler\Test\cputest.com", 0x100);
            //emulator.PerformMultipleStep(@"C:\Test\spaceinvaders\i8080_Disassembler\Test\tst8080.com", 0x100);
            //emulator.PerformMultipleStep(@"C:\Test\spaceinvaders\i8080_Disassembler\Test\8080pre.com", 0x100);
            //emulator.PerformMultipleStep(@"C:\Test\spaceinvaders\i8080_Disassembler\Test\8080exm.com", 0x100);
            emulator.PerformMultipleStep(@"C:\Test\spaceinvaders\i8080_Disassembler\Test\8080exer.com", 0x100);
            //emulator.PerformMultipleStep(@"C:\Test\spaceinvaders\i8080_Disassembler\Test\cpudiag.bin", 0x100);                     

            Console.WriteLine("\n");
            Console.WriteLine("Test Finished");
            Console.WriteLine("Press key to exit");
            Console.ReadKey();
        }
    }
}
