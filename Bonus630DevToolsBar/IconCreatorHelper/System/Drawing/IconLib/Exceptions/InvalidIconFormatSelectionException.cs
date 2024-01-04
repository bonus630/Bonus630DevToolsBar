
namespace System.Drawing.IconLib.Exceptions
{
    
    [Serializable]
    public class InvalidIconFormatSelectionException : Exception
    {
        #region Constructors
        public InvalidIconFormatSelectionException() : base ("Invalid IconImageFormat selection")
        {
        }
        #endregion
    }
}
