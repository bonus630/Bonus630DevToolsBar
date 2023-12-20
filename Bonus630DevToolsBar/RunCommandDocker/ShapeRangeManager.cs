using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Corel.Interop.VGCore;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public class ShapeRangeManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        Application corelApp;
        List<int> ShapesIds = new List<int>();

        public ShapeRangeManager(Application app)
        {
            this.corelApp = app;
        }
        public int Count { get { return ShapesIds.Count; } }
        public ShapeRange GetShapes(Command command = null)
        {

            ShapeRange sr = corelApp.CreateShapeRange();
            try
            {
                for (int i = 0; i < ShapesIds.Count; i++)
                {
                    sr.Add(corelApp.ActivePage.Shapes.FindShape(StaticID: ShapesIds[i]));
                }
            }
            catch { }
            
            return sr;

        }

        public void AddActiveSelection()
        {
            try
            {

                for (int i = 1; i <= corelApp.ActiveSelection.Shapes.Count; i++)
                {
                    int id = corelApp.ActiveSelection.Shapes[i].StaticID;
                    if (!ShapesIds.Contains(id))
                        ShapesIds.Add(id);
                }
            }
            catch { }
            OnPropertyChanged("Count");
        }
        public void RemoveActiveSelection()
        {
            try
            {
                for (int i = 1; i <= corelApp.ActiveSelection.Shapes.Count; i++)
                {
                    ShapesIds.Remove(ShapesIds.Single(r => r == corelApp.ActiveSelection.Shapes[i].StaticID));
                }
            }
            catch { }
            OnPropertyChanged("Count");
        }

        public void Clear()
        {
            ShapesIds.Clear();
            OnPropertyChanged("Count");
        }
        public void SetSelection()
        {
            try
            {
                ShapeRange sr = this.GetShapes();
                sr.CreateSelection();
            }
            catch
            { }
        }
    }
}
