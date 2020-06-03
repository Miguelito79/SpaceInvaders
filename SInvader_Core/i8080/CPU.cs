using SInvader_Core.IO;
using SInvader_Core.MMU;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace SInvader_Core.i8080
{
    public class CPU
    {
        //memory control unit
        public MCU MCU
        {
            get { return Emulator.Instance.MCU; }
        }

        //Input device list connected to this CPU
        private Dictionary<byte, IInputDevice> InputDeviceList
        {
            get { return Emulator.Instance.Devices.InputDeviceList; }
        }

        //Output device list connected to this CPU
        private Dictionary<byte, IOutputDevice> OutputDeviceList
        {
            get { return Emulator.Instance.Devices.OutputDeviceList; }
        }

        private Instructions _instructions; //Instruction set       
        public Instructions Instructions
        {
            get { return _instructions; }
            private set { }
        }

        bool _interrputEnabled; //Interrupt enabled flag
        public bool InterruptEnabled
        {
            get { return _interrputEnabled; }
            private set { }
        }

        bool _halt; //Halt execution of CPU
        public bool Halted
        {
            get { return _halt; }
            private set { }
        }

        private int _tCycles; //Number of cpu clock cycles               
        public int TCycles
        {
            get { return _tCycles; }
            private set { }
        }
        private Queue<byte> _interruptQueue;
        public Queue<byte> InterruptQueue
        {
            get { return _interruptQueue; }
            private set { }
        }

        private byte _opcode; //Current opcode                               
        public ushort PC; //Program counter
        public ushort SP; //Stack pointer               

        #region Registers
        public Registers _registers; //Cpu registers

        public struct Registers
        {
            public byte a; //Accumulator
            public byte c;
            public byte d;
            public byte e;
            public byte h;
            public byte l;
            public byte b;

            public ushort BC
            {
                get { return (ushort)((b << 8) | c); }
                set
                {
                    b = (byte)(value >> 8);
                    c = (byte)(value);
                }
            }

            public ushort DE
            {
                get { return (ushort)((d << 8) | e); }
                set
                {
                    d = (byte)(value >> 8);
                    e = (byte)(value);
                }
            }

            public ushort HL
            {
                get { return (ushort)((h << 8) | l); }
                set
                {
                    h = (byte)(value >> 8);
                    l = (byte)(value);
                }
            }
        }

        public ref byte GetRegisterByOpcode(byte opcode)
        {
            switch (opcode)
            {
                case 0x38: //00 111 000
                    return ref _registers.a;
                case 0x00: //00 000 000
                    return ref _registers.b;
                case 0x08: //00 001 000
                    return ref _registers.c;
                case 0x10:  //00 010 000:
                    return ref _registers.d;
                case 0x18: //00 011 000
                    return ref _registers.e;
                case 0x20: //00 100 000
                    return ref _registers.h;
                case 0x28: //00 101 000
                    return ref _registers.l;
                default:
                    throw new Exception("Undetected register in opcode");
            }
        }        
        #endregion

        #region Memory read functions
        private byte NextByte()
        {
            return MCU.ReadByte((ushort)(this.PC + 1));
        }

        private ushort NextWord()
        {            
            return (ushort)(ushort)(MCU.ReadByte((ushort)(this.PC + 1)) | (MCU.ReadByte((ushort)(this.PC + 2)) << 8));
        }
        #endregion

        #region Flag Operators
        byte f; //flag register

        public int GetFlagConditionByOpcode(byte opcode)
        {
            switch (opcode)
            {
                case 0x00:  //00 000 000
                case 0x08:  //00 001 000
                    return ZFlag;
                case 0x10:  //00 010 000 
                case 0x18:  //00 011 000
                    return CYFlag;
                case 0x20:  //00 100 000
                case 0x28:  //00 101 000
                    return PFlag;
                case 0x30:  //00 110 000
                case 0x38:  //00 111 000
                    return SFlag;
                default:
                    throw new Exception("Undetected flag by opcode");
            }
        }

        //SIGN FLAG - BIT 7
        public int SFlag
        {
            get { return (f & 0x80) >> 7; }
            set { f = (byte)((f & ~(1 << 7)) | (value << 7)); }
        }        

        //ZERO FLAG - BIT 6
        public int ZFlag
        {
            get { return (f & 0x40) >> 6; }
            set { f = (byte)((f & ~(1 << 6)) | (value << 6)); }
        }       

        //BIT 5 UNUSED - ALWAYS 0 


        // AUXILIARY CARRY FLAG - BIT 4
        public int ACFlag
        {
            get { return (f & 0x10) >> 4; }
            set { f = (byte)((f & ~(1 << 4)) | (value << 4)); }
        }

        //BIT 3 UNUSED - ALWAYS 0

        //BIT 2 UNUSED - ALWAYS 1

        // PARITY FLAG
        public int PFlag
        {
            get { return (f & 0x04) >> 2; }
            set { f = (byte)((f & ~(1 << 2)) | (value << 2)); }
        }

        // CARRY FLAG
        public int CYFlag
        {
            get { return (f & 0x01); }
            set { f = (byte)((f & ~(1)) | value); }
        }        
        #endregion

        public void Initialize()
        {            
            PC = 0x0000;
            SP = 0x0000;

            _interruptQueue = new Queue<byte>();
            _interrputEnabled = false;
            _halt = false;

            _registers.BC = 0x0000;
            _registers.DE = 0x0000;
            _registers.HL = 0x0000;

            _registers.a = 0;
            f = 0x02;

            //Initialize instruction set 
            _instructions = Instructions.Get();
            _instructions.SetActions(this);                 
        }

        #region DATA TRANSFER GROUP INSTRUCTIONS
        public void MoveRegister(Instruction instruction)
        {
            byte opcodeDestinationRegister = (byte) (instruction.opcode & 0x38);
            byte opcodeSourceRegister = (byte)((instruction.opcode & 0x07) << 3);

            //Assigning by reference 
            GetRegisterByOpcode(opcodeDestinationRegister) = GetRegisterByOpcode(opcodeSourceRegister);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        public void MoveFromMemory(Instruction instruction)
        {
            byte opcodeDestinationRegister = (byte)(instruction.opcode & 0x38);
            GetRegisterByOpcode(opcodeDestinationRegister) = MCU.ReadByte(_registers.HL);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        public void MoveToMemory(Instruction instruction)
        {
            byte opcodeSourceRegister = (byte)((instruction.opcode & 0x07) << 3);
            MCU.WriteByte(_registers.HL, GetRegisterByOpcode(opcodeSourceRegister));

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// MVI r,data
        /// </summary>
        /// <param name="instruction"></param>
        public void MoveImmediate(Instruction instruction)
        {
            byte opcodeDestinationRegister = (byte)(instruction.opcode & 0x38);
            GetRegisterByOpcode(opcodeDestinationRegister) = NextByte();            

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        public void MoveToMemoryImmediate(Instruction instruction)
        {
            MCU.WriteByte(_registers.HL, NextByte());

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// LXI H
        /// </summary>
        /// <param name="instruction"></param>
        public void LoadRegisterPairImmediate(Instruction instruction)
        { 
            byte opcodeRegisterPair = (byte)((instruction.opcode & 0x30) >> 4);
            switch (opcodeRegisterPair)
            {
                case 0x00:
                    _registers.BC = NextWord();                    
                    break;
                case 0x01:
                    _registers.DE = NextWord();
                    break;
                case 0x02:
                    _registers.HL = NextWord();
                    break;
                case 0x03:
                    SP = NextWord();
                    break;
            }          

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// LDA A
        /// </summary>
        /// <param name="instruction"></param>
        public void LoadAccumulatorDirect(Instruction instruction)
        {
            _registers.a = MCU.ReadByte(NextWord());

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// STA a16
        /// </summary>
        /// <param name="instruction"></param>
        public void StoreAccumulatorDirect(Instruction instruction)
        {
            MCU.WriteByte(NextWord(), _registers.a);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        public void LoadHLDirect(Instruction instruction)       
        {
            ushort address = NextWord();

            _registers.l = MCU.ReadByte(address);
            _registers.h = MCU.ReadByte((ushort)(address + 1));

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// SHLD
        /// </summary>
        /// <param name="instruction"></param>
        public void StoreHlDirect(Instruction instruction)
        {
            ushort address = NextWord();

            MCU.WriteByte(address, _registers.l);
            MCU.WriteByte((ushort)(address + 1), _registers.h);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// LDAX
        /// </summary>
        /// <param name="instruction"></param>
        public void LoadAccumulatorIndirect(Instruction instruction)
        {
            byte opcodeRegisterPair = (byte)((instruction.opcode & 0x30) >> 4);
            switch (opcodeRegisterPair)
            {
                case 0x00:
                    _registers.a = MCU.ReadByte(_registers.BC);                    
                    break;
                case 0x01:
                    _registers.a = MCU.ReadByte(_registers.DE);                    
                    break;
                default:
                    throw new Exception("Unidentified register pair in LDAX");
            }

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// STAX
        /// </summary>
        /// <param name="instruction"></param>
        public void StoreAccumulatorIndirect(Instruction instruction)
        {
            byte opcodeRegisterPair = (byte)((instruction.opcode & 0x30) >> 4);
            switch (opcodeRegisterPair)
            {
                case 0x00:
                    MCU.WriteByte(_registers.BC, _registers.a);
                    break;
                case 0x01:
                    MCU.WriteByte(_registers.DE, _registers.a);
                    break;
                default:
                    throw new Exception("Unidentified register pair in LDAX");
            }

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        public void ExchangeHLwithDE(Instruction instruction)
        {
            ushort previousContent = _registers.HL;

            _registers.HL = _registers.DE;
            _registers.DE = previousContent;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }
        #endregion

        #region ARITHMETIC GROUP
        public void AddRegister(Instruction instruction)
        {
            byte opcodeSourceRegister = (byte)((instruction.opcode & 0x07) << 3);
            byte sourceRegister = GetRegisterByOpcode(opcodeSourceRegister);

            ushort result = (ushort) (_registers.a + sourceRegister);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);
            ACFlag = ComputeACFlag(_registers.a, sourceRegister);
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        public void AddMemory(Instruction instruction)
        {
            byte data = MCU.ReadByte(_registers.HL);
            ushort result = (ushort)(_registers.a + data);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);
            ACFlag = ComputeACFlag(_registers.a, data);
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// ADI d8
        /// </summary>
        /// <param name="instruction"></param>
        public void AddImmediate(Instruction instruction)
        {
            byte data = NextByte();
            ushort result = (ushort)(_registers.a + data);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);
            ACFlag = ComputeACFlag(_registers.a, data);
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// ADC r
        /// </summary>
        /// <param name="instruction"></param>
        public void AddRegisterWithCarry(Instruction instruction)
        {
            byte opcodeSourceRegister = (byte)((instruction.opcode & 0x07) << 3);
            byte sourceRegister = GetRegisterByOpcode(opcodeSourceRegister);

            ushort result = (ushort)(_registers.a + sourceRegister + CYFlag);            

            if (CYFlag == 1)
                ACFlag = ComputeACFlagWithCarry(_registers.a, sourceRegister);
            else
                ACFlag = ComputeACFlag(_registers.a, sourceRegister);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// ADC M
        /// </summary>
        /// <param name="instruction"></param>
        public void AddMemoryWithCarry(Instruction instruction)
        {
            byte data = MCU.ReadByte(_registers.HL);
            ushort result = (ushort)(_registers.a + data + CYFlag);

            if (CYFlag == 1)
                ACFlag = ComputeACFlagWithCarry(_registers.a, data);
            else
                ACFlag = ComputeACFlag(_registers.a, data);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);            
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// ACI d8
        /// </summary>
        /// <param name="instruction"></param>
        public void AddImmediateWithCarry(Instruction instruction)
        {
            byte data = NextByte();
            ushort result = (ushort)(_registers.a + data + CYFlag);            

            if (CYFlag == 1)
                ACFlag = ComputeACFlagWithCarry(_registers.a, data);
            else
                ACFlag = ComputeACFlag(_registers.a, data);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);
            PFlag = ComputeParity((byte)result);
            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// SBC r
        /// </summary>
        /// <param name="instruction"></param>
        public void SubtractRegister(Instruction instruction)
        {
            byte opcodeSourceRegister = (byte)((instruction.opcode & 0x07)  << 3);
            byte sourceRegister = GetRegisterByOpcode(opcodeSourceRegister);

            ushort result = (ushort)(_registers.a - sourceRegister);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);              
            CYFlag = ComputeCYFlag(result);           
            ACFlag = ComputeACFlagOnSub(_registers.a, sourceRegister);
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// SBC M
        /// </summary>
        /// <param name="instruction"></param>
        public void SubtractMemory(Instruction instruction)
        {
            byte data = MCU.ReadByte(_registers.HL);
            ushort result = (ushort)(_registers.a - data);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);                            
            ACFlag = ComputeACFlagOnSub(_registers.a, data);
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// SUI d8
        /// </summary>
        /// <param name="instruction"></param>
        public void SubtractImmediate(Instruction instruction)
        {            
            byte data = NextByte();            
            ushort result = (ushort)(_registers.a - data);            

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);            
            ACFlag = ComputeACFlagOnSub(_registers.a, data);
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// SBB r
        /// </summary>
        /// <param name="instruction"></param>
        public void SubtractRegisterWithBorrow(Instruction instruction)
        {
            byte opcodeSourceRegister = (byte)((instruction.opcode & 0x07) << 3);
            byte sourceRegister = GetRegisterByOpcode(opcodeSourceRegister);

            ushort result = (ushort)(_registers.a - sourceRegister - CYFlag);

            if (CYFlag == 1)
                ACFlag = ComputeACFlagOnSubCarry(_registers.a, sourceRegister);
            else
                ACFlag = ComputeACFlagOnSub(_registers.a, sourceRegister);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);                       
            PFlag = ComputeParity((byte)result);

           _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// SBB M
        /// </summary>
        /// <param name="instruction"></param>
        public void SubtractMemoryWithBorrow(Instruction instruction)
        {
            byte data = MCU.ReadByte(_registers.HL);
            ushort result = (ushort)(_registers.a - data - CYFlag);            

            if (CYFlag == 1)
                ACFlag = ComputeACFlagOnSubCarry(_registers.a, data);
            else
                ACFlag = ComputeACFlagOnSub(_registers.a, data);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// SBI d8
        /// </summary>
        /// <param name="instruction"></param>
        public void SubtractImmediateWithBorrow(Instruction instruction)
        {
            byte data = NextByte();            
            ushort result = (ushort)(_registers.a - data - CYFlag);

            if (CYFlag == 1)
                ACFlag = ComputeACFlagOnSubCarry(_registers.a, data);
            else
                ACFlag = ComputeACFlagOnSub(_registers.a, data);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);            
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)(result);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// INR r
        /// </summary>
        /// <param name="instruction"></param>
        public void IncrementRegister(Instruction instruction)
        {
            byte opcodeDestinationRegister = (byte)(instruction.opcode & 0x38);
            byte destinationRegister = GetRegisterByOpcode(opcodeDestinationRegister);

            ushort result = (ushort)(destinationRegister + 1);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            ACFlag = ComputeACFlag(destinationRegister, 1);
            PFlag = ComputeParity((byte)result);

            GetRegisterByOpcode(opcodeDestinationRegister) = (byte)(result);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// INR M
        /// </summary>
        /// <param name="instruction"></param>
        public void IncrementMemory(Instruction instruction)
        {
            byte data = MCU.ReadByte(_registers.HL);
            ushort result = (ushort)(data + 1);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            ACFlag = ComputeACFlag(data, 1);
            PFlag = ComputeParity((byte)result);

            MCU.WriteByte(_registers.HL, (byte)(result));

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// DCR R
        /// </summary>
        /// <param name="instruction"></param>
        public void DecrementRegister(Instruction instruction)
        {            
            byte opcodeDestinationRegister = (byte)(instruction.opcode & 0x38);
            byte destinationRegister = GetRegisterByOpcode(opcodeDestinationRegister);
            
            ushort result = (ushort)(destinationRegister - 1);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);

            ACFlag = ComputeACFlagOnSub(destinationRegister, 1);
            PFlag = ComputeParity((byte)result);

            GetRegisterByOpcode(opcodeDestinationRegister) = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// DCR M
        /// </summary>
        /// <param name="instruction"></param>
        public void DecrementMemory(Instruction instruction)
        {         
            byte data = MCU.ReadByte(_registers.HL);
            ushort result = (ushort)(data - 1);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            ACFlag = ComputeACFlagOnSub(data, 1);
            PFlag = ComputeParity((byte)result);

            MCU.WriteByte(_registers.HL, (byte)result);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// INX rp
        /// </summary>
        /// <param name="instruction"></param>
        public void IncrementRegisterPair(Instruction instruction)
        {
            byte opcodeRegisterPair = (byte)(instruction.opcode & 0x30);
            switch (opcodeRegisterPair)
            {
                case 0x00:
                    _registers.BC += 1;
                    break;
                case 0x10:
                    _registers.DE += 1;
                    break;
                case 0x20:
                    _registers.HL += 1;
                    break;
                case 0x30:
                    SP += 1;
                    break;
            }

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// DCX rp
        /// </summary>
        /// <param name="instruction"></param>
        public void DecrementRegisterPair(Instruction instruction)
        {
            byte opcodeRegisterPair = (byte)(instruction.opcode & 0x30);
            switch (opcodeRegisterPair)
            {
                case 0x00:
                    _registers.BC -= 1;
                    break;
                case 0x10:
                    _registers.DE -= 1;
                    break;
                case 0x20:
                    _registers.HL -= 1;
                    break;
                case 0x30:
                    SP -= 1;
                    break;
            }

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// DAD rp
        /// </summary>
        /// <param name="instruction"></param>
        public void AddRegisterPairToHandL(Instruction instruction)
        {            
            int result = 0;
            byte opcodeRegisterPair = (byte)((instruction.opcode & 0x30) >> 4);
            switch (opcodeRegisterPair)
            {
                case 0x00: //00 00 0000
                    result = _registers.HL + _registers.BC;                    
                    break;
                case 0x01: //00 01 0000 
                    result = _registers.HL + _registers.DE;
                    break;
                case 0x02: //00 10 0000
                    result = _registers.HL + _registers.HL;
                    break;
                case 0x03: //00 11 0000
                    result = _registers.HL + SP;
                    break;
            }

            CYFlag = (result > 0xFFFF) ? 1 : 0;
            _registers.HL = (ushort)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// DAA
        /// </summary>
        public void DecimalAdjustAccumulator(Instruction instruction)
        {
            byte lnibble = (byte)(_registers.a & 0x0F);          
            ushort result = _registers.a;

            if (lnibble > 0x9 || ACFlag == 1)
            {                
                ACFlag = ((lnibble + 0x06) & 0xF0) != 0 ? 1 : 0;
                result += 0x06;

                if ((result & 0xFF00) != 0)
                    CYFlag = 1;
            }

            byte hnibble = (byte)((result & 0xF0) >> 4);
            if (hnibble > 0x9 || CYFlag == 1)
            {
                result += 0x60;

                if ((result & 0xFF00) != 0)
                    CYFlag = 1;
            }

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);            
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        //public void DecimalAdjustAccumulator(Instruction instruction)
        //{
        //    int daa = _registers.a;
        //    if ((daa & 0x0F) > 0x9 || ACFlag == 1)
        //    {
        //        ACFlag = (((daa & 0x0F) + 0x06) & 0xF0) != 0 ? 1 : 0;

        //        daa += 0x06;
        //        if ((daa & 0xFF00) != 0)
        //        {
        //            CYFlag = 1;
        //        }
        //    }

        //    if ((daa & 0xF0) > 0x90 || CYFlag == 1)
        //    {
        //        daa += 0x60;

        //        if ((daa & 0xFF00) != 0)
        //        {
        //            CYFlag = 1;
        //        }
        //    }
            
        //    ZFlag = ComputeZFlag((ushort)daa);
        //    SFlag = ComputeSFlag((ushort)daa);
        //    PFlag = ComputeParity((byte)daa);

        //    _registers.a = (byte)daa;

        //    PC += instruction.length;
        //    _tCycles = instruction.tCycles;
        //}
        #endregion

        #region Logical Group
        /// <summary>
        /// ANA R
        /// </summary>
        /// <param name="instruction"></param>
        public void AndRegister(Instruction instruction)
        {
            byte opcodeSourceRegister = (byte)((instruction.opcode & 0x07) << 3);
            byte sourceRegister = GetRegisterByOpcode(opcodeSourceRegister);

            ushort result = (byte)(sourceRegister & _registers.a);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = 0;
            ACFlag = ((_registers.a | sourceRegister) & 0x08) != 0 ? 1 : 0;
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// ANA M
        /// </summary>
        /// <param name="instruction"></param>
        public void AndMemory(Instruction instruction)
        {
            byte data = MCU.ReadByte(_registers.HL);
            ushort result = (ushort)(data & _registers.a);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = 0;
            ACFlag = ((_registers.a | data) & 0x08) != 0 ? 1 : 0;
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// ANI d8
        /// </summary>
        /// <param name="instruction"></param>
        public void AndImmediate(Instruction instruction)
        {
            byte data = NextByte();
            ushort result = (ushort)(data & _registers.a);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = 0;

            // special case on 8080 documented by intel p1-120;
            ACFlag = ((_registers.a | data) & 0x08) != 0 ? 1 : 0; 
            
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// XRA r
        /// </summary>
        /// <param name="instruction"></param>
        public void ExclusiveOr(Instruction instruction)
        {
            byte opcodeSourceRegister = (byte)((instruction.opcode & 0x07) << 3);
            ushort result = (ushort)((GetRegisterByOpcode(opcodeSourceRegister)) ^ _registers.a);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = 0;
            ACFlag = 0;
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// XRA M
        /// </summary>
        /// <param name="instruction"></param>
        public void ExclusiveOrMemory(Instruction instruction)
        {
            ushort result = (ushort)(MCU.ReadByte(_registers.HL) ^ _registers.a);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = 0;
            ACFlag = 0;
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// XRI d8
        /// </summary>
        /// <param name="instruction"></param>
        public void ExclusiveOrImmediate(Instruction instruction)
        {
            byte data = NextByte();
            ushort result = (ushort)(data ^ _registers.a);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = 0;
            ACFlag = 0;
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// ORA r
        /// </summary>
        /// <param name="instruction"></param>
        public void ORRegister(Instruction instruction)
        {
            byte opcodeSourceRegister = (byte)((instruction.opcode & 0x07) << 3);
            ushort result = (ushort)(GetRegisterByOpcode(opcodeSourceRegister) | _registers.a);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = 0;
            ACFlag = 0;
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// ORA M
        /// </summary>
        /// <param name="instruction"></param>
        public void ORMemory(Instruction instruction)
        {
            ushort result = (ushort)(MCU.ReadByte(_registers.HL) | _registers.a);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = 0;
            ACFlag = 0;
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// ORI d8
        /// </summary>
        /// <param name="instruction"></param>
        public void ORImmediate(Instruction instruction)
        {
            byte data = NextByte();
            ushort result = (ushort)(data | _registers.a);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = 0;
            ACFlag = 0;
            PFlag = ComputeParity((byte)result);

            _registers.a = (byte)result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// CMP r
        /// </summary>
        /// <param name="instruction"></param>
        public void CompareRegister(Instruction instruction)
        {
            byte opcodeSourceRegister = (byte)((instruction.opcode & 0x07) << 3);
            byte sourceRegister = GetRegisterByOpcode(opcodeSourceRegister);
            
            ushort result = (ushort)(_registers.a - sourceRegister);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);            
            ACFlag = ComputeACFlagOnSub(_registers.a, sourceRegister);
            PFlag = ComputeParity((byte)result);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// CMP M
        /// </summary>
        /// <param name="instruction"></param>
        public void CompareMemory(Instruction instruction)
        {
            byte data = MCU.ReadByte(_registers.HL);
            ushort result = (ushort)(_registers.a - data);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);            
            ACFlag = ComputeACFlagOnSub(_registers.a, data);
            PFlag = ComputeParity((byte)result);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// CPI data
        /// </summary>
        /// <param name="instruction"></param>
        public void CompareImmediate(Instruction instruction)
        {
            byte data = NextByte();            
            ushort result = (ushort)(_registers.a - data);

            ZFlag = ComputeZFlag(result);
            SFlag = ComputeSFlag(result);
            CYFlag = ComputeCYFlag(result);
            ACFlag = ComputeACFlagOnSub(_registers.a, data);                        
            PFlag = ComputeParity((byte)result);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// RLC 
        /// </summary>
        /// <param name="instruction"></param>
        public void RotateLeft(Instruction instruction)
        {
            byte msb = (byte)(_registers.a >> 7);
            byte result = (byte)((_registers.a << 1) | msb);

            CYFlag = msb;
            _registers.a = result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// RRC
        /// </summary>
        /// <param name="instruction"></param>
        public void RotateRight(Instruction instruction)
        {
            byte lsb = (byte)(_registers.a & 0x01);
            byte result = (byte)((_registers.a >> 1) | (lsb << 7));

            CYFlag = lsb;
            _registers.a = result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// RAL
        /// </summary>
        /// <param name="instruction"></param>
        public void RotateLeftWithCarry(Instruction instruction)
        {
            byte msb = (byte)(_registers.a >> 7);
            byte result = (byte)((_registers.a << 1) | CYFlag);

            CYFlag = msb;
            _registers.a = result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// RAR
        /// </summary>
        /// <param name="instruction"></param>
        public void RotateRightWithCarry(Instruction instruction)
        {
            byte lsb = (byte)(_registers.a & 0x01);
            byte result = (byte)((_registers.a >> 1) | (CYFlag << 7));

            CYFlag = lsb;
            _registers.a = result;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// CMA
        /// </summary>
        /// <param name="instruction"></param>
        public void ComplementAccumulator(Instruction instruction)
        {
            _registers.a = (byte) ~(_registers.a);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// CMC
        /// </summary>
        /// <param name="instruction"></param>
        public void ComplementCarry(Instruction instruction)
        {
            CYFlag = CYFlag == 1 ? 0 : 1;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// STC
        /// </summary>
        /// <param name="instruction"></param>
        public void SetCarry(Instruction instruction)
        {
            CYFlag = 1;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }
        #endregion

        #region BRANCH GROUP
        /// <summary>
        /// JMP address unconditional
        /// </summary>
        /// <param name="instruction"></param>
        public void Jump(Instruction instruction)
        {
            PC = NextWord();
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// JMP conditional
        /// </summary>
        /// <param name="instruction"></param>
        public void JumpConditional(Instruction instruction)
        {
            byte conditionOpcode = (byte)(instruction.opcode & 0x38);
            
            int actualFlagValue = GetFlagConditionByOpcode(conditionOpcode);
            int expectedFlagValue = 0;

            switch (instruction.opcode)
            {
                case 0xC2: //JNZ a16: jump if ZFLAG is 0
                case 0xD2: //JNC a16: jump if CFLAG is 0
                case 0xE2: //JPO a16: jump if PFLAG is 0
                case 0xF2: //JP a16: jump if SFLAG is 0
                    expectedFlagValue = 0;                    
                    break;
                case 0xCA: //JZ a16: jump if ZFLAG is 1:
                case 0xDA: //JC a16: jump if CFLAG is 1
                case 0xEA: //JPE a16: jump if PFLAG is 1
                case 0xFA: //JM a16: jump if SFLAG is 1
                    expectedFlagValue = 1;
                    break;
                default:
                    throw new Exception("Undetected Jump opode");
            }

            if (actualFlagValue == expectedFlagValue)
            {
                PC = NextWord();
                _tCycles = instruction.tCycles;
            }
            else
            {
                PC += instruction.length;
                _tCycles = instruction.aCycles;
            }
        }

        /// <summary>
        /// Call Address Unconditional
        /// </summary>
        /// <param name="instruction"></param>
        public void CallUnconditional(Instruction instruction)
        {
            ushort nextInstructionAddress = (ushort)(PC + instruction.length);
            ushort callingAddress = NextWord();

#if DEBUG
            if (callingAddress == 0x05)
            {
                if (_registers.c == 0x09)
                {
                    ushort startingAddress = _registers.DE;

                    byte character = MCU.ReadByte(startingAddress);
                    while (character != '$')
                    {
                        Console.Write((char)character);
                        startingAddress += 1;

                        character = MCU.ReadByte(startingAddress);
                    }
                }
                else if (_registers.c == 0x02)
                {
                    Console.Write((char)_registers.e);
                }
            }
            else if (callingAddress == 0x00)
            {
                Emulator.Instance.StopEmulation();
            }
#endif

            //Saving into stack pointer the address of the instruction to be 
            //processed when returning from call
            SP -= 1;
            MCU.WriteByte(SP, (byte)(nextInstructionAddress >> 8));
            SP -= 1;
            MCU.WriteByte(SP, (byte)(nextInstructionAddress & 0xFF));

            PC = callingAddress;
            _tCycles = instruction.tCycles;
        
        }

        /// <summary>
        /// Call conditional
        /// </summary>
        /// <param name="instruction"></param>
        public void CallConditional(Instruction instruction)
        {
            byte conditionOpcode = (byte)(instruction.opcode & 0x38);

            int actualFlagValue = GetFlagConditionByOpcode(conditionOpcode);
            int expectedFlagValue = 0;

            switch (instruction.opcode)
            {
                case 0xC4: //CNZ a16: call if ZFLAG is 0
                case 0xD4: //CNC a16: call if CFLAG is 0
                case 0xE4: //CPO a16: call if PFLAG is 0
                case 0xF4: //CP a16:  call if SFLAG is 0
                    expectedFlagValue = 0;
                    break;
                case 0xCC: //CZ a16:  call if ZFLAG is 1:
                case 0xDC: //CC a16:  call if CFLAG is 1
                case 0xEC: //CPE a16: call if PFLAG is 1
                case 0xFC: //CM a16:  call if SFLAG is 1
                    expectedFlagValue = 1;
                    break;
                default:
                    throw new Exception("Undetected Jump opode");
            }

            if (actualFlagValue == expectedFlagValue)
            {
                ushort nextInstructionAddress = (ushort)(PC + instruction.length);
                ushort callingAddress = NextWord();

                SP -= 1;
                MCU.WriteByte(SP, (byte)(nextInstructionAddress >> 8));
                SP -= 1;
                MCU.WriteByte(SP, (byte)(nextInstructionAddress & 0xFF));

                PC = callingAddress;
                _tCycles = instruction.tCycles;
            }
            else
            {
                PC += instruction.length;
                _tCycles = instruction.aCycles;
            }
        }

        /// <summary>
        /// Return unconditional from Call
        /// </summary>
        /// <param name="instruction"></param>
        public void Return(Instruction instruction)
        {
            byte lowOrderValue = MCU.ReadByte(SP);
            SP += 1;

            byte highOrderValue = MCU.ReadByte(SP);
            SP += 1;

            PC = (ushort)(lowOrderValue + (highOrderValue << 8));
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// Return conditional from call
        /// </summary>
        /// <param name="instruction"></param>
        public void ReturnConditional(Instruction instruction)
        {
            byte conditionOpcode = (byte)(instruction.opcode & 0x38);

            int actualFlagValue = GetFlagConditionByOpcode(conditionOpcode);
            int expectedFlagValue = 0;

            switch (instruction.opcode)
            {
                case 0xC0: //RNZ: return if ZFLAG is 0
                case 0xD0: //RNC: return if CFLAG is 0
                case 0xE0: //RPO: return if PFLAG is 0
                case 0xF0: //RP:  return if SFLAG is 0
                    expectedFlagValue = 0;
                    break;
                case 0xC8: //RZ:  return if ZFLAG is 1:
                case 0xD8: //RC:  return if CFLAG is 1
                case 0xE8: //RPE: return if PFLAG is 1
                case 0xF8: //RM:  return if SFLAG is 1
                    expectedFlagValue = 1;
                    break;
                default:
                    throw new Exception("Undetected Jump opode");
            }

            if (actualFlagValue == expectedFlagValue)
            {
                byte lowOrderValue = MCU.ReadByte(SP);
                SP += 1;

                byte highOrderValue = MCU.ReadByte(SP);
                SP += 1;

                PC = (ushort)(lowOrderValue + (highOrderValue << 8));
                _tCycles = instruction.tCycles;
            }
            else
            {
                PC += instruction.length;
                _tCycles = instruction.aCycles;
            }
        }

        /// <summary>
        /// Restart unconditional at n
        /// </summary>
        /// <param name="instruction"></param>
        public void RST(Instruction instruction)
        {
            //Address of the instruction that mut be executed after RST call (on RET instruction os subroutine)
            ushort nextAddress = (ushort)(PC + instruction.length);
            //Address of the subroutine to be called
            ushort callingAddress = (ushort)(((instruction.opcode & 0x38) >> 3) * 8);

            SP -= 1;
            MCU.WriteByte(SP, (byte)(nextAddress >> 8));
            SP -= 1;
            MCU.WriteByte(SP, (byte)(nextAddress & 0xFF));

            PC = callingAddress;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// PCHL
        /// </summary>
        /// <param name="instruction"></param>
        public void PCHL(Instruction instruction)
        {
            PC = _registers.HL;
            _tCycles = instruction.tCycles;
        }
        #endregion

        #region STACK, I/O AND MACHINE CONTROL GROUP
        /// <summary>
        /// PUSH RP
        /// </summary>
        /// <param name="instruction"></param>
        public void PushRP(Instruction instruction)
        {
            ushort selectedRegister = 0;
            byte opcodeRegisterPair = (byte)((instruction.opcode & 0x30) >> 4);
            switch (opcodeRegisterPair)
            {
                case 0x00:
                    selectedRegister = _registers.BC;
                    break;
                case 0x01:
                    selectedRegister = _registers.DE;
                    break;
                case 0x02:
                    selectedRegister = _registers.HL;
                    break;
                case 0x03:
                    throw new Exception("SP not allowed in PUSH instruction");
                default:
                    throw new Exception("Unknown register pair in PUSH");
            }

            SP -= 1;
            MCU.WriteByte(SP, (byte)(selectedRegister >> 8));
            SP -= 1;
            MCU.WriteByte(SP, (byte)(selectedRegister & 0xFF));

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        public void PushPSW(Instruction instruction)
        {
            SP -= 1;
            MCU.WriteByte(SP, _registers.a);

            //Assembling Status byte
            byte statusByte = (byte)(CYFlag | 1 << 1 | PFlag << 2 | 0 << 3 | ACFlag << 4 |
                                     0 << 5 | ZFlag << 6 | SFlag << 7);

            SP -= 1;
            MCU.WriteByte(SP, statusByte);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        public void PopRP(Instruction instruction)
        {
            byte lowOrderByte = MCU.ReadByte(SP);
            SP += 1;
            byte highOrderByte = MCU.ReadByte(SP);
            SP += 1;

            byte opcodeRegisterPair = (byte)((instruction.opcode & 0x30) >> 4);
            switch (opcodeRegisterPair)
            {
                case 0x00:                                       
                    _registers.BC = (ushort)(lowOrderByte | highOrderByte << 8);
                    break;
                case 0x01:
                    _registers.DE = (ushort)(lowOrderByte | highOrderByte << 8);
                    break;
                case 0x02:
                    _registers.HL = (ushort)(lowOrderByte | highOrderByte << 8);
                    break;
                case 0x03:
                    throw new Exception("SP not allowed in PUSH instruction");
                default:
                    throw new Exception("Unknown register pair in PUSH");
            }

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        public void PopPSW(Instruction instruction)
        {
            byte statusByte = MCU.ReadByte(SP);
            SP += 1;

            _registers.a = MCU.ReadByte(SP);
            SP += 1;

            //Restoring flags from status byte
            CYFlag = (statusByte & 0x01) == 0x01 ? 1 : 0;
            PFlag =  (statusByte & 0x04) == 0x04 ? 1 : 0;
            ACFlag = (statusByte & 0x10) == 0x10 ? 1 : 0;
            ZFlag =  (statusByte & 0x40) == 0x40 ? 1 : 0;
            SFlag =  (statusByte & 0x80) == 0x80 ? 1 : 0;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// Exchange stack top with H and L
        /// </summary>
        /// <param name="instruction"></param>
        public void Xthl(Instruction instruction)
        {
            byte previousLValue = _registers.l;
            _registers.l = MCU.ReadByte(SP);
            MCU.WriteByte(SP, previousLValue);

            byte previousHValue = _registers.h;
            _registers.h = MCU.ReadByte((ushort)(SP + 1));
            MCU.WriteByte((ushort)(SP + 1), previousHValue);

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// Move HL to SP
        /// </summary>
        /// <param name="instruction"></param>
        public void SPHL(Instruction instruction)
        {
            SP = _registers.HL;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// Read from bi-directional data bus by the port
        /// specified in input e put data in accumulator
        /// </summary>
        /// <param name="instruction"></param>
        public void In (Instruction instruction)
        {
            byte portNumber = NextByte();

            if (InputDeviceList.ContainsKey(portNumber))
            {
                _registers.a = InputDeviceList[portNumber].Read();
            }

            PC += instruction.length;
            _tCycles = instruction.tCycles;            
        }

        /// <summary>
        /// The content of register A is placed on the eight bit
        /// bi-directional data bus for transmission to the specified port.
        /// </summary>
        /// <param name="instruction"></param>
        public void Out (Instruction instruction)
        {
            byte portNumber = NextByte();

            if (OutputDeviceList.ContainsKey(portNumber))
            {
                OutputDeviceList[portNumber].Write(_registers.a);                
            }

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// Enable interrupts
        /// </summary>
        /// <param name="instruction"></param>
        public void EI (Instruction instruction)
        {
            _interrputEnabled = true;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// Disable interrupts
        /// </summary>
        /// <param name="instruction"></param>
        public void DI (Instruction instruction)
        {
            _interrputEnabled = false;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// Halt cpu 
        /// </summary>
        /// <param name="instruction"></param>
        public void Hlt(Instruction instruction)
        {
            _halt = true;

            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }

        /// <summary>
        /// No operation is executed
        /// </summary>
        /// <param name="instruction"></param>
        public void Nop(Instruction instruction)
        {
            PC += instruction.length;
            _tCycles = instruction.tCycles;
        }
        #endregion

        #region STUFFS
        //Compute Sign Flag
        private int ComputeSFlag(ushort value)
        {
            return (value & 0x80) == 0x80 ? 1 : 0;
        }

        //Compute Zero Flag
        private int ComputeZFlag(ushort value)
        {
            return (value & 0xff) == 0 ? 1 : 0;
        }

        //Compute AuxiliaRy Carry Flag 
        private int ComputeACFlag(byte a, byte b)
        {
            return ((a & 0xf) + (b & 0xf)) > 0xf ? 1 : 0;            
        }

        private int ComputeACFlagWithCarry(byte a, byte b)
        {
            return ((a & 0xf) + (b & 0xf)) >= 0xf ? 1 : 0;
        }

        private int ComputeACFlagOnSub(byte a, byte b)
        {
            return (b & 0xF) <= (a & 0xF)  ? 1 : 0;
        }

        private int ComputeACFlagOnSubCarry(byte a, byte b)
        {
            return (b & 0xF) < (a & 0xF) ? 1 : 0;
        }
       
        //Compute Carry Flag
        public int ComputeCYFlag(ushort value)
        {            
            return (value >> 8) != 0 ? 1 : 0;
        }

        //Compute Parity Flag
        private int ComputeParity(byte number)
        {
            int bitMask = 1;
            int bitset = 0;

            for (int count = 0; count < 8; count++)
            {
                if ((number & (bitMask << count)) != 0)
                    bitset += 1;
            }            

            return (bitset % 2) == 0 ? 1 : 0;
        }
        #endregion

        public void Fetch()
        {
            _opcode = MCU.ReadByte(PC);
        }

        public int Execute()
        {
            int opcodeCycles = 0;

            if (!AreThereInterruptToBeExecuted())
            {
                Instruction currentInstruction = _instructions[_opcode];
                currentInstruction.action.Invoke(currentInstruction);
                opcodeCycles = currentInstruction.tCycles;
            }

            return opcodeCycles;
        }

        public bool AreThereInterruptToBeExecuted()
        {
            bool response = false;

            //Process interrupt if exist and if interrupts are enabled
            if (InterruptQueue.Count > 0)
            {
                byte interruptOpcode = _interruptQueue.Dequeue();
                if (_interrputEnabled)
                {
                    if (_halt == true)
                        _halt = false;

                    _interrputEnabled = false;                    
                    JumpToTheInterruptServiceRoutine(interruptOpcode);

                    response = true;
                }
            }

            return response;
        }

        private void JumpToTheInterruptServiceRoutine(byte opcode)
        {
            ushort callingAddress = (ushort)(((opcode & 0x38) >> 3) * 8);

            SP -= 1;
            MCU.WriteByte(SP, (byte)(PC >> 8));
            SP -= 1;
            MCU.WriteByte(SP, (byte)(PC & 0xFF));

            PC = callingAddress;
            _tCycles = 11;
        }
    }
}
