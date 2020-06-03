using System;
using System.Collections.Generic;
using System.Text;

namespace SInvader_Core.IO
{
    public class ShiftOffsetDevice : IInputDevice, IOutputDevice
    {
        private byte _shiftOffset = 0;

        public byte Read()
        {
            return _shiftOffset;
        }

        public void Write(byte data)
        {
            _shiftOffset = (byte)(data & 0x07); //Offset may vary from0 to 8 thus the and with 0x07
        }
    }
}
