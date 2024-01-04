
namespace System.Drawing.IconLib.Exceptions
{
    
    [Serializable]
    public class InvalidMultiIconMaskBitmap : Exception
    {
        #region Constructors
        public InvalidMultiIconMaskBitmap() : base ("Invalid mask bitmap. Mask must be same size as the bitmap and PixelFormat must be Format1bppIndexed")
        {
        }
        #endregion
    }
}
