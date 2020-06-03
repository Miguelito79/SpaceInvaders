using System;
using System.Collections.Generic;
using System.Text;

namespace SInvader_Core.IO
{
    public class ShiftDevice : IInputDevice, IOutputDevice 
    {
        //Shift Register
        ushort _shiftRegister;

        //Offset necessary where reading
        ShiftOffsetDevice _shiftOffsetDevice;

        public ShiftDevice(ShiftOffsetDevice device)
        {
            _shiftRegister = 0;
            _shiftOffsetDevice = device;
        }

        /// <summary>
        /// Return shift register offsetted by 'shiftoffsetdevice' (Port 2) 
        /// </summary>
        /// <returns></returns>
        public byte Read()
        {
            return (byte)((_shiftRegister >> (8 - _shiftOffsetDevice.Read())) & 0xFF);
        }

        /// <summary>
        /// Write on shift register pushing the previous most significant byte in the
        /// lower part and input data in the higher.
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte data)
        {
            byte lsb = (byte)((_shiftRegister >> 8) & 0xFF);
            _shiftRegister = (ushort)((data << 8) | lsb);
        }
    }
}
