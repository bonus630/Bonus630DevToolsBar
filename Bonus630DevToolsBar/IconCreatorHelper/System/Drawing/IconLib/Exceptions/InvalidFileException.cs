
namespace System.Drawing.IconLib.Exceptions
{
  
    [Serializable]
    public class InvalidFileException : Exception
    {
        #region Constructors
        public InvalidFileException() : base ("Format not recognized by IconLib")
        {
        }
        #endregion
    }
}
