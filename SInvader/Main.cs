using SInvader_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SInvader
{
    public partial class Main : Form
    {        
        Emulator _emulator = Emulator.Instance;        

        public Main()
        {
            InitializeComponent();

            _emulator.OnDrawImage += _emulator_OnDrawImage;
            _emulator.OnEmulatorStateChange += _emulator_OnEmulatorStateChange;
        }

        private void _emulator_OnEmulatorStateChange(Emulator.TCurrentState state)
        {
            if (state == Emulator.TCurrentState.STOPPED)
            {
                if (this.InvokeRequired)
                    this.Invoke((MethodInvoker)delegate { this.Close(); });
                else
                    this.Close();
            }                
        }

        private void _emulator_OnDrawImage(Image image)
        {
            if (this.InvokeRequired)
                this.Invoke((MethodInvoker)delegate
                   {
                       pictureBox.Image = _emulator.GPU.Display;
                       pictureBox.Refresh();
                   });
            else
            {
                pictureBox.Image = _emulator.GPU.Display;
                pictureBox.Refresh();
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else
                _emulator.KeyDown(e.KeyValue);
        }

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            _emulator.KeyUp(e.KeyValue);
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            pictureBox.Image = _emulator.GPU.Display;
        }
        

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_emulator.CurrentState == Emulator.TCurrentState.RUNNING)
            {
                _emulator.StopEmulation();
                e.Cancel = true;
            }                
        }

        private void runSpaceInvaders_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Zip Files (*.zip)|*.zip|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = @"C:\Temp\games\spaceinvaders";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _emulator.Run(openFileDialog.FileName);
            }            
        }        
    }
}
