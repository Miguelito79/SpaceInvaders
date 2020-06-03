using System;
using System.Collections.Generic;
using System.Text;

namespace SInvader_Core.IO
{
    public class ArcadePort2 : IInputDevice
    {
        public enum ArcadeButtons
        {
            DIP3 = 0x01,     //DIP3 and DIP5 in conjunction set the initial number of ships  
            DIP5 = 0X02,     //00 = 3 ships, 01 = 4 ships, 10 = 5 ships, 11 = 6 ships
            TILT = 0X04,     //Derive from flipper but cannot shake you PC LOL :-) Left unemulated
            DIP6 = 0x08,     //0 = give extra ship at 1500, 1 = give extra ship at 1000
            P2shot = 0x10,
            P2Left = 0x20,
            P2Right = 0x40,
            CoinInfo = 0x80  //Coin info displayed in demo screen 0=ON
        }

        //list of admitted key
        private Dictionary<int, int> _keys;

        private byte _register = 0x0;
        /* bit 0 = DIP3                      bit 1 = DIP5
           bit 2 = TILT (1 if pressed)       bit 3 = DIP6
           bit 4 = 2P shot(1 if pressed)     bit 5 = 2P left(1 if pressed)
           bit 6 = 2P right(1 if pressed)    bit 7 = COIN INFO
         */

        /// <summary>
        /// Bit 0 - In conjunction con DIP5 set the number of ships available to the gamer 
        /// </summary>
        public byte DIP3
        {
            get { return (byte)(_register & 0x01); }
            set { _register = (byte)((_register & ~(1)) | value); }
        }

        /// <summary>
        /// Bit 1 - In conjunction con DIP5 set the number of ships available to the gamer
        /// </summary>
        public byte DIP5
        {
            get { return (byte)((_register & 0x02) >> 1); }
            private set { _register = (byte)((_register & ~(1 << 1)) | (value << 1)); }
        }

        /// <summary>
        /// Emulate TILT if machine is shaked - Bit 2
        /// </summary>
        public byte TILT
        {
            get { return (byte)((_register & 0x04) >> 2); }
            private set { _register = (byte)((_register & ~(1 << 2)) | (value << 2)); }
        }

        /// <summary>        
        /// Bit 3 - if 0 = give an extra ship at 1500 - If 1 give an extra ship at 1000
        /// </summary>
        public byte DIP6
        {
            get { return (byte)((_register & 0x8) >> 3); }
            private set { _register = (byte)((_register & ~(1 << 3)) | (value << 3)); }
        }

        /// <summary>
        /// Emulate Player2 Shot - Bit 4
        /// </summary>
        public byte P2Shot
        {
            get { return (byte)((_register & 0x10) >> 4); }
            private set { _register = (byte)((_register & ~(1 << 4)) | (value << 4)); }
        }

        /// <summary>
        /// Emulate Player2 Left - Bit 5
        /// </summary>
        public byte P2Left
        {
            get { return (byte)((_register & 0x20) >> 5); }
            private set { _register = (byte)((_register & ~(1 << 5)) | (value << 5)); }
        }

        /// <summary>
        /// Emulate Player2 Right - Bit 6
        /// </summary>
        public byte P2Right
        {
            get { return (byte)((_register & 0x40) >> 6); }
            private set { _register = (byte)((_register & ~(1 << 6)) | (value << 6)); }
        }

        /// <summary>
        /// Coin info displayed in demo screen 0=ON - Bit 7
        /// </summary>
        public byte CoinInfo
        {
            get { return (byte)((_register & 0x80) >> 7); }
            private set { _register = (byte)((_register & ~(1 << 7)) | (value << 7)); }
        }

        public ArcadePort2(int player2Left, int player2Right, int player2Shot)
        {
            _keys = new Dictionary<int, int>();
            _keys.Add(player2Left, (int)ArcadeButtons.P2Left);
            _keys.Add(player2Right, (int)ArcadeButtons.P2Right);
            _keys.Add(player2Shot, (int)ArcadeButtons.P2shot);
        }

        private void SetKeyValue(int key, byte value)
        {
            if (_keys[key] == (int)ArcadeButtons.P2Left)        //Pusched/Released (1 or 0) player1 shot key
                P2Left = value;
            else if (_keys[key] == (int)ArcadeButtons.P2Right)  //Pusched/Released (1 or 0) Player1 left key
                P2Right = value;
            else if (_keys[key] == (int)ArcadeButtons.P2shot)   //Pusched/Released (1 or 0) Player1 right ke
                P2Shot = value;
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
