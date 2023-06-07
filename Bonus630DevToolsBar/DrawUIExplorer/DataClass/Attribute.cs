using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass
{
    public class Attribute : System.IComparable
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsGuid { get { return this.isGuid(); } }
        private Regex reg = new Regex("[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}",RegexOptions.Compiled|RegexOptions.IgnoreCase);

        public Attribute(string name,string value)
        {
            this.Name = name;
            this.Value = value;
        }
        public override string ToString()
        {
            return string.Format("{0}=\'{1}\'",this.Name,this.Value);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Attribute att = obj as Attribute;
            if (att == null)
                return false;
            if (att.Name == this.Name && att.Value == this.Value)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -244751520;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }

        private bool isGuid()
        {
            bool result = false;
            if (this.Name == "guid")
                return true;
            result = reg.IsMatch(this.Value);


            return result;

        }

        public int CompareTo(object obj)
        {
            int result = 1;
            if (obj != null)
            {
                Attribute att = obj as Attribute;
                result = att.Name.CompareTo(this.Name);
            }
            return result;
        }
    }
}
