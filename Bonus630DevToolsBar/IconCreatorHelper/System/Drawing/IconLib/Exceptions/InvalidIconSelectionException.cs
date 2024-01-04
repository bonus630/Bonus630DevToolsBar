namespace System.Drawing.IconLib.Exceptions
{
    
    [Serializable]
    public class InvalidIconSelectionException : Exception
    {
        #region Constructors
        public InvalidIconSelectionException() : base ("Selected Icon is invalid")
        {
        }
        #endregion
    }
}
