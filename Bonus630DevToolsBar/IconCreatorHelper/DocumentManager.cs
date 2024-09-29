using Corel.Interop.VGCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.IconLib;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Bonus630DevToolsBar;

namespace br.com.Bonus630DevToolsBar.IconCreatorHelper
{
    public class DocumentManager
    {


        private Corel.Interop.VGCore.Application corelApp;

     

        private FileSystemWatcher fileSystemWatcher;
        private double pageSize = 256;
        private double pageCenterX, pageCenterY;

        readonly Color colorWhite;
        readonly Color colorBlack;
        readonly Color colorGray;
        readonly Color colorYellow;
        readonly Color colorGreen;
        readonly Color colorBlue;
        readonly Color colorRed;
        readonly Color colorOrange;

        readonly string iconLayer = "Icon";
        readonly List<int> PreFixedSizes = new List<int> { 16, 32, 48, 64, 128, 256 };
        readonly List<System.Windows.Controls.Image> ImgsControls;
        readonly Color[] Colors;
        readonly double margin = 16;
        readonly double contourWidth = 16;//antigo 14
        readonly int resolution = 96;
        //precisamos de 96DPI

        private List<int> resolutions = new List<int>();
        System.Windows.Media.Imaging.BitmapImage WhiteImage;
        string ExportFolder = string.Empty;
        string PreviewFolder = string.Empty;


        Thread updatePreviewsThread;
        MarkShapeCache msc;
        //private bool running = false;
        //private bool firtTime = true;

        private Document Doc = null;
        private string GetDocumentFullPath(Document doc)
        {
            if (doc == null)
                return string.Empty;
            return Path.Combine(doc.FilePath, doc.FileName);
        }

        public DocumentManager(object app)
        {

            try
            {

                this.corelApp = app as Corel.Interop.VGCore.Application;
                colorWhite = corelApp.CreateRGBColor(255, 255, 255);
                colorBlack = corelApp.CreateRGBColor(0, 0, 0);
                colorGray = corelApp.CreateRGBColor(210, 211, 213);
                colorYellow = corelApp.CreateRGBColor(215, 171, 105);
                colorGreen = corelApp.CreateRGBColor(137, 209, 133);
                colorBlue = corelApp.CreateRGBColor(117, 189, 255);
                colorRed = corelApp.CreateRGBColor(255, 70, 53);
                colorOrange = corelApp.CreateRGBColor(243, 134, 112);
                Colors = new Corel.Interop.VGCore.Color[] {
                    colorWhite,
                    colorBlack,
                    corelApp.CreateRGBColor(181,181,181),
                    colorGray,
                    colorYellow,
                    colorGreen,
                    corelApp.CreateRGBColor(0,153,51),
                    colorBlue,
                    corelApp.CreateRGBColor(0,92,185),
                    corelApp.CreateRGBColor(6,169,244),
                    colorRed,
                    colorOrange,
                    corelApp.CreateRGBColor(255,102,0)
                };
                msc = new MarkShapeCache(corelApp);
             

                string file = Path.Combine(corelApp.AddonPath, "Bonus630DevToolsBar\\Images\\white-quad.png");
                WhiteImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(file));
            }
            catch
            {
                global::System.Windows.MessageBox.Show("VGCore Erro");
            }


        }
        private bool HasDoc()
        {
            if (Doc == null || corelApp.ActiveDocument == null)
                return false;

            return docFilePath == GetDocumentFullPath(corelApp.ActiveDocument);


        }




