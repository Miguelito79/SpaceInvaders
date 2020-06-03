using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInvader_Core.i8080
{
    public class Instructions : Dictionary<ushort, Instruction>
    {
        public static Instructions Get()
        {
            Instructions instructions = new Instructions();

            instructions.Add(0x00, new Instruction(0x00, "NOP", 4, 0, 1));
            instructions.Add(0x01, new Instruction(0x01, "LXI B,d16", 10, 0, 3));
            instructions.Add(0x02, new Instruction(0x02, "STAX B", 7, 0, 1));
            instructions.Add(0x03, new Instruction(0x03, "INX B", 5, 0, 1));
            instructions.Add(0x04, new Instruction(0x04, "INR B", 5, 0, 1));
            instructions.Add(0x05, new Instruction(0x05, "DCR B", 5, 0, 1));
            instructions.Add(0x06, new Instruction(0x06, "MVI B,d8", 7, 0, 2));
            instructions.Add(0x07, new Instruction(0x07, "RLC", 4, 0, 1));
            instructions.Add(0x08, new Instruction(0x08, "NOP", 4, 0, 1));
            instructions.Add(0x09, new Instruction(0x09, "DAD B", 10, 0, 1));
            instructions.Add(0x0A, new Instruction(0x0A, "LDAX B", 7, 0, 1));
            instructions.Add(0x0B, new Instruction(0x0B, "DCX B", 5, 0, 1));
            instructions.Add(0x0C, new Instruction(0x0C, "INR C", 5, 0, 1));
            instructions.Add(0x0D, new Instruction(0x0D, "DCR C", 5, 0, 1));
            instructions.Add(0x0E, new Instruction(0x0E, "MVI C,d8", 7, 0, 2));
            instructions.Add(0x0F, new Instruction(0x0F, "RRC", 4, 0, 1)); 

            instructions.Add(0x10, new Instruction(0x10, "NOP", 4, 0, 1));
            instructions.Add(0x11, new Instruction(0x11, "LXI D,d16", 10, 0, 3));
            instructions.Add(0x12, new Instruction(0x12, "STAX D", 7, 0, 1));
            instructions.Add(0x13, new Instruction(0x13, "INX D", 5, 0, 1));
            instructions.Add(0x14, new Instruction(0x14, "INR D", 5, 0, 1));
            instructions.Add(0x15, new Instruction(0x15, "DCR D", 5, 0, 1));
            instructions.Add(0x16, new Instruction(0x16, "MVI D,d8", 7, 0, 2));
            instructions.Add(0x17, new Instruction(0x17, "RAL", 4, 0, 1));
            instructions.Add(0x18, new Instruction(0x18, "NOP", 4, 0, 1));
            instructions.Add(0x19, new Instruction(0x19, "DAD D", 10, 0, 1));
            instructions.Add(0x1A, new Instruction(0x1A, "LDAX D", 7, 0, 1));
            instructions.Add(0x1B, new Instruction(0x1B, "DCX D", 5, 0, 1));
            instructions.Add(0x1C, new Instruction(0x1C, "INR E", 5, 0, 1));
            instructions.Add(0x1D, new Instruction(0x1D, "DCR E", 5, 0, 1));
            instructions.Add(0x1E, new Instruction(0x1E, "MVI E,d8", 7, 0, 2));
            instructions.Add(0x1F, new Instruction(0x1F, "RAR", 4, 0, 1));

            instructions.Add(0x20, new Instruction(0x20, "NOP", 4, 0, 1));
            instructions.Add(0x21, new Instruction(0x21, "LXI H,d16", 10, 0, 3));
            instructions.Add(0x22, new Instruction(0x22, "SHLD", 16, 0, 3));
            instructions.Add(0x23, new Instruction(0x23, "INX H", 5, 0, 1));
            instructions.Add(0x24, new Instruction(0x24, "INR H", 5, 0, 1));
            instructions.Add(0x25, new Instruction(0x25, "DCR H", 5, 0, 1));
            instructions.Add(0x26, new Instruction(0x26, "MVI H,d8", 7, 0, 2));
            instructions.Add(0x27, new Instruction(0x27, "DAA", 4, 0, 1));
            instructions.Add(0x28, new Instruction(0x28, "NOP", 4, 0, 1));
            instructions.Add(0x29, new Instruction(0x29, "DAD H", 10, 0, 1));
            instructions.Add(0x2A, new Instruction(0x2A, "LHDL,a16", 16, 0, 3));
            instructions.Add(0x2B, new Instruction(0x2B, "DCX H", 5, 0, 1));
            instructions.Add(0x2C, new Instruction(0x2C, "INR L", 5, 0, 1));
            instructions.Add(0x2D, new Instruction(0x2D, "DCR L", 5, 0, 1));
            instructions.Add(0x2E, new Instruction(0x2E, "MVI L,d8", 7, 0, 2));
            instructions.Add(0x2F, new Instruction(0x2F, "CMA", 4, 0, 1));

            instructions.Add(0x30, new Instruction(0x30, "NOP", 4, 0, 1));
            instructions.Add(0x31, new Instruction(0x31, "LXI SP,d16", 10, 0, 3));
            instructions.Add(0x32, new Instruction(0x32, "STA a16", 13, 0, 3));
            instructions.Add(0x33, new Instruction(0x33, "INX SP", 5, 0, 1));
            instructions.Add(0x34, new Instruction(0x34, "INR M", 10, 0, 1));
            instructions.Add(0x35, new Instruction(0x35, "DCR M", 10, 0, 1));
            instructions.Add(0x36, new Instruction(0x36, "MVI M,d8", 10, 0, 2));
            instructions.Add(0x37, new Instruction(0x37, "STC", 4, 0, 1));
            instructions.Add(0x38, new Instruction(0x38, "NOP", 4, 0, 1));
            instructions.Add(0x39, new Instruction(0x39, "DAD SP", 10, 0, 1));
            instructions.Add(0x3A, new Instruction(0x3A, "LDA a16", 13, 0, 3));
            instructions.Add(0x3B, new Instruction(0x3B, "DCX SP", 5, 0, 1));
            instructions.Add(0x3C, new Instruction(0x3C, "INR A", 5, 0, 1));
            instructions.Add(0x3D, new Instruction(0x3D, "DCR A", 5, 0, 1));
            instructions.Add(0x3E, new Instruction(0x3E, "MVI A,d8", 7, 0, 2));
            instructions.Add(0x3F, new Instruction(0x3F, "CMC", 4, 0, 1));

            instructions.Add(0x40, new Instruction(0x40, "MOV B,B", 5, 0, 1));
            instructions.Add(0x41, new Instruction(0x41, "MOV B,C", 5, 0, 1));
            instructions.Add(0x42, new Instruction(0x42, "MOV B,D", 5, 0, 1));
            instructions.Add(0x43, new Instruction(0x43, "MOV B,E", 5, 0, 1));
            instructions.Add(0x44, new Instruction(0x44, "MOV B,H", 5, 0, 1));
            instructions.Add(0x45, new Instruction(0x45, "MOV B,L", 5, 0, 1));
            instructions.Add(0x46, new Instruction(0x46, "MOV B,M", 7, 0, 1));
            instructions.Add(0x47, new Instruction(0x47, "MOV B,A", 5, 0, 1));
            instructions.Add(0x48, new Instruction(0x48, "MOV C,B", 5, 0, 1));
            instructions.Add(0x49, new Instruction(0x49, "MOV C,C", 5, 0, 1));
            instructions.Add(0x4A, new Instruction(0x4A, "MOV C,D", 5, 0, 1));
            instructions.Add(0x4B, new Instruction(0x4B, "MOV C,E", 5, 0, 1));
            instructions.Add(0x4C, new Instruction(0x4C, "MOV C,H", 5, 0, 1));
            instructions.Add(0x4D, new Instruction(0x4D, "MOV C,L", 5, 0, 1));
            instructions.Add(0x4E, new Instruction(0x4E, "MOV C,M", 7, 0, 1));
            instructions.Add(0x4F, new Instruction(0x4F, "MOV C,A", 5, 0, 1));

            instructions.Add(0x50, new Instruction(0x50, "MOV D,B", 5, 0, 1));
            instructions.Add(0x51, new Instruction(0x51, "MOV D,C", 5, 0, 1));
            instructions.Add(0x52, new Instruction(0x52, "MOV D,D", 5, 0, 1));
            instructions.Add(0x53, new Instruction(0x53, "MOV D,E", 5, 0, 1));
            instructions.Add(0x54, new Instruction(0x54, "MOV D,H", 5, 0, 1));
            instructions.Add(0x55, new Instruction(0x55, "MOV D,L", 5, 0, 1));
            instructions.Add(0x56, new Instruction(0x56, "MOV D,M", 7, 0, 1));
            instructions.Add(0x57, new Instruction(0x57, "MOV D,A", 5, 0, 1));
            instructions.Add(0x58, new Instruction(0x58, "MOV E,B", 5, 0, 1));
            instructions.Add(0x59, new Instruction(0x59, "MOV E,C", 5, 0, 1));
            instructions.Add(0x5A, new Instruction(0x5A, "MOV E,D", 5, 0, 1));
            instructions.Add(0x5B, new Instruction(0x5B, "MOV E,E", 5, 0, 1));
            instructions.Add(0x5C, new Instruction(0x5C, "MOV E,H", 5, 0, 1));
            instructions.Add(0x5D, new Instruction(0x5D, "MOV E,L", 5, 0, 1));
            instructions.Add(0x5E, new Instruction(0x5E, "MOV E,M", 7, 0, 1));
            instructions.Add(0x5F, new Instruction(0x5F, "MOV E,A", 5, 0, 1));

            instructions.Add(0x60, new Instruction(0x60, "MOV H,B", 5, 0, 1));
            instructions.Add(0x61, new Instruction(0x61, "MOV H,C", 5, 0, 1));
            instructions.Add(0x62, new Instruction(0x62, "MOV H,D", 5, 0, 1));
            instructions.Add(0x63, new Instruction(0x63, "MOV H,E", 5, 0, 1));
            instructions.Add(0x64, new Instruction(0x64, "MOV H,H", 5, 0, 1));
            instructions.Add(0x65, new Instruction(0x65, "MOV H,L", 5, 0, 1));
            instructions.Add(0x66, new Instruction(0x66, "MOV H,M", 7, 0, 1));
            instructions.Add(0x67, new Instruction(0x67, "MOV H,A", 5, 0, 1));
            instructions.Add(0x68, new Instruction(0x68, "MOV L,B", 5, 0, 1));
            instructions.Add(0x69, new Instruction(0x69, "MOV L,C", 5, 0, 1));
            instructions.Add(0x6A, new Instruction(0x6A, "MOV L,D", 5, 0, 1));
            instructions.Add(0x6B, new Instruction(0x6B, "MOV L,E", 5, 0, 1));
            instructions.Add(0x6C, new Instruction(0x6C, "MOV L,H", 5, 0, 1));
            instructions.Add(0x6D, new Instruction(0x6D, "MOV L,L", 5, 0, 1));
            instructions.Add(0x6E, new Instruction(0x6E, "MOV L,M", 7, 0, 1));
            instructions.Add(0x6F, new Instruction(0x6F, "MOV L,A", 5, 0, 1));

            instructions.Add(0x70, new Instruction(0x70, "MOV M,B", 7, 0, 1));
            instructions.Add(0x71, new Instruction(0x71, "MOV M,C", 7, 0, 1));
            instructions.Add(0x72, new Instruction(0x72, "MOV M,D", 7, 0, 1));
            instructions.Add(0x73, new Instruction(0x73, "MOV M,E", 7, 0, 1));
            instructions.Add(0x74, new Instruction(0x74, "MOV M,H", 7, 0, 1));
            instructions.Add(0x75, new Instruction(0x75, "MOV M,L", 7, 0, 1));
            instructions.Add(0x76, new Instruction(0x76, "HLT", 7, 0, 1));
            instructions.Add(0x77, new Instruction(0x77, "MOV M,A", 7, 0, 1));
            instructions.Add(0x78, new Instruction(0x78, "MOV A,B", 5, 0, 1));
            instructions.Add(0x79, new Instruction(0x79, "MOV A,C", 5, 0, 1));
            instructions.Add(0x7A, new Instruction(0x7A, "MOV A,D", 5, 0, 1));
            instructions.Add(0x7B, new Instruction(0x7B, "MOV A,E", 5, 0, 1));
            instructions.Add(0x7C, new Instruction(0x7C, "MOV A,H", 5, 0, 1));
            instructions.Add(0x7D, new Instruction(0x7D, "MOV A,L", 5, 0, 1));
            instructions.Add(0x7E, new Instruction(0x7E, "MOV A,M", 7, 0, 1));
            instructions.Add(0x7F, new Instruction(0x7F, "MOV A,A", 5, 0, 1));

            instructions.Add(0x80, new Instruction(0x80, "ADD B", 4, 0, 1));
            instructions.Add(0x81, new Instruction(0x81, "ADD C", 4, 0, 1));
            instructions.Add(0x82, new Instruction(0x82, "ADD D", 4, 0, 1));
            instructions.Add(0x83, new Instruction(0x83, "ADD E", 4, 0, 1));
            instructions.Add(0x84, new Instruction(0x84, "ADD H", 4, 0, 1));
            instructions.Add(0x85, new Instruction(0x85, "ADD L", 4, 0, 1));
            instructions.Add(0x86, new Instruction(0x86, "ADD M", 7, 0, 1));
            instructions.Add(0x87, new Instruction(0x87, "ADD A", 4, 0, 1));
            instructions.Add(0x88, new Instruction(0x88, "ADC B", 4, 0, 1));
            instructions.Add(0x89, new Instruction(0x89, "ADC C", 4, 0, 1));
            instructions.Add(0x8A, new Instruction(0x8A, "ADC D", 4, 0, 1));
            instructions.Add(0x8B, new Instruction(0x8B, "ADC E", 4, 0, 1));
            instructions.Add(0x8C, new Instruction(0x8C, "ADC H", 4, 0, 1));
            instructions.Add(0x8D, new Instruction(0x8D, "ADC L", 4, 0, 1));
            instructions.Add(0x8E, new Instruction(0x8E, "ADC M", 7, 0, 1));
            instructions.Add(0x8F, new Instruction(0x8F, "ADC A", 4, 0, 1));

            instructions.Add(0x90, new Instruction(0x90, "SUB B", 4, 0, 1));
            instructions.Add(0x91, new Instruction(0x91, "SUB C", 4, 0, 1));
            instructions.Add(0x92, new Instruction(0x92, "SUB D", 4, 0, 1));
            instructions.Add(0x93, new Instruction(0x93, "SUB E", 4, 0, 1));
            instructions.Add(0x94, new Instruction(0x94, "SUB H", 4, 0, 1));
            instructions.Add(0x95, new Instruction(0x95, "SUB L", 4, 0, 1));
            instructions.Add(0x96, new Instruction(0x96, "SUB M", 7, 0, 1));
            instructions.Add(0x97, new Instruction(0x97, "SUB A", 4, 0, 1));
            instructions.Add(0x98, new Instruction(0x98, "SBB B", 4, 0, 1));
            instructions.Add(0x99, new Instruction(0x99, "SBB C", 4, 0, 1));
            instructions.Add(0x9A, new Instruction(0x9A, "SBB D", 4, 0, 1));
            instructions.Add(0x9B, new Instruction(0x9B, "SBB E", 4, 0, 1));
            instructions.Add(0x9C, new Instruction(0x9C, "SBB H", 4, 0, 1));
            instructions.Add(0x9D, new Instruction(0x9D, "SBB L", 4, 0, 1));
            instructions.Add(0x9E, new Instruction(0x9E, "SBB M", 7, 0, 1));
            instructions.Add(0x9F, new Instruction(0x9F, "SBB A", 4, 0, 1));

            instructions.Add(0xA0, new Instruction(0xA0, "ANA B", 4, 0, 1));
            instructions.Add(0xA1, new Instruction(0xA1, "ANA C", 4, 0, 1));
            instructions.Add(0xA2, new Instruction(0xA2, "ANA D", 4, 0, 1));
            instructions.Add(0xA3, new Instruction(0xA3, "ANA E", 4, 0, 1));
            instructions.Add(0xA4, new Instruction(0xA4, "ANA H", 4, 0, 1));
            instructions.Add(0xA5, new Instruction(0xA5, "ANA L", 4, 0, 1));
            instructions.Add(0xA6, new Instruction(0xA6, "ANA M", 7, 0, 1));
            instructions.Add(0xA7, new Instruction(0xA7, "ANA A", 4, 0, 1));
            instructions.Add(0xA8, new Instruction(0xA8, "XRA B", 4, 0, 1));
            instructions.Add(0xA9, new Instruction(0xA9, "XRA C", 4, 0, 1));
            instructions.Add(0xAA, new Instruction(0xAA, "XRA D", 4, 0, 1));
            instructions.Add(0xAB, new Instruction(0xAB, "XRA E", 4, 0, 1));
            instructions.Add(0xAC, new Instruction(0xAC, "XRA H", 4, 0, 1));
            instructions.Add(0xAD, new Instruction(0xAD, "XRA L", 4, 0, 1));
            instructions.Add(0xAE, new Instruction(0xAE, "XRA M", 7, 0, 1));
            instructions.Add(0xAF, new Instruction(0xAF, "XRA A", 4, 0, 1));

            instructions.Add(0xB0, new Instruction(0xB0, "ORA B", 4, 0, 1));
            instructions.Add(0xB1, new Instruction(0xB1, "ORA C", 4, 0, 1));
            instructions.Add(0xB2, new Instruction(0xB2, "ORA D", 4, 0, 1));
            instructions.Add(0xB3, new Instruction(0xB3, "ORA E", 4, 0, 1));
            instructions.Add(0xB4, new Instruction(0xB4, "ORA H", 4, 0, 1));
            instructions.Add(0xB5, new Instruction(0xB5, "ORA L", 4, 0, 1));
            instructions.Add(0xB6, new Instruction(0xB6, "ORA M", 7, 0, 1));
            instructions.Add(0xB7, new Instruction(0xB7, "ORA A", 4, 0, 1));
            instructions.Add(0xB8, new Instruction(0xB8, "CMP B", 4, 0, 1));
            instructions.Add(0xB9, new Instruction(0xB9, "CMP C", 4, 0, 1));
            instructions.Add(0xBA, new Instruction(0xBA, "CMP D", 4, 0, 1));
            instructions.Add(0xBB, new Instruction(0xBB, "CMP E", 4, 0, 1));
            instructions.Add(0xBC, new Instruction(0xBC, "CMP H", 4, 0, 1));
            instructions.Add(0xBD, new Instruction(0xBD, "CMP L", 4, 0, 1));
            instructions.Add(0xBE, new Instruction(0xBE, "CMP M", 7, 0, 1));
            instructions.Add(0xBF, new Instruction(0xBF, "CMP A", 4, 0, 1));

            instructions.Add(0xC0, new Instruction(0xC0, "RNZ", 11, 5, 1));
            instructions.Add(0xC1, new Instruction(0xC1, "POP B", 10, 0, 1));
            instructions.Add(0xC2, new Instruction(0xC2, "JNZ a16", 10, 0, 3));
            instructions.Add(0xC3, new Instruction(0xC3, "JMP a16", 10, 0, 3));
            instructions.Add(0xC4, new Instruction(0xC4, "CNZ a16", 17, 11, 3));
            instructions.Add(0xC5, new Instruction(0xC5, "PUSH B", 11, 0, 1));
            instructions.Add(0xC6, new Instruction(0xC6, "ADI d8", 7, 0, 2));
            instructions.Add(0xC7, new Instruction(0xC7, "RST 0", 11, 0, 1));
            instructions.Add(0xC8, new Instruction(0xC8, "RZ", 11, 5, 1));
            instructions.Add(0xC9, new Instruction(0xC9, "RET", 10, 0, 1));
            instructions.Add(0xCA, new Instruction(0xCA, "JZ a16", 10, 0, 3));
            instructions.Add(0xCB, new Instruction(0xCB, "JMP a16", 10, 0, 3));
            instructions.Add(0xCC, new Instruction(0xCC, "CZ a16", 17, 11, 3));
            instructions.Add(0xCD, new Instruction(0xCD, "CALL a16", 17, 0, 3));
            instructions.Add(0xCE, new Instruction(0xCE, "ACI d8", 7, 0, 2));
            instructions.Add(0xCF, new Instruction(0xCF, "RST 1", 11, 0, 1));

            instructions.Add(0xD0, new Instruction(0xD0, "RNC", 11, 5, 1));
            instructions.Add(0xD1, new Instruction(0xD1, "POP D", 10, 0, 1));
            instructions.Add(0xD2, new Instruction(0xD2, "JNC a16", 10, 0, 3));
            instructions.Add(0xD3, new Instruction(0xD3, "OUT d8", 10, 0, 2));
            instructions.Add(0xD4, new Instruction(0xD4, "CNC a16", 17, 11, 3));
            instructions.Add(0xD5, new Instruction(0xD5, "PUSH D", 11, 0, 1));
            instructions.Add(0xD6, new Instruction(0xD6, "SUI d8", 7, 0, 2));
            instructions.Add(0xD7, new Instruction(0xD7, "RST 2", 11, 0, 1));
            instructions.Add(0xD8, new Instruction(0xD8, "RC", 11, 5, 1));
            instructions.Add(0xD9, new Instruction(0xD9, "RET", 10, 0, 1));
            instructions.Add(0xDA, new Instruction(0xDA, "JC a16", 10, 0, 3));
            instructions.Add(0xDB, new Instruction(0xDB, "IN d8", 10, 0, 2));
            instructions.Add(0xDC, new Instruction(0xDC, "CC a16", 17, 11, 3));
            instructions.Add(0xDD, new Instruction(0xDD, "CALL a16", 17, 0, 3));
            instructions.Add(0xDE, new Instruction(0xDE, "SBI d8", 7, 0, 2));
            instructions.Add(0xDF, new Instruction(0xDF, "RST 3", 11, 0, 1));

            instructions.Add(0xE0, new Instruction(0xE0, "RPO", 11, 5, 1));
            instructions.Add(0xE1, new Instruction(0xE1, "POP H", 10, 0, 1));
            instructions.Add(0xE2, new Instruction(0xE2, "JPO a16", 10, 0, 3));
            instructions.Add(0xE3, new Instruction(0xE3, "XTHL", 18, 0, 1));
            instructions.Add(0xE4, new Instruction(0xE4, "CPO a16", 17, 11, 3));
            instructions.Add(0xE5, new Instruction(0xE5, "PUSH H", 11, 0, 1));
            instructions.Add(0xE6, new Instruction(0xE6, "ANI d8", 7, 0, 2));
            instructions.Add(0xE7, new Instruction(0xE7, "RST 4", 11, 0, 1));
            instructions.Add(0xE8, new Instruction(0xE8, "RPE", 11, 5, 1));
            instructions.Add(0xE9, new Instruction(0xE9, "PCHL", 5, 0, 1));
            instructions.Add(0xEA, new Instruction(0xEA, "JPE a16", 10, 0, 3));
            instructions.Add(0xEB, new Instruction(0xEB, "XCHG", 5, 0, 1));
            instructions.Add(0xEC, new Instruction(0xEC, "CPE a16", 17, 11, 3));
            instructions.Add(0xED, new Instruction(0xED, "CALL a16", 17, 0, 3));
            instructions.Add(0xEE, new Instruction(0xEE, "XRI d8", 7, 0, 2));
            instructions.Add(0xEF, new Instruction(0xEF, "RST 5", 11, 0, 1));

            instructions.Add(0xF0, new Instruction(0xF0, "RP", 11, 5, 1));
            instructions.Add(0xF1, new Instruction(0xF1, "POP PSW", 10, 0, 1));
            instructions.Add(0xF2, new Instruction(0xF2, "JP a16", 10, 0, 3));
            instructions.Add(0xF3, new Instruction(0xF3, "DI", 4, 0, 1));
            instructions.Add(0xF4, new Instruction(0xF4, "CP a16", 17, 11, 3));
            instructions.Add(0xF5, new Instruction(0xF5, "PUSH PSW", 11, 0, 1));
            instructions.Add(0xF6, new Instruction(0xF6, "ORI d8", 7, 0, 2));
            instructions.Add(0xF7, new Instruction(0xF7, "RST 6", 11, 0, 1));
            instructions.Add(0xF8, new Instruction(0xF8, "RM", 11, 5, 1));
            instructions.Add(0xF9, new Instruction(0xF9, "SPHL", 5, 0, 1));
            instructions.Add(0xFA, new Instruction(0xFA, "JM a16", 10, 0, 3));
            instructions.Add(0xFB, new Instruction(0xFB, "EI", 4, 0, 1));
            instructions.Add(0xFC, new Instruction(0xFC, "CM a16", 17, 11, 3));
            instructions.Add(0xFD, new Instruction(0xFD, "CALL a16", 17, 0, 3));
            instructions.Add(0xFE, new Instruction(0xFE, "CPI d8", 7, 0, 2));
            instructions.Add(0xFF, new Instruction(0xFF, "RST 7", 11, 0, 1));

            return instructions;
        }

        public void SetActions(CPU cpu)
        {
            this[0x00].action = new Action<Instruction>(cpu.Nop);
            this[0x01].action = new Action<Instruction>(cpu.LoadRegisterPairImmediate);
            this[0x02].action = new Action<Instruction>(cpu.StoreAccumulatorIndirect);
            this[0x03].action = new Action<Instruction>(cpu.IncrementRegisterPair);
            this[0x04].action = new Action<Instruction>(cpu.IncrementRegister);
            this[0x05].action = new Action<Instruction>(cpu.DecrementRegister);
            this[0x06].action = new Action<Instruction>(cpu.MoveImmediate);
            this[0x07].action = new Action<Instruction>(cpu.RotateLeft);
            this[0x08].action = new Action<Instruction>(cpu.Nop);
            this[0x09].action = new Action<Instruction>(cpu.AddRegisterPairToHandL);
            this[0x0A].action = new Action<Instruction>(cpu.LoadAccumulatorIndirect);
            this[0x0B].action = new Action<Instruction>(cpu.DecrementRegisterPair);
            this[0x0C].action = new Action<Instruction>(cpu.IncrementRegister);
            this[0x0D].action = new Action<Instruction>(cpu.DecrementRegister);
            this[0x0E].action = new Action<Instruction>(cpu.MoveImmediate);
            this[0x0F].action = new Action<Instruction>(cpu.RotateRight);

            this[0x10].action = new Action<Instruction>(cpu.Nop);
            this[0x11].action = new Action<Instruction>(cpu.LoadRegisterPairImmediate);
            this[0x12].action = new Action<Instruction>(cpu.StoreAccumulatorIndirect);
            this[0x13].action = new Action<Instruction>(cpu.IncrementRegisterPair);
            this[0x14].action = new Action<Instruction>(cpu.IncrementRegister);
            this[0x15].action = new Action<Instruction>(cpu.DecrementRegister);
            this[0x16].action = new Action<Instruction>(cpu.MoveImmediate);
            this[0x17].action = new Action<Instruction>(cpu.RotateLeftWithCarry);
            this[0x18].action = new Action<Instruction>(cpu.Nop);
            this[0x19].action = new Action<Instruction>(cpu.AddRegisterPairToHandL);
            this[0x1A].action = new Action<Instruction>(cpu.LoadAccumulatorIndirect);
            this[0x1B].action = new Action<Instruction>(cpu.DecrementRegisterPair);
            this[0x1C].action = new Action<Instruction>(cpu.IncrementRegister);
            this[0x1D].action = new Action<Instruction>(cpu.DecrementRegister);
            this[0x1E].action = new Action<Instruction>(cpu.MoveImmediate);
            this[0x1F].action = new Action<Instruction>(cpu.RotateRightWithCarry);

            this[0x20].action = new Action<Instruction>(cpu.Nop);
            this[0x21].action = new Action<Instruction>(cpu.LoadRegisterPairImmediate);
            this[0x22].action = new Action<Instruction>(cpu.StoreHlDirect);
            this[0x23].action = new Action<Instruction>(cpu.IncrementRegisterPair);
            this[0x24].action = new Action<Instruction>(cpu.IncrementRegister);
            this[0x25].action = new Action<Instruction>(cpu.DecrementRegister);
            this[0x26].action = new Action<Instruction>(cpu.MoveImmediate);
            this[0x27].action = new Action<Instruction>(cpu.DecimalAdjustAccumulator);
            this[0x28].action = new Action<Instruction>(cpu.Nop);
            this[0x29].action = new Action<Instruction>(cpu.AddRegisterPairToHandL);
            this[0x2A].action = new Action<Instruction>(cpu.LoadHLDirect);
            this[0x2B].action = new Action<Instruction>(cpu.DecrementRegisterPair);
            this[0x2C].action = new Action<Instruction>(cpu.IncrementRegister);
            this[0x2D].action = new Action<Instruction>(cpu.DecrementRegister);
            this[0x2E].action = new Action<Instruction>(cpu.MoveImmediate);
            this[0x2F].action = new Action<Instruction>(cpu.ComplementAccumulator);

            this[0x30].action = new Action<Instruction>(cpu.Nop);
            this[0x31].action = new Action<Instruction>(cpu.LoadRegisterPairImmediate);
            this[0x32].action = new Action<Instruction>(cpu.StoreAccumulatorDirect);
            this[0x33].action = new Action<Instruction>(cpu.IncrementRegisterPair);
            this[0x34].action = new Action<Instruction>(cpu.IncrementMemory);
            this[0x35].action = new Action<Instruction>(cpu.DecrementMemory);
            this[0x36].action = new Action<Instruction>(cpu.MoveToMemoryImmediate);
            this[0x37].action = new Action<Instruction>(cpu.SetCarry);
            this[0x38].action = new Action<Instruction>(cpu.Nop);
            this[0x39].action = new Action<Instruction>(cpu.AddRegisterPairToHandL);
            this[0x3A].action = new Action<Instruction>(cpu.LoadAccumulatorDirect);
            this[0x3B].action = new Action<Instruction>(cpu.DecrementRegisterPair);
            this[0x3C].action = new Action<Instruction>(cpu.IncrementRegister);
            this[0x3D].action = new Action<Instruction>(cpu.DecrementRegister);
            this[0x3E].action = new Action<Instruction>(cpu.MoveImmediate);
            this[0x3F].action = new Action<Instruction>(cpu.ComplementCarry);

            this[0x40].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x41].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x42].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x43].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x44].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x45].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x46].action = new Action<Instruction>(cpu.MoveFromMemory);
            this[0x47].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x48].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x49].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x4A].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x4B].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x4C].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x4D].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x4E].action = new Action<Instruction>(cpu.MoveFromMemory);
            this[0x4F].action = new Action<Instruction>(cpu.MoveRegister);

            this[0x50].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x51].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x52].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x53].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x54].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x55].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x56].action = new Action<Instruction>(cpu.MoveFromMemory);
            this[0x57].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x58].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x59].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x5A].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x5B].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x5C].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x5D].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x5E].action = new Action<Instruction>(cpu.MoveFromMemory);
            this[0x5F].action = new Action<Instruction>(cpu.MoveRegister);

            this[0x60].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x61].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x62].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x63].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x64].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x65].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x66].action = new Action<Instruction>(cpu.MoveFromMemory);
            this[0x67].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x68].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x69].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x6A].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x6B].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x6C].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x6D].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x6E].action = new Action<Instruction>(cpu.MoveFromMemory);
            this[0x6F].action = new Action<Instruction>(cpu.MoveRegister);

            this[0x70].action = new Action<Instruction>(cpu.MoveToMemory);
            this[0x71].action = new Action<Instruction>(cpu.MoveToMemory);
            this[0x72].action = new Action<Instruction>(cpu.MoveToMemory);
            this[0x73].action = new Action<Instruction>(cpu.MoveToMemory);
            this[0x74].action = new Action<Instruction>(cpu.MoveToMemory);
            this[0x75].action = new Action<Instruction>(cpu.MoveToMemory);
            this[0x76].action = new Action<Instruction>(cpu.Hlt);
            this[0x77].action = new Action<Instruction>(cpu.MoveToMemory);
            this[0x78].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x79].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x7A].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x7B].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x7C].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x7D].action = new Action<Instruction>(cpu.MoveRegister);
            this[0x7E].action = new Action<Instruction>(cpu.MoveFromMemory);
            this[0x7F].action = new Action<Instruction>(cpu.MoveRegister);

            this[0x80].action = new Action<Instruction>(cpu.AddRegister);
            this[0x81].action = new Action<Instruction>(cpu.AddRegister);
            this[0x82].action = new Action<Instruction>(cpu.AddRegister);
            this[0x83].action = new Action<Instruction>(cpu.AddRegister);
            this[0x84].action = new Action<Instruction>(cpu.AddRegister);
            this[0x85].action = new Action<Instruction>(cpu.AddRegister);
            this[0x86].action = new Action<Instruction>(cpu.AddMemory);
            this[0x87].action = new Action<Instruction>(cpu.AddRegister);
            this[0x88].action = new Action<Instruction>(cpu.AddRegisterWithCarry);
            this[0x89].action = new Action<Instruction>(cpu.AddRegisterWithCarry);
            this[0x8A].action = new Action<Instruction>(cpu.AddRegisterWithCarry);
            this[0x8B].action = new Action<Instruction>(cpu.AddRegisterWithCarry);
            this[0x8C].action = new Action<Instruction>(cpu.AddRegisterWithCarry);
            this[0x8D].action = new Action<Instruction>(cpu.AddRegisterWithCarry);
            this[0x8E].action = new Action<Instruction>(cpu.AddMemoryWithCarry);
            this[0x8F].action = new Action<Instruction>(cpu.AddRegisterWithCarry);

            this[0x90].action = new Action<Instruction>(cpu.SubtractRegister);
            this[0x91].action = new Action<Instruction>(cpu.SubtractRegister);
            this[0x92].action = new Action<Instruction>(cpu.SubtractRegister);
            this[0x93].action = new Action<Instruction>(cpu.SubtractRegister);
            this[0x94].action = new Action<Instruction>(cpu.SubtractRegister);
            this[0x95].action = new Action<Instruction>(cpu.SubtractRegister);
            this[0x96].action = new Action<Instruction>(cpu.SubtractMemory);
            this[0x97].action = new Action<Instruction>(cpu.SubtractRegister);
            this[0x98].action = new Action<Instruction>(cpu.SubtractRegisterWithBorrow);
            this[0x99].action = new Action<Instruction>(cpu.SubtractRegisterWithBorrow);
            this[0x9A].action = new Action<Instruction>(cpu.SubtractRegisterWithBorrow);
            this[0x9B].action = new Action<Instruction>(cpu.SubtractRegisterWithBorrow);
            this[0x9C].action = new Action<Instruction>(cpu.SubtractRegisterWithBorrow);
            this[0x9D].action = new Action<Instruction>(cpu.SubtractRegisterWithBorrow);
            this[0x9E].action = new Action<Instruction>(cpu.SubtractMemoryWithBorrow);
            this[0x9F].action = new Action<Instruction>(cpu.SubtractRegisterWithBorrow);

            this[0xA0].action = new Action<Instruction>(cpu.AndRegister);
            this[0xA1].action = new Action<Instruction>(cpu.AndRegister);
            this[0xA2].action = new Action<Instruction>(cpu.AndRegister);
            this[0xA3].action = new Action<Instruction>(cpu.AndRegister);
            this[0xA4].action = new Action<Instruction>(cpu.AndRegister);
            this[0xA5].action = new Action<Instruction>(cpu.AndRegister);
            this[0xA6].action = new Action<Instruction>(cpu.AndMemory);
            this[0xA7].action = new Action<Instruction>(cpu.AndRegister);
            this[0xA8].action = new Action<Instruction>(cpu.ExclusiveOr);
            this[0xA9].action = new Action<Instruction>(cpu.ExclusiveOr);
            this[0xAA].action = new Action<Instruction>(cpu.ExclusiveOr);
            this[0xAB].action = new Action<Instruction>(cpu.ExclusiveOr);
            this[0xAC].action = new Action<Instruction>(cpu.ExclusiveOr);
            this[0xAD].action = new Action<Instruction>(cpu.ExclusiveOr);
            this[0xAE].action = new Action<Instruction>(cpu.ExclusiveOrMemory);
            this[0xAF].action = new Action<Instruction>(cpu.ExclusiveOr);

            this[0xB0].action = new Action<Instruction>(cpu.ORRegister);
            this[0xB1].action = new Action<Instruction>(cpu.ORRegister);
            this[0xB2].action = new Action<Instruction>(cpu.ORRegister);
            this[0xB3].action = new Action<Instruction>(cpu.ORRegister);
            this[0xB4].action = new Action<Instruction>(cpu.ORRegister);
            this[0xB5].action = new Action<Instruction>(cpu.ORRegister);
            this[0xB6].action = new Action<Instruction>(cpu.ORMemory);
            this[0xB7].action = new Action<Instruction>(cpu.ORRegister);
            this[0xB8].action = new Action<Instruction>(cpu.CompareRegister);
            this[0xB9].action = new Action<Instruction>(cpu.CompareRegister);
            this[0xBA].action = new Action<Instruction>(cpu.CompareRegister);
            this[0xBB].action = new Action<Instruction>(cpu.CompareRegister);
            this[0xBC].action = new Action<Instruction>(cpu.CompareRegister);
            this[0xBD].action = new Action<Instruction>(cpu.CompareRegister);
            this[0xBE].action = new Action<Instruction>(cpu.CompareMemory);
            this[0xBF].action = new Action<Instruction>(cpu.CompareRegister);

            this[0xC0].action = new Action<Instruction>(cpu.ReturnConditional);
            this[0xC1].action = new Action<Instruction>(cpu.PopRP);
            this[0xC2].action = new Action<Instruction>(cpu.JumpConditional);
            this[0xC3].action = new Action<Instruction>(cpu.Jump);
            this[0xC4].action = new Action<Instruction>(cpu.CallConditional);
            this[0xC5].action = new Action<Instruction>(cpu.PushRP);
            this[0xC6].action = new Action<Instruction>(cpu.AddImmediate);
            this[0xC7].action = new Action<Instruction>(cpu.RST);
            this[0xC8].action = new Action<Instruction>(cpu.ReturnConditional);
            this[0xC9].action = new Action<Instruction>(cpu.Return);
            this[0xCA].action = new Action<Instruction>(cpu.JumpConditional);
            this[0xCB].action = new Action<Instruction>(cpu.Jump);
            this[0xCC].action = new Action<Instruction>(cpu.CallConditional);
            this[0xCD].action = new Action<Instruction>(cpu.CallUnconditional);
            this[0xCE].action = new Action<Instruction>(cpu.AddImmediateWithCarry);
            this[0xCF].action = new Action<Instruction>(cpu.RST);

            this[0xD0].action = new Action<Instruction>(cpu.ReturnConditional);
            this[0xD1].action = new Action<Instruction>(cpu.PopRP);
            this[0xD2].action = new Action<Instruction>(cpu.JumpConditional);
            this[0xD3].action = new Action<Instruction>(cpu.Out);
            this[0xD4].action = new Action<Instruction>(cpu.CallConditional);
            this[0xD5].action = new Action<Instruction>(cpu.PushRP);
            this[0xD6].action = new Action<Instruction>(cpu.SubtractImmediate);
            this[0xD7].action = new Action<Instruction>(cpu.RST);
            this[0xD8].action = new Action<Instruction>(cpu.ReturnConditional);
            this[0xD9].action = new Action<Instruction>(cpu.Return);
            this[0xDA].action = new Action<Instruction>(cpu.JumpConditional);
            this[0xDB].action = new Action<Instruction>(cpu.In);
            this[0xDC].action = new Action<Instruction>(cpu.CallConditional);
            this[0xDD].action = new Action<Instruction>(cpu.CallUnconditional);
            this[0xDE].action = new Action<Instruction>(cpu.SubtractImmediateWithBorrow);
            this[0xDF].action = new Action<Instruction>(cpu.RST);

            this[0xE0].action = new Action<Instruction>(cpu.ReturnConditional);
            this[0xE1].action = new Action<Instruction>(cpu.PopRP);
            this[0xE2].action = new Action<Instruction>(cpu.JumpConditional);
            this[0xE3].action = new Action<Instruction>(cpu.Xthl);
            this[0xE4].action = new Action<Instruction>(cpu.CallConditional);
            this[0xE5].action = new Action<Instruction>(cpu.PushRP);
            this[0xE6].action = new Action<Instruction>(cpu.AndImmediate);
            this[0xE7].action = new Action<Instruction>(cpu.RST);
            this[0xE8].action = new Action<Instruction>(cpu.ReturnConditional);
            this[0xE9].action = new Action<Instruction>(cpu.PCHL);
            this[0xEA].action = new Action<Instruction>(cpu.JumpConditional);
            this[0xEB].action = new Action<Instruction>(cpu.ExchangeHLwithDE);
            this[0xEC].action = new Action<Instruction>(cpu.CallConditional);
            this[0xED].action = new Action<Instruction>(cpu.CallUnconditional);
            this[0xEE].action = new Action<Instruction>(cpu.ExclusiveOrImmediate);
            this[0xEF].action = new Action<Instruction>(cpu.RST);

            this[0xF0].action = new Action<Instruction>(cpu.ReturnConditional);
            this[0xF1].action = new Action<Instruction>(cpu.PopPSW);
            this[0xF2].action = new Action<Instruction>(cpu.JumpConditional);
            this[0xF3].action = new Action<Instruction>(cpu.DI);
            this[0xF4].action = new Action<Instruction>(cpu.CallConditional);
            this[0xF5].action = new Action<Instruction>(cpu.PushPSW);
            this[0xF6].action = new Action<Instruction>(cpu.ORImmediate);
            this[0xF7].action = new Action<Instruction>(cpu.RST);
            this[0xF8].action = new Action<Instruction>(cpu.ReturnConditional);
            this[0xF9].action = new Action<Instruction>(cpu.SPHL);
            this[0xFA].action = new Action<Instruction>(cpu.JumpConditional);
            this[0xFB].action = new Action<Instruction>(cpu.EI);
            this[0xFC].action = new Action<Instruction>(cpu.CallConditional);
            this[0xFD].action = new Action<Instruction>(cpu.CallUnconditional);
            this[0xFE].action = new Action<Instruction>(cpu.CompareImmediate);
            this[0xFF].action = new Action<Instruction>(cpu.RST);
        }
    }

    public class Instruction
    {
        public Instruction(byte opcode, string mnemonic, int tCycles, int aCycles, ushort length)
        {
            this.opcode = opcode;
            this.mnemonic = mnemonic;
            this.tCycles = tCycles;
            this.aCycles = aCycles;
            this.length = length;
        }

        public Instruction(byte opcode, string mnemonic, int tCycles, int aCycles, ushort length, Action<Instruction> action)
        {
            this.opcode = opcode;
            this.mnemonic = mnemonic;
            this.tCycles = tCycles;
            this.aCycles = aCycles;
            this.length = length;
            this.action = action;
        }

        //Action to be performed by instruction
        public Action<Instruction> action;

        //Instruction Opcode
        public byte opcode;
        //Mnemonic Code
        public string mnemonic { get; private set; }
        //Clock Cycles
        public int tCycles { get; private set; }
        //Number of cycles to be considered when the action is not taken
        public int aCycles { get; private set; }
        //Length of instruction (max 3 bytes)
        public ushort length { get; private set; }
    }
}
