using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace br.com.Bonus630DevToolsBar.CustomControls
{
    /// <summary>
    /// Interaction logic for DraggableGrid.xaml
    /// </summary>
    public partial class DraggableGrid : StackPanel
    {
        private bool isDragging = false;
        private Point offset;
        public Rectangle DragIndicator { get; set; }
        public DraggableGrid()
        {

            // Adicionando a faixa de arraste
            DragIndicator = new Rectangle
            {
                Fill = Brushes.Red,
                Height = 12,
                Width = 20,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 4, 0, 0)
            };
            DragIndicator.MouseDown += OnDragPanelMouseDown;
            DragIndicator.MouseMove += OnDragPanelMouseMove;
            DragIndicator.MouseUp += OnDragPanelMouseUp;
            DragIndicator.MouseLeave += DragIndicator_MouseLeave;
            DragIndicator.MouseEnter += DragIndicator_MouseEnter;
            //this.Items.Add(DragIndicator);
            Children.Add(DragIndicator);
        }

        private void DragIndicator_MouseEnter(object sender, MouseEventArgs e)
        {


            // Mouse.OverrideCursor = Cursors.ScrollAll;
        }

        private void DragIndicator_MouseLeave(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Mouse Leave", "Dragging");
              isDragging = false;
             Mouse.OverrideCursor = null;
        }

        private void DraggableGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Lost focus", "Dragging");
            isDragging = false;
        }

        private void OnDragPanelMouseDown(object sender, MouseButtonEventArgs e)
        {

            isDragging = true;
            Mouse.OverrideCursor = Cursors.ScrollAll;
            offset = e.GetPosition(this);

            Debug.WriteLine("MouseOver - " + isDragging, "Dragging");
            Debug.WriteLine("offset - " + offset, "Dragging");
        }

        private void OnDragPanelMouseMove(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("MouseMove - " + isDragging, "Dragging");
            if (isDragging)
            {
                Point mousePosition = e.GetPosition(null);
                //Thickness margin = new Thickness(mousePosition.X - offset.X, mousePosition.Y - offset.Y,0, 0);
                //this.Margin = margin;
                // Calcula a nova posição do Grid levando em consideração a tela e a margem
                //double newLeft = mousePosition.X - offset.X;
                //double newTop = mousePosition.Y - offset.Y - 31;
                double newLeft = mousePosition.X;
                double newTop = mousePosition.Y;

                Point m = Mouse.GetPosition(this);
                Debug.WriteLine("M - " + offset, "Dragging");
                Point m2 = Mouse.GetPosition(null);
                Debug.WriteLine("M2 - " + offset, "Dragging");

                if (newLeft > 280)
                    newLeft = 280;
                if (newTop < 20)
                    newTop = 20;

                Thickness margin = new Thickness(newLeft, newTop, 0, 0);
                this.Margin = margin;
                Debug.WriteLine("Left: " + newLeft + " - Top: " + newTop, "Dragging");
            }
        }

        private void OnDragPanelMouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("MouseUp - " + isDragging, "Dragging");
            isDragging = false;
            Mouse.OverrideCursor = null;
        }
    }
}