        string docFilePath = string.Empty;

    
        public bool CreateDocument()
        {
            Doc = corelApp.CreateDocument();

            Doc.Save();

            if (Doc.Dirty || string.IsNullOrEmpty(Doc.FilePath))
            {
                corelApp.MsgShow("Process is canceled!");
                return false;
            }
            Doc.BeginCommandGroup();
            //HasDoc = true;
            this.corelApp.Unit = cdrUnit.cdrPixel;
            Doc.Unit = cdrUnit.cdrPixel;
            docFilePath = GetDocumentFullPath(Doc);
            Doc.Rulers.HUnits = cdrUnit.cdrPixel;
            Doc.Rulers.VUnits = cdrUnit.cdrPixel;
            Doc.Unit = cdrUnit.cdrPixel;
            Doc.PreserveSelection = true;
            Doc.Resolution = resolution;

            //Coreldraw 2022 '*Bind(DataSource=AppOrDocDataSource;Path=NudgeOffset;BindType=TwoWay)'

            //Coreldraw X8 '*Bind(DataSource=WAppDataSource;Path=NudgeOffset;BindType=TwoWay)'

            //guid = 'a49dc96d-29e7-4848-9747-9ffe643d61ff'

            string ds = "WAppDataSource";

#if X11 || X12 || X13 || X14 || X15
    ds = "AppOrDocDataSource";
#endif

            var dsp = corelApp.FrameWork.Application.DataContext.GetDataSource(ds);
            dsp.SetProperty("NudgeOffset", 846);
            ShapeRange sr = corelApp.CreateShapeRange();
            Page page = Doc.ActivePage;
            for (int C = 0; C < Colors.Length; C++)
            {
                sr.Add(page.ActiveLayer.CreateRectangle(0, 0, 1, 1));
                sr[C + 1].Fill.UniformColor = Colors[C];
            }
            sr.Delete();
            Doc.Save();
            Doc.EndCommandGroup();
            Doc.ClearUndoList();
            StartWatcher();
            return true;
        }

