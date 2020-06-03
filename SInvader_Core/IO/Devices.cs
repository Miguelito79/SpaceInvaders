using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SInvader_Core.IO
{
    public class Devices
    {
        private enum Keys
        {
            P1Start = 0x31,     //Player1 Start
            P2Start = 0x32,     //Player2 Start
            C = 0x43,           //Insert Coint
            Z = 0x5A,           //Player1 Shot
            ArrowLeft = 0x25,   //Move left 
            ArrowRight = 0x27,  //Move Right
            E = 0x45,           //Player2 Left
            S = 0x53,           //Player2 Right
            F = 0x46            //Player2 Shot
        }

        private ArcadePort1 _arcadePort1; //Input device managing Player1 action
        private ArcadePort2 _arcadePort2; //Input device managing Player2 action
        private ShiftDevice _shiftDevice; //Shift register device for fast shifting of data
        private ShiftOffsetDevice _shiftOffsetDevice; //Shift offset device defining the offset of shift register
        private SoundDevice _soundDevice3; //Manage output sounds on output port 3
        private SoundDevice _soundDevice5; //Manage output sounds on output port 5

        private Dictionary<byte, IInputDevice> _inputDeviceList;
        public Dictionary<byte, IInputDevice> InputDeviceList
        {
            get { return _inputDeviceList; } 
            private set { }
        }


        private Dictionary<byte, IOutputDevice> _outputDevice;
        public Dictionary<byte, IOutputDevice> OutputDeviceList
        {
            get { return _outputDevice; }
            private set { }
        }

        public Devices()
        {
            _inputDeviceList = new Dictionary<byte, IInputDevice>();
            _outputDevice = new Dictionary<byte, IOutputDevice>();

            _shiftOffsetDevice = new ShiftOffsetDevice();
            _shiftDevice = new ShiftDevice(_shiftOffsetDevice);
            _arcadePort1 = new ArcadePort1((int)Keys.C, (int)Keys.P1Start, (int)Keys.P2Start, (int)Keys.Z, (int)Keys.ArrowLeft, (int)Keys.ArrowRight);
            _arcadePort2 = new ArcadePort2((int)Keys.S, (int)Keys.F, (int)Keys.E);
            _soundDevice3 = new SoundDevice(3);
            _soundDevice5 = new SoundDevice(5);

            InputDeviceList.Add(1, _arcadePort1);
            InputDeviceList.Add(2, _arcadePort2);
            InputDeviceList.Add(3, _shiftDevice);
            OutputDeviceList.Add(2, _shiftOffsetDevice);
            OutputDeviceList.Add(3, _soundDevice3);
            OutputDeviceList.Add(4, _shiftDevice);            
            OutputDeviceList.Add(5, _soundDevice5);
        }

        public void KeyDown(int key)
        {
            switch(key)
            {
                case (int)Keys.C:
                case (int)Keys.P1Start:
                case (int)Keys.P2Start:
                case (int)Keys.Z:
                case (int)Keys.ArrowLeft:
                case (int)Keys.ArrowRight:
                    _arcadePort1.KeyDown(key);
                    break;
                case (int)Keys.E:
                case (int)Keys.F:
                case (int)Keys.S:
                    _arcadePort1.KeyDown(key);
                    break;
            }
        }

        public void KeyUp(int key)
        {
            switch (key)
            {
                case (int)Keys.C:
                case (int)Keys.P1Start:
                case (int)Keys.P2Start:
                case (int)Keys.Z:
                case (int)Keys.ArrowLeft:
                case (int)Keys.ArrowRight:
                    _arcadePort1.KeyUp(key);
                    break;
                case (int)Keys.E:
                case (int)Keys.F:
                case (int)Keys.S:
                    _arcadePort1.KeyUp(key);
                    break;
            }
        }
    }
}
