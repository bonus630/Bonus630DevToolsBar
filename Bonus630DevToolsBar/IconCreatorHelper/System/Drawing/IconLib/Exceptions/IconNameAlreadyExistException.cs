
namespace System.Drawing.IconLib.Exceptions
{
   
    [Serializable]
    public class IconNameAlreadyExistException : Exception
    {
        #region Constructors
        public IconNameAlreadyExistException() : base ("Icon name already exist in the collection")
        {
        }
        #endregion
    }
}