        //https://learn.microsoft.com/en-us/visualstudio/extensibility/ux-guidelines/images-and-icons-for-visual-studio?view=vs-2022
        public void PreparePage(Page page, double pageSize = 256, bool optimization = true)
        {

            // Document doc = CreateDocument();
            // Page page = doc.ActivePage;
            page.SizeWidth = pageSize;
            page.SizeHeight = pageSize;
            page.PrintExportBackground = false;
            page.Name = pageSize.ToString();
            double margin = pageSize / this.margin;
            double contourWidth = pageSize / this.contourWidth;
            //      corelApp.Optimization = true;

            //corelApp.Optimization = false;
            //corelApp.Refresh();
            if (optimization)
                Doc.BeginCommandGroup();
            page.ActiveLayer.Name = iconLayer;
            page.Background = cdrPageBackground.cdrPageBackgroundNone;
            //System.Windows.Forms.MessageBox.Show(page.Layers.Count.ToString());
            //Shape c = page.ActiveLayer.CreateRectangle2(0, 0, pageSize, pageSize);
            //c.Outline.SetNoOutline();
            //c.Fill.ApplyNoFill();
            //c.Name = "Transparent BG";
            //c.Locked = true;





            ShapeRange sr = corelApp.CreateShapeRange();

            sr.Add(page.ActiveLayer.CreateGuide(page.LeftX, page.BottomY, page.LeftX, page.TopY));
            sr.Add(page.ActiveLayer.CreateGuide(page.RightX, page.BottomY, page.RightX, page.TopY));
            sr.Add(page.ActiveLayer.CreateGuide(page.LeftX, page.BottomY, page.RightX, page.BottomY));
            sr.Add(page.ActiveLayer.CreateGuide(page.LeftX, page.TopY, page.RightX, page.TopY));
            sr.Add(page.ActiveLayer.CreateGuide(margin, page.BottomY, margin, page.TopY));
            sr.Add(page.ActiveLayer.CreateGuide(pageSize - margin, page.BottomY, pageSize - margin, page.TopY));
            sr.Add(page.ActiveLayer.CreateGuide(page.LeftX, margin, page.RightX, margin));
            sr.Add(page.ActiveLayer.CreateGuide(page.LeftX, page.TopY - margin, page.RightX, page.TopY - margin));
            sr.Add(page.ActiveLayer.CreateGuide(page.LeftX + margin + contourWidth, page.TopY, page.BottomY + margin + contourWidth, page.BottomY));
            sr.Add(page.ActiveLayer.CreateGuide(page.RightX - margin - contourWidth, page.TopY, page.RightX - margin - contourWidth, page.BottomY));
            sr.Add(page.ActiveLayer.CreateGuide(page.LeftX, page.BottomY + margin + contourWidth, page.RightX, page.BottomY + margin + contourWidth));
            sr.Add(page.ActiveLayer.CreateGuide(page.LeftX, page.TopY - margin - contourWidth, page.RightX, page.TopY - margin - contourWidth));
            sr.Add(page.ActiveLayer.CreateGuide(page.CenterX, page.BottomY, page.CenterX, page.TopY));
            sr.Add(page.ActiveLayer.CreateGuide(page.LeftX, page.CenterY, page.RightX, page.CenterY));
            sr.Add(page.ActiveLayer.CreateGuide(page.LeftX, page.BottomY, page.RightX, page.TopY));
            sr.Add(page.ActiveLayer.CreateGuide(page.LeftX, page.TopY, page.RightX, page.BottomY));
            //for (int i = 1; i <= page.Layers.Count; i++)
            //{
            //    System.Windows.Forms.MessageBox.Show(page.Layers[i].IsGuidesLayer.ToString());
            //}

            //Layer layer = null;
            //if (page.Layers.Count < 2)
            //    layer = page.CreateLayer(string.Format("Guides {0}x{0}", pageSize));
            //else
            //{
            //    layer = page.ActiveLayer;
            //    layer.Name = string.Format("Guides {0}x{0}", pageSize);
            //}
            sr.MoveToLayer(page.GuidesLayer);

            // layer.Printable = false;
            page.GuidesLayer.MoveAbove(page.Layers[1]);
            //layer.Editable = false;
            sr.Lock();
            // c.Layer.Activate();
            if (optimization)
                Doc.EndCommandGroup();

        }
        public string ExportPng(string name, string folder = "D:\\ExportedPNG", int size = 256, Page page = null, bool optimization = true)
        {
            if (optimization)
                Doc.BeginCommandGroup();
            // Page page = Doc.Pages[pageIndex];
            if (page == null)
                return string.Empty;
            ShapeRange sr = GetShapeRange(page);
            if (sr.Count == 0)
                return string.Empty;


            page.Activate();
            Layer l = GetIconLayer(page);

            if (sr.LeftX < page.LeftX || sr.RightX > page.RightX || sr.TopY > page.TopY || sr.BottomY < page.BottomY)
            {
                Shape a = l.CreateRectangle(page.LeftX, page.TopY, page.RightX, page.BottomY);
                Shape b = l.CreateRectangle(sr.LeftX - 0.01, sr.TopY + 0.01, sr.RightX + 0.01, sr.BottomY - 0.01);

                Shape cutter = a.Trim(b, false, false);

                for (int i = sr.Count; i > 0; i--)
                {
                    cutter.Trim(sr[i], true, false);
                }
                cutter.Delete();
                sr.RemoveAll();
                sr = GetShapeRange(page);
            }

            //Shape bg = page.FindShape("Transparent BG");
            //if (bg != null)
            //{
            //    bg.Locked = false;
            //    bg.Fill.ApplyNoFill();
            //    bg.Outline.SetNoOutline();
            //    sr.Add(bg);
            //}

            //string name = corelApp.ActiveShape.Name;
            string pngFile = string.Format("{0}\\{1}.png", folder, name);
            //Shape c = null;
            //if (Math.Round(sr.SizeWidth) < size || Math.Round(sr.SizeHeight) < size)
            //{

            //    c = l.CreateRectangle2(0, 0, size, size);
            //    c.Outline.SetNoOutline();
            //    c.Fill.ApplyNoFill();
            //    c.OrderToBack();
            //    sr.CenterX = l.Page.CenterX;
            //    sr.CenterY = l.Page.CenterY;
            //    sr.Add(c);
            //    //sr.AddToSelection();

            //}
            try
            {

                ExportFilter ef = Doc.ExportBitmap(pngFile, cdrFilter.cdrPNG, cdrExportRange.cdrCurrentPage, cdrImageType.cdrRGBColorImage, size, size, resolution, resolution, Transparent: true, ExportArea: page.BoundingBox);
                ef.Finish();
            }
            catch { return string.Empty; }
            if (optimization)
                Doc.EndCommandGroup();
            //if(c!=null)
            //    c.Delete();
            return pngFile;
        }
        public bool PrepareFiles(string folder)
        {
            if (!HasDoc())
                return false;


            int pageIndex = Doc.ActivePage.Index;
            corelApp.BeginDraw();
            for (int i = 0; i < resolutions.Count; i++)
            {

                ExportPng(resolutions[i].ToString(), folder, resolutions[i], GetPageBySize(resolutions[i]), false);
            }

            GoTo(Doc.Pages[pageIndex].Name);
            corelApp.EndDraw();
            return true;
            // firtTime = false;
        }

