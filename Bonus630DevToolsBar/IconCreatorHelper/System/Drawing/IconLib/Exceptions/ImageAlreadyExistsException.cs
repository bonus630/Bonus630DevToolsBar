
namespace System.Drawing.IconLib.Exceptions
{
  
    [Serializable]
    public class ImageAlreadyExistsException : Exception
    {
        #region Constructors
        public ImageAlreadyExistsException() : base ("Image with same size and format already exists")
        {
        }
        #endregion
    }
}
