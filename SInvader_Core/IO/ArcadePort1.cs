using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SInvader_Core.IO
{
    /// <summary>
    /// Emulating arcade port interconnected to the intel 8080 via 16bit address bus
    /// </summary>
    public class ArcadePort1 : IInputDevice
    {       
        private enum ArcadeButtons
        {
            InsertCoin = 0x00,
            P2Start = 0x02,
            P1Start = 0x04,
            P1Shot = 0x10,
            P1Left = 0x20,
            P1Right = 0x40
        }

        //list of admitted key
        private Dictionary<int, int> _keys;

        private byte _register = 0x8;
        /* bit 0 = CREDIT(1 if deposit)      bit 1 = 2P start(1 if pressed)
           bit 2 = 1P start(1 if pressed)    bit 3 = Always 1
           bit 4 = 1P shot(1 if pressed)     bit 5 = 1P left(1 if pressed)
           bit 6 = 1P right(1 if pressed)    bit 7 = Not connected
         */

        /// <summary>
        /// Emulate instert coin - Bit 0
        /// </summary>
        public byte Credit
        {
            get { return (byte)(_register & 0x01); }
            set { _register = (byte)((_register & ~(1)) | value); }
        }

        /// <summary>
        /// Emulate Player2 Start - Bit 1
        /// </summary>
        public byte P2Start
        {
            get { return (byte)((_register & 0x02) >> 1); }
            private set { _register = (byte) ((_register & ~(1 << 1)) | (value << 1)); }
        }

        /// <summary>
        /// Emulate Player1 Start - Bit 2
        /// </summary>
        public byte P1Start
        {
            get { return (byte)((_register & 0x04) >> 2); }
            private set { _register = (byte)((_register & ~(1 << 2)) | (value << 2)); }
        }

        /// <summary>
        /// Emulate Player1 Shot - Bit 4
        /// </summary>
        public byte P1Shot
        {
            get { return (byte)((_register & 0x10) >> 4); }
            private set { _register = (byte)((_register & ~(1 << 4)) | (value << 4)); }
        }

        /// <summary>
        /// Emulate Player1 Left - Bit 5
        /// </summary>
        public byte P1Left
        {
            get { return (byte)((_register & 0x20) >> 5); }
            private set { _register = (byte)((_register & ~(1 << 5)) | (value << 5)); }
        }

        /// <summary>
        /// Emulate Player1 Right - Bit 6
        /// </summary>
        public byte P1Right
        {
            get { return (byte)((_register & 0x40) >> 6); }
            private set { _register = (byte)((_register & ~(1 << 6)) | (value << 6)); }
        }

        public ArcadePort1(int insertCoinKey, int player1StartKey, int player2StartKey,
                          int player1ShotKey, int player1LeftKey, int player1RightKey)
        {
            //Mapping user selected keys with 
            _keys = new Dictionary<int, int>();
            _keys.Add(insertCoinKey,   (int)ArcadeButtons.InsertCoin);
            _keys.Add(player2StartKey, (int)ArcadeButtons.P2Start);
            _keys.Add(player1StartKey, (int)ArcadeButtons.P1Start);            
            _keys.Add(player1ShotKey,  (int)ArcadeButtons.P1Shot);
            _keys.Add(player1LeftKey,  (int)ArcadeButtons.P1Left);
            _keys.Add(player1RightKey, (int)ArcadeButtons.P1Right);
        }

        private void SetKeyValue(int key, byte value)
        {
            if (_keys.ContainsKey(key))
            {
                if (_keys[key] == (int)ArcadeButtons.InsertCoin)    //Pusched/Released (1 or 0) insert coin key
                    Credit = value;
                else if (_keys[key] == (int)ArcadeButtons.P2Start)  //Pusched/Released (1 or 0) player2 start game key
                    P2Start = value;
                else if (_keys[key] == (int)ArcadeButtons.P1Start)  //Pusched/Released (1 or 0) player1 start game key
                    P1Start = value;
                else if (_keys[key] == (int)ArcadeButtons.P1Shot)   //Pusched/Released (1 or 0) player1 shot key
                    P1Shot = value;
                else if (_keys[key] == (int)ArcadeButtons.P1Left)   //Pusched/Released (1 or 0) Player1 left key 
                    P1Left = value;
                else if (_keys[key] == (int)ArcadeButtons.P1Right)  //Pusched/Released (1 or 0) Player1 right key
                    P1Right = value;
            }
        }

        /// <summary>
        /// On key pressed, put value 1 on the bit register associated with that key 
        /// </summary>
        /// <param name="key"></param>
        public void KeyDown(int key)
        {            
            SetKeyValue(key, 1);
        }

        /// <summary>
        /// On key released, put value 0 on the bit register associated with previously pushed key
        /// </summary>
        /// <param name="key"></param>
        public void KeyUp(int key)
        {
            SetKeyValue(key, 0);
        }

        /// <summary>
        /// Return the value of register arcade port
        /// </summary>
        /// <returns></returns>
        public byte Read()
        {
            return _register;
        }
    }
}