        public void ClonePage(Page[] pages)
        {
            ShapeRange sr = GetShapeRange(corelApp.ActivePage);
            for (int i = 0; i < pages.Length; i++)
            {
                sr.Clone();
                sr.MoveToLayer(pages[i].Layers[i]);
            }

        }


        private void GoTo(int resolution)
        {
            if (Doc == null)
                return;
            Page p = GetPageBySize(resolution);
            if (p != null)
            {
                corelApp.EventsEnabled = false;
                p.Activate();
                try
                {
                    double margin = resolution / this.margin;
                    Doc.ActiveWindow.ActiveView.ToFitArea(p.LeftX - margin, p.TopY + margin, p.RightX + margin, p.BottomY - margin);
                    //Doc.ActiveWindow.Refresh();
                }
                catch { }
                finally
                {
                    corelApp.EventsEnabled = true;
                }
            }
        }
        private void GoTo(string pageName)
        {
            int size = 16;
            if (!Int32.TryParse(pageName, out size) && resolutions.Count > 0)
                size = resolutions[0];
            GoTo(size);

        }
        private Page GetPageBySize(int size)
        {
            Page page = null;
            if (Doc == null)
                return page;
            for (int i = 1; i <= Doc.Pages.Count; i++)
            {
                if (Math.Round(Doc.Pages[i].SizeWidth) == size)
                {
                    page = Doc.Pages[i];
                    break;
                }
            }
            return page;
        }
        private Layer GetIconLayer(int size)
        {
            Page p = GetPageBySize(size);

            return GetIconLayer(p);
        }
        private Layer GetIconLayer(Page p)
        {
            if (p != null)
            {
                Layer l = p.Layers.Find(iconLayer);
                return l;
            }
            return null;
        }



        private void InvertBackground(Page page)
        {
            try
            {
                corelApp.BeginDraw();
                if (page.Background == cdrPageBackground.cdrPageBackgroundNone && page.Color.RGBValue != 0)
                {
                    page.Background = cdrPageBackground.cdrPageBackgroundSolid;
                    page.Color = colorBlack;
                }
                else
                {
                    page.Background = cdrPageBackground.cdrPageBackgroundNone;
                    page.Color = colorWhite;
                }
                corelApp.EndDraw();
            }
            catch { }
        }
        private void InvertShapesColors(ShapeRange sr)
        {
            for (int i = sr.Count; i > 0; i--)
            {
                InvertShapeColor(sr[i]);
            }
        }
        public void InvertShapeColor(Shape s)
        {
            if (s.Type == cdrShapeType.cdrGroupShape)
            {

                for (int i = 1; i <= s.Shapes.Count; i++)
                {
                    InvertShapeColor(s.Shapes[i]);
                }
                return;
            }

            if (s.Fill.Type == cdrFillType.cdrPatternFill)
            {
                InvertColor(s.Fill.Pattern.FrontColor);
                InvertColor(s.Fill.Pattern.BackColor);
            }
            if (s.Fill.Type == cdrFillType.cdrFountainFill)
            {
                foreach (FountainColor item in s.Fill.Fountain.Colors)
                {
                    InvertColor(item.Color);
                }
            }

            if (s.Fill.Type == cdrFillType.cdrUniformFill)
            {
                InvertColor(s.Fill.UniformColor);
            }

            if (s.Outline.Type == cdrOutlineType.cdrOutline)
            {
                InvertColor(s.Outline.Color);
            }

        }

