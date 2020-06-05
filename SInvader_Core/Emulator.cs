using SInvader_Core.GUI;
using SInvader_Core.i8080;
using SInvader_Core.IO;
using SInvader_Core.MMU;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SInvader_Core
{
    public class Emulator
    {
        private Stopwatch _stopWatch;
        private Timer _timer;

        public delegate void DrawImage(Image image);
        public event DrawImage OnDrawImage;

        public delegate void EmulatorStateChange(TCurrentState state);
        public event EmulatorStateChange OnEmulatorStateChange;

        
        public double _lastInterruptTime = 0.0f;

        public enum TCurrentState
        {
            RUNNING,
            PAUSED,
            STOPPED
        }

        private CPU _cpu;
        private GPU _gpu;
        private MCU _mcu;

        //Number of total cycles executed by CPU
        //private int _tCycles;
        //Number of VBlank Cycles executed during an intevall of time
        private int _vBlankCycles;
        //
        private long _lastValue;

        private Devices _devices;
        private bool _stopEmulation;        

        //Next VBLANK interrupt to be fired
        private byte _nextVblankInterrupt;       

        private TCurrentState _currentState;
        

        public TCurrentState CurrentState
        {
            get { return _currentState; }
            private set { }
        }

        public CPU CPU        {
            get { return _cpu; }
            private set { }
        }

        public MCU MCU
        {
            get { return _mcu; }
            set { _mcu = value; }
        }

        public GPU GPU
        {
            get { return _gpu; }
            private set { }
        }

        public Devices Devices
        {
            get { return _devices; }
            private set { }
        }

        private static readonly Emulator _instance;
        public static Emulator Instance
        {
            get { return _instance; }
            private set { }
        }

        private Emulator()
        {
            _stopEmulation = false;
            _nextVblankInterrupt = GPU.END_VBLANK_OPCODE;

            _mcu = new MCUR();          
          
            _cpu = new CPU();
            _cpu.Initialize();

            _gpu = new GPU();
            _gpu.Initialize();

            _devices = new Devices();
            _stopWatch = new Stopwatch();

            _currentState = TCurrentState.STOPPED;            
        }

        static Emulator()
        {
            _instance = new Emulator();
        }

        public void StopEmulation()
        {
            if (_currentState != TCurrentState.STOPPED)
            {
                _stopEmulation = true;
                _stopWatch.Stop();

                _mcu.Clear();
                _cpu.Initialize();                
                _gpu.Initialize();
            }
        }


        public void Run(String fullPath)
        {
            if (_currentState == TCurrentState.STOPPED)
            {
                if (MCU.LoadFile(fullPath))
                {
                    _stopWatch.Start();
                    _currentState = TCurrentState.RUNNING;
                    _timer = new Timer(TimerAsync_CallBack, null, 0, Timeout.Infinite);
                }
            }
        }

        public void Run(String fullPath, int offset)
        {
            if (MCU.LoadFileInMemoryAt(fullPath, offset))
            {
                _stopWatch.Start();
                _timer = new Timer(TimerAsync_CallBack, null, 0, Timeout.Infinite);
            }
        }

        public void Run(string[] multipleRomFiles)
        {
            if (_currentState == TCurrentState.STOPPED)
            {
                if (MCU.LoadMultipleFiles(multipleRomFiles))
                {
                    _stopWatch.Start();
                    _currentState = TCurrentState.RUNNING;
                    
                    _timer = new Timer(TimerAsync_CallBack, null, 0, Timeout.Infinite);
                }
            }
        }
      
        private void TimerAsync_CallBack(object state)
        {
            if (!_stopEmulation)
            {
                long currentTime = _stopWatch.ElapsedMilliseconds;
                long elapsedTimeFromLastPeriod = currentTime - _lastValue;

                long numberOfCycleToRun = 2000 * elapsedTimeFromLastPeriod;
                while (numberOfCycleToRun > 0)
                {
                    numberOfCycleToRun -= PerformSingleStep();
                }

                _lastValue = currentTime;
                _timer.Change(0, Timeout.Infinite);
            }
            else
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                OnEmulatorStateChange(_currentState = TCurrentState.STOPPED);
            }
        }     
        
        /// <summary>
        /// Function added to display result of cpu exercizer test
        /// </summary>
        public void PerformMultipleStep(string fullPath, int memoryOffset)
        {
            if (MCU.LoadFileInMemoryAt(fullPath, memoryOffset))
            {
                while (!_stopEmulation)
                {
                    if (!_cpu.Halted)
                    {
                        _cpu.Fetch();
                        _cpu.Execute();
                    }

                    // CP/M warm boot (test finished and restarted itself)
                    if (_cpu.PC == 0x00)
                        _stopEmulation = true;

                    // Call to CP/M bios emulated function to write characters on screen
                    if (_cpu.PC == 0x05)
                        _cpu.PerformBdosCall();
                }
            }
        }

        private int PerformSingleStep()
        {
            //Number of cycles of this step 
            int cycles = 0;

            if (!_cpu.Halted)
            {
                _cpu.Fetch();
                cycles = _cpu.Execute();                
                _vBlankCycles += cycles;
            }

            if (_vBlankCycles >= GPU.HALF_CYCLES_PER_FRAMES)
            {
                //Adding VBLANK interrupt and draw image
                AddVblankInterrupts();
            }

            return cycles;
        }

        private void AddVblankInterrupts()
        {
            switch (_nextVblankInterrupt)
            {
                case GPU.END_VBLANK_OPCODE:
                    _cpu.InterruptQueue.Enqueue(GPU.END_VBLANK_OPCODE);
                    _nextVblankInterrupt = GPU.START_VBLANK_OPCODE;
                    break;
                case GPU.START_VBLANK_OPCODE:
                    _cpu.InterruptQueue.Enqueue(GPU.START_VBLANK_OPCODE);
                    _nextVblankInterrupt = GPU.END_VBLANK_OPCODE;
                    break;
            }

            //Reset VBLANK counter
            _vBlankCycles = 0;

            //Time to draw on screen and firing the event to the main form

            if (OnDrawImage != null)
            {
                _gpu.DrawImageOnScreen();
                OnDrawImage(_gpu.Display);
            }
        }

        public void KeyDown(int key)
        {
            _devices.KeyDown(key);
        }

        public void KeyUp(int key)
        {
            _devices.KeyUp(key);
        }        
    }
}
