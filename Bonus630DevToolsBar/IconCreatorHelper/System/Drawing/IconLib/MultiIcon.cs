
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Drawing.IconLib.Exceptions;
using System.Drawing.IconLib.EncodingFormats;

namespace System.Drawing.IconLib
{
   
    public class MultiIcon : List<SingleIcon>
    {
        #region Variables Declaration
        private int mSelectedIndex  = -1;
        #endregion

        #region Constructors
        public MultiIcon()
        {
        }

        public MultiIcon(IEnumerable<SingleIcon> collection)
        {
            AddRange(collection);
        }

        public MultiIcon(SingleIcon singleIcon)
        {
            Add(singleIcon);
            SelectedName = singleIcon.Name;
        }
        #endregion

        #region Properties
        public int SelectedIndex
        {
            get {return mSelectedIndex;}
            set
            {
                if (value >= Count)
                    throw new ArgumentOutOfRangeException("SelectedIndex");

                mSelectedIndex = value;
            }
        }

        public string SelectedName
        {
            get
            {
                if (mSelectedIndex < 0 || mSelectedIndex >= Count)
                    return null;

                return this[mSelectedIndex].Name;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("SelectedName");

                for(int i=0; i<Count; i++)
                    if (this[i].Name.ToLower() == value.ToLower())
                    {
                        mSelectedIndex = i;
                        return;
                    }

                throw new InvalidDataException("SelectedName does not exist.");
            }
        }

        public string[] IconNames
        {
            get 
            {
                List<string> names = new List<string>();
                foreach(SingleIcon icon in this)
                    names.Add(icon.Name);
                return names.ToArray();
            }
        }
        #endregion

        #region Indexers
        public SingleIcon this[string name]
        {
            get
            {
                for(int i=0; i<Count; i++)
                    if (this[i].Name.ToLower() == name.ToLower())
                        return this[i];
                return null;
            }
        }
        #endregion

        #region Public Methods
        public SingleIcon Add(string iconName)
        {
            // Already exist?
            if (Contains(iconName))
                throw new IconNameAlreadyExistException();

            // Lets Create the icon group
            // Add group to the master list and also lets give a name
            SingleIcon singleIcon = new SingleIcon(iconName);
            this.Add(singleIcon);
            return singleIcon;
        }

        public void Remove(string iconName)
        {
            if (iconName == null)
                throw new ArgumentNullException("iconName");

            // If not exist then do nothing
            int index = IndexOf(iconName);
            if (index == -1)
                return;

            RemoveAt(index);
        }

        public bool Contains(string iconName)
        {
            if (iconName == null)
                throw new ArgumentNullException("iconName");

            // Exist?
            return IndexOf(iconName) != -1 ? true : false;
        }

        public int IndexOf(string iconName)
        {
            if (iconName == null)
                throw new ArgumentNullException("iconName");

            // Exist?
            for(int i=0; i<Count; i++)
                if (this[i].Name.ToLower() == iconName.ToLower())
                    return i;
            return -1;
        }

        public void Load(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            try
            {
                Load(fs);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        public void Load(Stream stream)
        {
            ILibraryFormat baseFormat;

            if ((baseFormat = new IconFormat()).IsRecognizedFormat(stream))
            {
                if (mSelectedIndex == -1)
                {
                    this.Clear();
                    this.Add(baseFormat.Load(stream)[0]);
                    this[0].Name = "Untitled";
                }
                else
                {
                    string currentName = this[mSelectedIndex].Name;
                    this[mSelectedIndex] = baseFormat.Load(stream)[0];
                    this[mSelectedIndex].Name = currentName;
                }
            }
           
            else
                throw new InvalidFileException();

            SelectedIndex = Count > 0 ? 0 : -1;
        }

        public void Save(string fileName, MultiIconFormat format)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            try
            {
                Save(fs, format);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        public void Save(Stream stream, MultiIconFormat format)
        {
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

            switch (format)
            {
                case MultiIconFormat.ICO:
                    if (mSelectedIndex == -1)
                        throw new InvalidIconSelectionException();

                    new IconFormat().Save(this, stream);
                    break;
               
                case MultiIconFormat.EXE:
                case MultiIconFormat.OCX:
                case MultiIconFormat.CPL:
                case MultiIconFormat.SRC:
                    throw new NotSupportedException("File format not supported");
                default:
                    throw new NotSupportedException("Unknow file type destination, Icons can't be saved");
            }
        }
        #endregion

        #region Private Methods
        private void CopyFrom(MultiIcon multiIcon)
        {
            mSelectedIndex = multiIcon.mSelectedIndex;
            Clear();
            AddRange(multiIcon);
        }
        #endregion
    }
}