        private void ScaleOutline(Shape item)
        {
            if (item.Type == cdrShapeType.cdrGroupShape)
            {
                for (int i = 1; i <= item.Shapes.Count; i++)
                {
                    ScaleOutline(item.Shapes[i]);
                }
                return;
            }
            double pw = item.Outline.PenWidth;
            double ow = item.Outline.Width;
            cdrOutlineType t = item.Outline.Type;
            ///System.Windows.Forms.MessageBox.Show(ow.ToString());
            item.Outline.ScaleWithShape = true;
            if (pw == 0)
                item.Outline.PenWidth = 0;
            if (ow == 0)
                item.Outline.Width = 0;
            item.Outline.Type = t;
        }
        public void InvertColor(Corel.Interop.VGCore.Color color)
        {
            if (color.Type != cdrColorType.cdrColorRGB)
                color.ConvertToRGB();
            color.RGBRed = 255 - color.RGBRed;
            color.RGBGreen = 255 - color.RGBGreen;
            color.RGBBlue = 255 - color.RGBBlue;
        }
        private ShapeRange GetShapeRange(Page p)
        {
            Layer iconLayer = GetIconLayer(p);
            ShapeRange sr = iconLayer.Shapes.All();
            //p.Shapes.All();
            //try
            //{
            //    //    sr.RemoveRange(p.Shapes.FindShapes(Type: cdrShapeType.cdrGuidelineShape));
            //    sr.RemoveRange(p.FindShapes("Transparent BG"));
            //}
            //catch { }
            return sr;
        }

