//  Copyright (c) 2006, Gustavo Franco
//  Email:  gustavo_franco@hotmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
using System.Drawing.Imaging;
namespace System.Drawing.IconLib.Exceptions
{
    
    [Serializable]
    public class InvalidPixelFormatException : Exception
    {
        #region Constructors
        public InvalidPixelFormatException(PixelFormat invalid, PixelFormat expected) : base (invalid != PixelFormat.Undefined ?
                                                                                            "PixelFormat " + invalid.ToString() + " is invalid" :
                                                                                            expected != PixelFormat.Undefined ?
                                                                                            "PixelFormat " + expected.ToString() + " expected" :
                                                                                            "Invalid PixelFormat")
        {
        }
        #endregion
    }
}
