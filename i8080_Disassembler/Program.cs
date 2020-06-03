using i8080_Disassembler.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i8080_Disassembler
{
    class Program
    {
        static void Main(string[] args)
        {
            BusinessManager businessManager = new BusinessManager();
            businessManager.Disassembly(@"C:\Test\spaceinvaders\i8080_Disassembler\Test\cputest.com", @"C:\temp\cputest.dis", 0x100);
        }
    }
}
