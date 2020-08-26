using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SInvader_Core.IO
{
    public class SoundDevice : IOutputDevice, IDisposable
    {
        private class PlaySound : IDisposable 
        {            
            private bool _loopPlaying;
            private string _path;
            private WaveOutEvent _outputDevice;
            private AudioFileReader _audioFileReader;

            public void Dispose()
            {
                _outputDevice.Dispose();
                _audioFileReader.Dispose();

                _outputDevice = null;
                _audioFileReader = null;
                _loopPlaying = false;
                _path = string.Empty;
            }

            private bool _enabled;
            public bool Enabled
            {
                get { return _enabled; }
                set { _enabled = value; }
            }

            public bool LoopPlaying
            {
                get { return _loopPlaying; }
                private set { }
            }

            public PlaySound(string path)
            {
                _path = path;
                _loopPlaying = false;                

                if (!string.IsNullOrEmpty(_path))
                {
                    _audioFileReader = new AudioFileReader(path);
                    _outputDevice = new WaveOutEvent();
                    _outputDevice.Init(_audioFileReader);
                    _enabled = true;
                }
            }

            public void Play()
            {
                if (_enabled)
                {
                    if (!string.IsNullOrEmpty(_path))
                    {
                        _outputDevice.Play();
                        _audioFileReader.Position = 0;
                    }
                }
            }            

            public void LoopPlay()
            {
                if (_enabled)
                {
                    if (_loopPlaying == false)
                    {
                        _loopPlaying = true;

                        Thread thread = new Thread(PlayInLoopAsync_CallBack);
                        thread.Start();
                    }
                }
            }            

            private void PlayInLoopAsync_CallBack()
            {                
                while (_loopPlaying)
                {                                       
                    switch (Emulator.Instance.CurrentState)
                    {
                        case Emulator.TCurrentState.PAUSED:
                            _outputDevice.Stop();
                            break;
                        case Emulator.TCurrentState.STOPPED:
                            _loopPlaying = false;
                            break;
                        default:
                            _outputDevice.Play();
                            _audioFileReader.Position = 0;
                            break;
                    }

                    if (Emulator.Instance.CurrentState == Emulator.TCurrentState.PAUSED)
                        _outputDevice.Pause();
                }                
            }

            public void StopLoopPlay()
            {
                _loopPlaying = false;
            }
        }

        private int _port;
        private Dictionary<int, PlaySound> _soundDictionary;

        private byte _previousRegister;        
        /*  Initialized on port 3                |   Initialized on port 5
         *  bit 0=UFO(repeats)      SX0 0.raw    |   bit 0=Fleet movement 1     SX6 4.raw
         *  bit 1=Shot              SX1 1.raw    |   bit 1=Fleet movement 2     SX7 5.raw
         *  bit 2=Flash(player die) SX2 2.raw    |   bit 2=Fleet movement 3     SX8 6.raw
         *  bit 3=Invader die       SX3 3.raw    |   bit 3=Fleet movement 4     SX9 7.raw
         *  bit 4=Extended play     SX4          |   bit 4=UFO Hit              SX10 8.raw
         *  bit 5= AMP enable       SX5          |   bit 5= NC (Cocktail mode control ... to flip screen)
         *  bit 6= NC(not wired)                 |   bit 6= NC (not wired)
         *  bit 7= NC(not wired)                 |   bit 7= NC (not wired)
         */

        public SoundDevice(int port)
        {
            _port = port;
            _soundDictionary  = new Dictionary<int, PlaySound>();

            switch (port)
            {
                case 3:
                    AllocateSoundForPort3();
                    break;
                case 5:
                    AllocateSoundForPort5();
                    break;
            }
        }        

        public bool Enabled
        {           
            set
            {
                foreach (PlaySound playSound in this._soundDictionary.Values)
                    playSound.Enabled = value;
            }
        }

        private void AllocateSoundForPort3()
        {
            string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _soundDictionary[0] = new PlaySound(Path.Combine(appDirectory, @"SFX\ufo.wav"));
            _soundDictionary[1] = new PlaySound(Path.Combine(appDirectory, @"SFX\shoot.wav"));
            _soundDictionary[2] = new PlaySound(Path.Combine(appDirectory, @"SFX\explosion.wav"));
            _soundDictionary[3] = new PlaySound(Path.Combine(appDirectory, @"SFX\invaderkilled.wav"));
            _soundDictionary[4] = new PlaySound(string.Empty);
            _soundDictionary[5] = new PlaySound(string.Empty);
            _soundDictionary[6] = new PlaySound(string.Empty);
            _soundDictionary[7] = new PlaySound(string.Empty);
        }

        private void AllocateSoundForPort5()
        {
            string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _soundDictionary[0] = new PlaySound(Path.Combine(appDirectory, @"SFX\fastinvader1.wav"));
            _soundDictionary[1] = new PlaySound(Path.Combine(appDirectory, @"SFX\fastinvader2.wav"));
            _soundDictionary[2] = new PlaySound(Path.Combine(appDirectory, @"SFX\fastinvader3.wav"));
            _soundDictionary[3] = new PlaySound(Path.Combine(appDirectory, @"SFX\fastinvader4.wav"));
            _soundDictionary[4] = new PlaySound(Path.Combine(appDirectory, @"SFX\UfoHit.wav"));
            _soundDictionary[5] = new PlaySound(string.Empty);
            _soundDictionary[6] = new PlaySound(string.Empty);
            _soundDictionary[7] = new PlaySound(string.Empty);            
        }

        public void Write(byte data)
        {
            //Avoid to play sound if there is not transition of any bit in current register values
            if (_previousRegister != data)
            {               
                byte bitmask = 0x01;
                byte bitstart = 0x00;

                if(ManageUfoSound(data))
                {
                    bitmask = 0x02;
                    bitstart = 1;
                }                

                //Testing bits of register and emit sound if there is
                //a transition from low to high respect of it's previous value 
                for (int bitnumber = bitstart; bitnumber < 7; bitnumber++)
                {
                    if (((data & bitmask) == bitmask) && (_previousRegister & bitmask) != bitmask)
                    {                                                
                        _soundDictionary[bitnumber].Play();
                    }
                    
                    bitmask <<= 1;
                }

                _previousRegister = data;
            }
        }

        private bool ManageUfoSound(byte data)
        {
            bool response = true;

            if (_port == 3)
            {
                //Manage transition from low to high
                if (((data & 0x01) == 1) && ((_previousRegister & 0x01) == 0))
                {
                    if (!_soundDictionary[0].LoopPlaying)
                        _soundDictionary[0].LoopPlay();
                }
                //Manage transition from high to low
                else if (((data & 0x01) == 0) && ((_previousRegister & 0x01) == 1))
                    _soundDictionary[0].StopLoopPlay();
            }
            else
                response = false;

            return response;
        }

        public void Dispose()
        {
            foreach(KeyValuePair<int, PlaySound> soundPlayer in _soundDictionary)
            {
                soundPlayer.Value.StopLoopPlay();
                soundPlayer.Value.Dispose();
            }

            _soundDictionary.Clear();
        }
    }
}
