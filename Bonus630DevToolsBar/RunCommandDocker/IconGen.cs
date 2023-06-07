using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public static class IconGen
    {

        static Bitmap GetBitmap(string commandName)
        {
            Bitmap bitmap = new Bitmap(64, 64);
            Graphics g = Graphics.FromImage(bitmap);
           Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            g.FillRectangle(Brushes.Blue, rectangle);
            g.DrawString(commandName[0].ToString(), new Font("Arial",10), Brushes.White,new PointF(10,10));
            return bitmap;
        }
    }
}
