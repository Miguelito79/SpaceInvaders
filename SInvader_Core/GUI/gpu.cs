using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SInvader_Core.GUI
{
    public class GPU
    {       
        private Image _display;
        public const int IMAGE_WIDTH = 256;
        public const int IMAGE_HEIGHT = 224;

        //Interrupt opcodes executed when VBLANK interrupt is fired
        //The first draw the bottom half of the screen, the second 
        //the top half. 
        public const byte START_VBLANK_OPCODE = 0xcf; 
        public const byte END_VBLANK_OPCODE = 0xd7;

        public const int FPS = 60; //Number of frames per second
        public const int CPU_CLOCK_FREQUENCY = 2000000; //Mhz

        //1 Frame takes 33.333,333 Milliseconds
        public const double CYCLE_PER_FRAMES = CPU_CLOCK_FREQUENCY / FPS;
        //Number of cycles used to fire START_VBLANK and END_VBLANK inside a frame
        public int HALF_CYCLES_PER_FRAMES = (int)(CYCLE_PER_FRAMES / 2);

        public Image Display
        {
            get { return (Image)_display.Clone(); }
            private set { }
        }

        public void Initialize()
        {
            using (Bitmap bmp = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.FillRectangle(Brushes.Black, 0, 0, IMAGE_WIDTH, IMAGE_HEIGHT);
                }

                _display = (Image)bmp.Clone();
            }
        }

        private byte[] GetVideoRam()
        {
            byte[] vram = Emulator.Instance.MCU.buffer[2];

            byte[] response = new byte[7168];
            Array.Copy(vram, 0x0, response, 0, 7168);
            return response;            
        }

        public void DrawImageOnScreen()
        {
            byte[] vram = GetVideoRam();
            GCHandle gcHandle = GCHandle.Alloc(vram, GCHandleType.Pinned);

            Array.Reverse(vram);
            gcHandle.Free();

            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(vram, 0);                        
            using (Bitmap bitmap = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT, 32, PixelFormat.Format1bppIndexed, ptr))
            {                
                bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                _display = (Image)bitmap.Clone();
            }            
        }
    }
}
