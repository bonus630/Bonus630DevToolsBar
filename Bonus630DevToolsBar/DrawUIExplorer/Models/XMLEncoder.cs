using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Models
{
    public class XmlEncoder
    {
        private DataClass.IBasicData _basicData;
        private StringBuilder stringBuilder = new StringBuilder();
        private Thread thread;
        public event Action<string> XmlRead;

        public void xmlEncode(DataClass.IBasicData basicData)
        {
            _basicData = basicData;
            thread = new Thread(() => xmlE(_basicData));
            thread.IsBackground = true;
            thread.Start();



        }

        private void xmlE(object bData)
        {
            DataClass.IBasicData basicData = bData as DataClass.IBasicData;
            stringBuilder.Append("<");
            stringBuilder.Append(basicData.TagName);


            foreach (DataClass.Attribute item in basicData.Attributes)
            {
                stringBuilder.Append(" ");
                stringBuilder.Append(item.ToString());
            }
            stringBuilder.Append(">\n");
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                for (int r = 0; r < (basicData.Childrens[i].TreeLevel - _basicData.TreeLevel); r++)
                {
                    stringBuilder.Append("\t");
                }

                xmlE(basicData.Childrens[i]);
            }
            for (int r = 0; r < (basicData.TreeLevel - _basicData.TreeLevel); r++)
            {
                stringBuilder.Append("\t");
            }
            stringBuilder.Append(string.Format("</{0}>\n", basicData.TagName));
            if (XmlRead != null && basicData.Equals(_basicData))
                XmlRead(stringBuilder.ToString());
        }
        public string xmlEncode1(DataClass.IBasicData basicData)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<");
            sb.Append(basicData.TagName);
            //if (!string.IsNullOrEmpty(basicData.Guid))
            //{
            //    sb.Append(" guid=\"");
            //    sb.Append(basicData.Guid);
            //    sb.Append("\"");
            //}
            //if (!string.IsNullOrEmpty(basicData.GuidRef))
            //{
            //    sb.Append(" guidRef=\"");
            //    sb.Append(basicData.GuidRef);
            //    sb.Append("\"");
            //}
            foreach (DataClass.Attribute item in basicData.Attributes)
            {
                sb.Append(" ");
                sb.Append(item.ToString());
            }
            sb.Append(">\n\r</");
            sb.Append(basicData.TagName);
            sb.Append(">");

            return sb.ToString();
        }
    }
}
