//  Copyright (c) 2006, Gustavo Franco
//  Email:  gustavo_franco@hotmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 

using System;
using System.IO;
using System.Text;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Drawing.IconLib.BitmapEncoders
{
   
    internal class PNGEncoder : ImageEncoder
    {
        #region Variables Declaration
        #endregion

        #region Constructors
        public PNGEncoder()
        {
        }
        #endregion

        #region Properties
        public override IconImageFormat IconImageFormat
        {
            get {return IconImageFormat.PNG;}
        }

        public override unsafe int ImageSize
        {
            get
            {
                // This is a fast and temporary solution,
                // Soon Ill implement a png cache, 
                // then the image will be generated just once between calls and writes
                MemoryStream ms = new MemoryStream();
                Icon.ToBitmap().Save(ms, ImageFormat.Png);
                return (int) ms.Length;
            }
        }
        #endregion

        #region Methods
        public unsafe override void Read(Stream stream, int resourceSize)
        {
            // Buffer a PNG image
            byte[] buffer = new byte[resourceSize];
            stream.Read(buffer, 0, buffer.Length);
            MemoryStream ms = new MemoryStream(buffer);
            Bitmap pngBitmap = new Bitmap(ms);

            // Set XOR and AND Image
            IconImage iconImage = new IconImage();
            iconImage.Set(pngBitmap, null, Color.Transparent);
            pngBitmap.Dispose();

            //Transfer the data from the BMPEncoder to the PNGEncoder
            CopyFrom(iconImage.Encoder);
         }

        public override void Write(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            Icon.ToBitmap().Save(ms, ImageFormat.Png);
            byte[] buffer = ms.GetBuffer();
            stream.Write(buffer, 0, (int) ms.Length);
        }
        #endregion
    }
}
