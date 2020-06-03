using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SInvader_Core.IO
{
    public interface IOutputDevice
    {
        void Write(byte data);
    }
}
