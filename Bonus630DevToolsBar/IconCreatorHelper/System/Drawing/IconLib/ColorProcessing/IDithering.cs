
using System;
using System.Text;
using System.Drawing;
using System.Drawing.IconLib;
using System.Collections.Generic;

namespace System.Drawing.IconLib.ColorProcessing
{
   
    public interface IDithering
    {
        #region Methods
        unsafe void Disperse(byte* pixelSource, int x, int y, byte bpp, int stride, int width, int height, Color colorEntry);
        #endregion
    }
}