        private void btn_outline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double res = Doc.ActivePage.SizeWidth;
                double contourWidth = res / this.contourWidth;
                if (contourWidth < 2)
                    contourWidth = 2;
                corelApp.BeginDraw();
                ShapeRange sr = corelApp.ActiveSelectionRange;
                for (int i = 1; i <= sr.Count; i++)
                {
                    Shape d = sr[i].Duplicate(0, 0);
                    Effect fx = d.CreateContour(Offset: contourWidth / 2);
                    ShapeRange efx = fx.Separate();
                    // for (int k = 1; k <= efx.Count; k++)
                    //   {
                    efx.BreakApart();
                    efx.SetOutlineProperties(contourWidth, ScaleWithShape: cdrTriState.cdrTrue);

                    efx.ApplyNoFill();
                    //   }
                    efx.Add(sr[i]);
                    efx.Group();
                    d.Delete();
                }
                corelApp.EndDraw();
            }
            catch { }
        }



        private void MenuItemClearPage_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;

            int size = Int32.Parse(item.Tag.ToString());

            Page p = GetPageBySize(size);

            if (p != null)
            {
                ShapeRange sr = GetShapeRange(p);
                sr.Delete();
            }

        }

        int pageCopiedSize = 0;
        private void MenuItemCopyPage_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;

            pageCopiedSize = Int32.Parse(item.Tag.ToString());

            Page p = GetPageBySize(pageCopiedSize);

            if (p != null)
            {
                ShapeRange sr = GetShapeRange(p);
                msc.StoreStaticIdCache(sr, pageCopiedSize);
                //corelApp.ActiveSelectionRange.RemoveFromSelection();
                //sr.AddToSelection();
                //sr.Copy();
            }

        }

        private void MenuItemPastPage_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;

            int size = Int32.Parse(item.Tag.ToString());

            Page p = GetPageBySize(size);

            if (p != null)
            {
                p.Activate();
                Layer l = p.Layers.Find("Icon");
                if (l != null)
                    l.Paste();
            }

        }
        private void MenuItemPastPageProp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;

            int size = Int32.Parse(item.Tag.ToString());

            Page p = GetPageBySize(size);

            if (p != null)
            {
                p.Activate();
                Layer l = p.Layers.Find("Icon");
                if (l != null)
                {
                    this.corelApp.BeginDraw();
                    // var stp = corelApp.CreateStructPasteOptions();

                    //ShapeRange pastedShape = l.PasteEx(null);

                    ShapeRange pastedShape = msc.RecoverStaticIdCache(GetPageBySize(msc.PageSize));
                    if (pastedShape != null)
                        pastedShape = pastedShape.Duplicate();
                    pastedShape.MoveToLayer(l);
                    //Shape pastedShape =  l.Paste();
                    if (pageCopiedSize == 0)
                        return;
                    //  Color c = corelApp.CreateRGBColor(100, 0, 0);
                    //  pastedShape.Fill.ApplyUniformFill(c);

                    Page oriPage = GetPageBySize(pageCopiedSize);
                    double scaleFactor = (double)size / pageCopiedSize;



                    for (int i = 1; i <= pastedShape.Shapes.Count; i++)
                    {
                        ScaleOutline(pastedShape.Shapes[i]);
                    }
                    //ScaleOutline(pastedShape);
                    pastedShape.SizeWidth = pastedShape.SizeWidth * scaleFactor;
                    pastedShape.SizeHeight = pastedShape.SizeHeight * scaleFactor;

                    pastedShape.CenterX = p.CenterX;
                    pastedShape.CenterY = p.CenterY;

                    GoTo(size);
                    this.corelApp.EndDraw();

                }
            }
        }

        private void MenuItemExportPage_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;

            int size = Int32.Parse(item.Tag.ToString());

            Page p = GetPageBySize(size);
            string path = ExportPng(size.ToString(), ExportFolder, size, p, true);
            if (!string.IsNullOrEmpty(path))
            {
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.Filter = "PNG file|*.png";
                sfd.Title = "Save your png";
                sfd.AddExtension = true;
                sfd.DefaultExt = ".png";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        File.Copy(path, sfd.FileName, true);
                    }
                    catch (IOException ioe)
                    {
                        corelApp.MsgShow(ioe.Message);
                    }

                }
            }
        }

        private void btn_centerPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShapeRange sr = corelApp.ActiveSelectionRange;
                sr.CenterX = corelApp.ActivePage.CenterX;
                sr.CenterY = corelApp.ActivePage.CenterY;
            }
            catch { }
        }


        private void IconButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShapeRange sr = GetShapeRange(corelApp.ActivePage);
            corelApp.BeginDraw();
            InvertShapesColors(sr);
            corelApp.EndDraw();
        }

        private void btn_invertBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            InvertBackground(corelApp.ActivePage);
        }

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.KeyStates == DragDropKeyStates.RightMouseButton)
            {
                e.Handled = true;
                return;
            }
            System.Windows.Controls.StackPanel border = sender as System.Windows.Controls.StackPanel;

            int size = Int32.Parse(border.Tag.ToString());

            Debug.WriteLine("Drop-size:" + size, "Drop");

            ShapeRange sr = corelApp.ActiveSelectionRange;
            ShapeRange nSr = corelApp.CreateShapeRange();
            Page p = GetPageBySize(size);

            Layer l = GetIconLayer(p);
            if (l == null)
            {
                e.Handled = true;
                return;
            }

            Page oriPage = sr[1].Page;
            int oriWidth = (int)Math.Round(sr[1].Page.SizeWidth);
            double scaleFactor = (double)size / oriWidth;

            corelApp.BeginDraw();
            if (e.KeyStates == DragDropKeyStates.ControlKey)
            {
                nSr = sr.Duplicate();
#if X14
            sr.Duplicate();
#endif
            }
            else
                nSr = sr;
            foreach (Shape item in nSr)
            {
                ScaleOutline(item);
            }
            double oriDeslX = sr.PositionX - oriPage.LeftX;
            double oriDeslY = oriPage.BottomY + sr.PositionY;
            nSr.MoveToLayer(l);


            nSr.SizeWidth = nSr.SizeWidth * scaleFactor;
            nSr.SizeHeight = nSr.SizeHeight * scaleFactor;
            double deslX = oriDeslX * scaleFactor;
            double deslY = oriDeslY * scaleFactor;

            nSr.PositionX = p.LeftX + deslX;
            nSr.PositionY = p.BottomY + deslY;
            Mouse.OverrideCursor = null;

            GoTo(size);

            corelApp.EndDraw();
            //System.Windows.Forms.MessageBox.Show(sr[1].Page.Name.ToString());
        }
        private void StackPanel_DragOver(object sender, DragEventArgs e)
        {

            Debug.WriteLine("DragOver - ", "Dragging");
            //System.Windows.Controls.Border border = sender as System.Windows.Controls.Border;

            //int size = Int32.Parse(border.Tag.ToString());
            ////e.Effects = DragDropEffects.Copy;
            ////e.Effects = DragDropEffects.None; 
            //Mouse.OverrideCursor = Cursors.Wait;
        }

        private void StackPanel_DragEnter(object sender, DragEventArgs e)
        {
            //Mouse.OverrideCursor = Cursors.Pen;
            //e.Effects = DragDropEffects.None;

            Debug.WriteLine("DragEnter - ", "Dragging");

        }



        private void StackPanel_DragLeave(object sender, DragEventArgs e)
        {

            Debug.WriteLine("DragLeave - ", "Dragging");
            //Mouse.OverrideCursor = null;
        }



    }
}

