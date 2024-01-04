
using System;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace System.Drawing.IconLib.ColorProcessing
{
    
    public interface IPaletteQuantizer
    {
        #region Methods
        ColorPalette CreatePalette(Bitmap image, int maxColors, int bitsPerPixel);
        #endregion
    }
}
