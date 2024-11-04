using Bonus630DevToolsBar;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using Corel.Interop.VGCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.IconLib;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace br.com.Bonus630DevToolsBar.IconCreatorHelper
{
    /// <summary>
    /// Interaction logic for IconCreatorHelperUI.xaml
    /// </summary>
    public partial class IconCreatorHelperUI : System.Windows.Controls.UserControl
    {

        private Corel.Interop.VGCore.Application corelApp;

        //private object corelObj;
        private StylesController stylesController;

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

        public IconCreatorHelperUI(object app)
        {
            InitializeComponent();
            try
            {

                this.corelApp = app as Corel.Interop.VGCore.Application;
                stylesController = new StylesController(this.Resources, this.corelApp);
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
                ImgsControls = new List<System.Windows.Controls.Image> { img_16, img_32, img_48, img_64, img_128, img_256 };
                msc = new MarkShapeCache(corelApp);
            }
            catch
            {
                global::System.Windows.MessageBox.Show("VGCore Erro");
            }
            this.Loaded += DockerUI_Loaded;


        }
        private bool HasDoc()
        {
            if (Doc == null || corelApp.ActiveDocument == null)
                return false;

            return docFilePath == GetDocumentFullPath(corelApp.ActiveDocument);


        }
        private void DockerUI_Loaded(object sender, RoutedEventArgs e)
        {
            stylesController.LoadThemeFromPreference();
            ExportFolder = PrepareWorkerFolder();
            PreviewFolder = PreparePreviewFolder();

            string file = Path.Combine(corelApp.AddonPath, "Bonus630DevToolsBar\\Images\\white-quad.png");
            WhiteImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(file));

            if (CheckDocument(this.corelApp.ActiveDocument))
                PrepareDoc(this.corelApp.ActiveDocument.FileName);
        }
        string docFilePath = string.Empty;
        public void Initialize(bool save = true)
        {
            if (CreateDocument(save))
            {
                //if (Doc == null)
                //    return;
                // startThread();
                if (resolutions.Count == 0)
                {
                    PreparePage(Doc.Pages[1], 16);
                    updateCks();
                    //return;
                }
                corelApp.BeginDraw();
                int nPages = Doc.Pages.Count;

                if (nPages < resolutions.Count)
                    Doc.InsertPages(resolutions.Count - nPages, false, 1);


                for (int i = 1; i <= Doc.Pages.Count; i++)
                {
                    PreparePage(Doc.Pages[i], resolutions[i - 1], false);
                }
                corelApp.EndDraw();
                PreparePreviewFolder();
                PrepareFiles(PreviewFolder);
                updateImgs();
                if(save)
                    Doc.Save();
                //if (!Doc.Dirty)
                    lba_DocumentName.Content = Doc.Name;

                Doc.QueryClose += Doc_QueryClose;

                StartWatcher();
            }

        }
        private void OpenDocument()
        {
            System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog();
            of.Multiselect = false;
            of.Filter = "CDR - CorelDRAW (*.cdr)|*.cdr";
            of.Title = "Select a CorelDRAW file";
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if (!CheckDocument(corelApp.OpenDocument(of.FileName)))
                        return;
                    //HasDoc = true;
                    PrepareDoc(of.FileName);
                }
                catch (Exception ex)
                {
                    this.corelApp.MsgShow(ex.Message);
                }
            }
            // startThread();
        }
        private void ImportIcon()
        {
            System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog();
            of.Multiselect = false;
            of.Filter = "Icon (*.ico)|*.ico";
            of.Title = "Select a Icon file";
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    MultiIcon mIcon = new MultiIcon();
                   // mIcon.Load(of.FileName);
                    using (Stream iconStream = new FileStream(of.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        mIcon.Load(iconStream);
                        // mIcon.Count;

                        //IconBitmapDecoder decoder;f
                        //using (Stream iconStream = new FileStream(of.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        //{
                        //     decoder = new IconBitmapDecoder(
                        //            iconStream,
                        //            BitmapCreateOptions.PreservePixelFormat,
                        //            BitmapCacheOption.None);
                        //}
                    }
                    resolutions.Clear();
                    if (mIcon[0].Count > 0)
                    {
                        
                        for (int i = 0; i < mIcon[0].Count; i++)
                        {
                            int width = mIcon[0][i].Icon.Width;
                            if (PreFixedSizes.Contains(width) && !resolutions.Contains(width))
                            {
                                resolutions.Add(width);
                                continue;
                            }
                            int height = mIcon[0][i].Icon.Height;
                            if (PreFixedSizes.Contains(height) && !resolutions.Contains(height))
                                resolutions.Add(height);

                        }
                        //resolutions.Sort();
                        if (resolutions.Count == 0)
                        {
                            if (System.Windows.MessageBox.Show("No standard resolution was found for this icon! it will not be possible to use some tools.",
                                "Do you want to continue?",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning).Equals(MessageBoxResult.No))
                                return;
                        }
                        Initialize(false);
                        if (Doc == null)
                            return;
                        updateCks();
                        for (int i = 0; i < mIcon[0].Count; i++)
                        {
                            //var frame = mIcon[i].Icon
                           
                                string filePath = string.Format("{0}\\{1}.png", ExportFolder, resolutions[i]);
                            //BitmapEncoder encoder = new PngBitmapEncoder();
                            //encoder.Frames.Add(frame);
                            //using (Stream saveStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
                           // {
                                System.Drawing.Bitmap bitmap = mIcon[0][i].Icon.ToBitmap();
                                bitmap.Save(filePath);
                               //  mIcon[i].Icon.Save(saveStream);
                                //encoder.Save(saveStream);
                         //   }
                            if (File.Exists(filePath))
                            {
                                Page p = GetPageBySize(resolutions[i]);
                                p.ActiveLayer.Import(filePath);
                            }
                        }
                        btn_update_Click(null, null);
                    }

                }
                catch (Exception ex)
                {
                    this.corelApp.MsgShow(ex.Message);
                }
            }
        }
        private void PrepareDoc(string filePath)
        {
            docFilePath = filePath;
            lba_DocumentName.Content = Doc.Name;
            ExportFolder = PrepareWorkerFolder();
            this.corelApp.Unit = cdrUnit.cdrPixel;
            Doc.Unit = cdrUnit.cdrPixel;
            PreparePreviewFolder();
            PrepareFiles(PreviewFolder);
            update();
            updateCks();
            updateImgs();
            StartWatcher();
            Doc.QueryClose += Doc_QueryClose;
        }

        private void Doc_QueryClose(ref bool Cancel)
        {

            lba_DocumentName.Content = string.Empty;
            PreparePreviewFolder();


        }
        private bool CheckDocument(Document doc)
        {
            try
            {
                this.Doc = doc;
                if (this.Doc == null)
                    return false;
                for (int i = 0; i < PreFixedSizes.Count; i++)
                {
                    Page p = GetPageBySize(PreFixedSizes[i]);
                    if (p != null)
                        return true;
                }
                this.corelApp.MsgShow("This is not valid Document for this tool!");
                Doc = null;
                return false;
            }
            catch
            {

            }


            return false;
        }
        private void update()
        {
            //while (running)
            //{
            //    if (firtTime)
            //        Thread.Sleep(20000);
            //    Thread.Sleep(5000);
            //    PrepareFiles();
            //Thread.Sleep(2000);
            //this.Dispatcher.Invoke(new Action(() =>
            //{
            startThread();
            //updateImgs();

            //}));

            //}
        }
        private System.Windows.Media.Imaging.BitmapImage GetImage(int size)
        {
            try
            {
                System.Windows.Media.Imaging.BitmapImage image = new System.Windows.Media.Imaging.BitmapImage();
                string file = string.Format("{0}\\{1}.png", PreviewFolder, size);
                if (File.Exists(file))
                {
                    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        if (fs != null)
                        {
                            image.BeginInit();
                            image.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                            image.StreamSource = fs;
                            image.EndInit();
                        }
                    }
                }
                else
                {
                    return null;
                    //return WhiteImage;
                }
                return image;
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        private void updateImgs()
        {

            for (int i = 0; i < ImgsControls.Count; i++)
            {
                updateImg(ImgsControls[i], PreFixedSizes[i]);
            }
        }
        private void updateImg(System.Windows.Controls.Image img, int size)
        {
            this.Dispatcher.Invoke(() =>
            {
                img.Source = GetImage(size);
            });
        }
        private void updateImgByFileName(string fileName)
        {
            int size = Int32.Parse(fileName.Replace(".png", ""));
            int index = PreFixedSizes.IndexOf(size);
            updateImg(ImgsControls[index], size);
        }
        private void updateCks()
        {
            var cks = new System.Windows.Controls.CheckBox[] { ck_16, ck_32, ck_48, ck_64, ck_128, ck_256 };
            for (int i = 0; i < cks.Length; i++)
            {
                checkCk(cks[i], PreFixedSizes[i]);
            }
        }
        private void checkCk(System.Windows.Controls.CheckBox ck, int size)
        {
            Page p = GetPageBySize(size);
            if (p != null)
            {

                ck.IsChecked = true;
                if (!resolutions.Contains(size))
                     resolutions.Add(size);
            }
            else
            {
                ck.IsChecked = false;
                if (resolutions.Contains(size))
                    resolutions.Remove(size);
            }
        }
        private void ck_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox ck = sender as System.Windows.Controls.CheckBox;

            int size = Int32.Parse(ck.Tag.ToString());

            if ((bool)ck.IsChecked && !resolutions.Contains(size))
            {
                resolutions.Add(size);
                resolutions.Sort();
                if (HasDoc())
                {
                    Page p = GetPageBySize(size);
                    if (p == null)
                    {
                        int index = resolutions.IndexOf(size);
                        bool before = false;

                        if (index == 0)
                        {
                            before = true;
                            index = 1;
                        }


                        p = Doc.InsertPagesEx(1, before, index, (double)size, (double)size);
                        PreparePage(p, size);
                    }
                }

            }
            else
            {
                if (resolutions.Count == 1)
                {
                    ck.IsChecked = true;
                    corelApp.MsgShow("You cannot delete the last page of the document");
                    return;
                }
                if (HasDoc())
                {

                    Page p = null;
                    p = GetPageBySize(size);
                    if (p != null)
                        p.Delete();
                    int index = resolutions.IndexOf(size);
                    if (index < resolutions.Count - 2)
                        index++;
                    if (index > resolutions.Count - 2)
                        index--;
                    resolutions.Remove(size);
                    resolutions.Sort();
                    GoTo(resolutions[index]);
                    try
                    {
                        string file = string.Format("{0}\\{1}.png", PreviewFolder, size);
                        if(File.Exists(file))
                            File.Delete(file);
                    }
                    catch { }
                    //Doc.Pages[resolutions.IndexOf(size) + 1].Delete();
                }
            }

        }


        private void startThread()
        {
            // running = true;

            updatePreviewsThread = new Thread(ThreadWork);
            updatePreviewsThread.IsBackground = true;
            updatePreviewsThread.Priority = ThreadPriority.Lowest;
            updatePreviewsThread.Start();
        }
        private void ThreadWork()
        {
            PreparePreviewFolder();
            PrepareFiles(PreviewFolder);
        }
        public bool CreateDocument(bool save = true)
        {
            Doc = corelApp.CreateDocument();

            if (save)
            {
                Doc.Save();

                if (Doc.Dirty || string.IsNullOrEmpty(Doc.FilePath))
                {
                    corelApp.MsgShow("Process is canceled!");
                    return false;
                }
                docFilePath = GetDocumentFullPath(Doc);
            }
            Doc.BeginCommandGroup();
            //HasDoc = true;
            this.corelApp.Unit = cdrUnit.cdrPixel;
            Doc.Unit = cdrUnit.cdrPixel;
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
            if(save)
                Doc.Save();
            Doc.EndCommandGroup();
            Doc.ClearUndoList();

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
        private bool ExportingPng = false;
        public string ExportPng(string name, string folder = "D:\\ExportedPNG", int size = 256, Page page = null, bool optimization = true, bool cut = false)
        {
            try
            {
                if (ExportingPng)
                    return string.Empty;
                ExportingPng = true;
                if (optimization)
                    Doc.BeginCommandGroup();
                // Page page = Doc.Pages[pageIndex];
                if (page == null)
                {
                    ExportingPng = false;
                    return string.Empty;
                }
                ShapeRange sr = GetShapeRange(page);
                if (sr.Count == 0)
                {
                    ExportingPng = false;
                    return string.Empty;
                }


                page.Activate();
                Layer l = GetIconLayer(page);

                if (cut)
                {
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
                ExportingPng = false;
                return pngFile;
            }

            catch
            {
                ExportingPng = false;
                return "";
            }
            finally
            {

            }
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
        public string PrepareWorkerFolder()
        {
            string folder = Path.Combine(Path.GetTempPath(), "bonus630\\IconCreator\\ExportFolder");
            ExportFolder = PrepareFolder(folder);
            return ExportFolder;
        }
        public string PreparePreviewFolder()
        {
            string folder = Path.Combine(Path.GetTempPath(), "bonus630\\IconCreator\\Preview");
            PreviewFolder = PrepareFolder(folder);
            return PreviewFolder;
        }
        private string PrepareFolder(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                DirectoryInfo di = new DirectoryInfo(folder);
                foreach (var item in di.GetFiles())
                {
                    try
                    {
                        item.Delete();
                    }
                    catch { }
                }
                return folder;
            }
            catch
            {
                return string.Empty;
            }
        }

        private List<FileInfo> GetFiles(string folder)
        {

            DirectoryInfo dirInfo = new DirectoryInfo(folder);
            FileInfo[] fi = dirInfo.GetFiles();
            List<FileInfo> list = fi.OrderBy(f => Int32.Parse(f.Name.Replace(".png", ""))).ToList<FileInfo>();
            return list;
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

        //ImageMagickMethod
        //public void ExportToIco()
        //{

        //    try
        //    {
        //        string[] files = Directory.GetFiles(WorkerFolder);
        //        if (files.Length == 0)
        //            return;
        //        using (var icon = new MagickImageCollection())
        //        {
        //            for (int i = 0; i < files.Length; i++)
        //            {
        //                icon.Add(new MagickImage(files[i]));
        //            }
        //            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
        //            sfd.Filter = "ico file|*.ico";
        //            sfd.Title = "Save your ico";
        //            sfd.AddExtension = true;
        //            sfd.DefaultExt = ".ico";
        //            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //            {

        //                // Salvar o ícone
        //                icon.Write(sfd.FileName);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        corelApp.MsgShow(e.Message);
        //    }
        //}
        public void ExportToIco()
        {
            try
            {
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.Filter = "ico file|*.ico";
                sfd.Title = "Save your ico";
                sfd.AddExtension = true;
                sfd.DefaultExt = ".ico";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ExportToIco(sfd.FileName);
                }

            }
            catch (Exception e)
            {
                corelApp.MsgShow(e.Message);
            }
        }
        public void ExportToIco(string path)
        {
            try
            {
                var files = GetFiles(ExportFolder);
                MultiIcon mIcon = new MultiIcon();
                SingleIcon sIcon = mIcon.Add(Doc.Name.Replace(".cdr", ""));
                System.Drawing.Bitmap bitmap16;
                for (int i = 0; i < files.Count; i++)
                {
                    Console.WriteLine(files[i].FullName);
                    bitmap16 = new System.Drawing.Bitmap(System.Drawing.Bitmap.FromFile(files[i].FullName));
                    sIcon.Add(bitmap16);
                    if (i == files.Count - 1)
                    {
                        sIcon[sIcon.Count - 1].IconImageFormat = IconImageFormat.PNG;
                    }
                }
                mIcon.SelectedIndex = 0;
                mIcon.Save(path, MultiIconFormat.ICO);
            }
            catch (Exception e)
            {
                corelApp.MsgShow(e.Message);
            }
        }

        private int lastSize = 0;
        private async Task IconIsReadyAsync(string path)
        {
            Debug.WriteLine("eNTER ICON");
            int security = 0;
            while (!File.Exists(path) && security < 500)
            {
                await Task.Delay(40);
                security++;
                Debug.WriteLine(security);
            }
        }
        private async Task SetIconAsync(string path)
        {

            if (File.Exists(path))
            {
                try
                {
                    await Task.Run(() =>
                    {
                        Debug.WriteLine("ENTER PREVIEW");
                        var commandBar = corelApp.FrameWork.CommandBars["Bonus630 Dev Tools"];

                        foreach (Corel.Interop.VGCore.Control item in commandBar.Controls)
                        {
                            if (item.ID.Equals("657042cb-3594-43a1-80bf-c8a27fd43146"))
                                item.SetIcon2(path);
                        }
                    });
                }
                catch
                {
                    //this.Dispatcher.Invoke(() =>
                    //{
                    //    (sender as System.Windows.Controls.Button).IsEnabled = true;
                    //});
                }
            }
        }

        private Thread updatePreviewThread;
        private void startupdatePreviewThread(int size)
        {
            // running = true;
            if (updatePreviewThread == null || updatePreviewThread.ThreadState == System.Threading.ThreadState.Stopped)
            {
                updatePreviewThread = new Thread(new ParameterizedThreadStart(UpdatePreviewThreadWork));
                updatePreviewThread.IsBackground = true;
                updatePreviewThread.Priority = ThreadPriority.Lowest;
                updatePreviewThread.Start(size);
            }
        }
        private void UpdatePreviewThreadWork(object size)
        {
            //string folder = Path.Combine(Path.GetTempPath(), "bonus630\\IconCreator\\Preview");
            string pngFile = string.Empty;
            Debug.WriteLine("UpdatePreviewThreadWork Start - " + size, "Threads");
            int safeLimiter = 10;
            while (pngFile == string.Empty && safeLimiter > -1)
            {
                pngFile = ExportPng(size.ToString(), PreviewFolder, (int)size, GetPageBySize((int)size), true);
                if (pngFile == string.Empty)
                {
                    if (File.Exists(pngFile))
                    {
                        File.Delete(pngFile);
                    }
                }
                safeLimiter--;
                Thread.Sleep(100);
            }
            // updateImgByFileName(size.ToString());
            Debug.WriteLine("UpdatePreviewThreadWork End - " + size, "Threads");
        }

        private void StartWatcher()
        {
            fileSystemWatcher = new FileSystemWatcher(PreviewFolder);
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.Filter = "*.png";
            fileSystemWatcher.Changed += (s, ef) =>
            {
                updateImgByFileName(ef.Name);
            };
            //fileSystemWatcher.Deleted += (s, ef) => { updateImgByFileName(ef.Name); };
            fileSystemWatcher.Created += (s, ef) =>
            {
                updateImgByFileName(ef.Name);
            };
            fileSystemWatcher.Deleted += (s, ef) =>
            {
                updateImgByFileName(ef.Name);
            };
            //Doc.ShapeCreate += (s) => { try { startupdatePreviewThread(Int32.Parse(corelApp.ActivePage.Name)); } catch { } };
            //Doc.ShapeDistort += (s) => { try { startupdatePreviewThread(Int32.Parse(corelApp.ActivePage.Name)); } catch { } };
            //Doc.ShapeMove += (s) => { try { startupdatePreviewThread(Int32.Parse(corelApp.ActivePage.Name)); } catch { } };
            //Doc.ShapeTransform += (s) => { try { startupdatePreviewThread(Int32.Parse(corelApp.ActivePage.Name)); } catch { } };
            //Doc.ShapeChange += (Shape Shape, cdrShapeChangeScope Scope) =>
            //{
            //    try
            //    {

            //        startupdatePreviewThread(Int32.Parse(corelApp.ActivePage.Name));

            //    }
            //    catch { }
            //};
            Doc.ShapeCreate += (s) => { 
                try {
                    Debug.WriteLine(s.Type.ToString());
                    startupdatePreviewThread(Int32.Parse(s.Layer.Page.Name));
                } catch { } };
            Doc.ShapeDistort += (s) => { try { startupdatePreviewThread(Int32.Parse(s.Layer.Page.Name)); } catch { } };
            Doc.ShapeMove += (s) => { try { startupdatePreviewThread(Int32.Parse(s.Layer.Page.Name)); } catch { } };
            Doc.ShapeTransform += (s) => { try { startupdatePreviewThread(Int32.Parse(s.Layer.Page.Name)); } catch { } };
            Doc.ShapeChange += (Shape s, cdrShapeChangeScope Scope) =>
            {
                try
                {

                    startupdatePreviewThread(Int32.Parse(s.Layer.Page.Name));

                }
                catch { }
            };
            Doc.ShapeDelete += (count) =>
            {
                try
                {
                    ShapeRange sr = GetShapeRange(corelApp.ActivePage);
                    if (sr.Count == 0)
                    {
                        string pngFile = string.Format("{0}\\{1}.png", PreviewFolder, corelApp.ActivePage.Name);
                        if (File.Exists(pngFile))
                        {
                            File.Delete(pngFile);
                        }
                    }
                    else
                        startupdatePreviewThread(Int32.Parse(corelApp.ActivePage.Name));
                    //ShapeRange sr = GetShapeRange(corelApp.ActivePage);
                    //if (sr.Count <= count)
                    //{

                    //    string pngFile = string.Format("{0}\\{1}.png", PreviewFolder, corelApp.ActivePage.Name);
                    //    if (File.Exists(pngFile))
                    //    {
                    //        File.Delete(pngFile);
                    //    }
                    //    updateImgByFileName(corelApp.ActivePage.Name);
                    //}
                }
                catch { }
            };
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
                if (int.Parse(Doc.Pages[i].Name) == size)
                {
                    page = Doc.Pages[i];
                    break;
                }
            }
            return page;
        }
        private ShapeRange GetShapeRangeBySize(int size)
        {
            Page p = GetPageBySize(size);
            ShapeRange sr = corelApp.CreateShapeRange();
            if (p != null)
                sr = GetShapeRange(p);
            return sr;
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
            Debug.WriteLine(sr.Count);
            //p.Shapes.All();
            //try
            //{
            //    //    sr.RemoveRange(p.Shapes.FindShapes(Type: cdrShapeType.cdrGuidelineShape));
            //    sr.RemoveRange(p.FindShapes("Transparent BG"));
            //}
            //catch { }
            return sr;
        }
        private void PastShapesProp(int size, bool enableEvents = true)
        {
            //Temos um problema aqui
            Page p = GetPageBySize(size);

            if (p != null)
            {
                p.Activate();
                Layer l = p.Layers.Find("Icon");
                if (l != null)
                {
                    this.corelApp.BeginDraw(enableEvents: enableEvents);
                    // var stp = corelApp.CreateStructPasteOptions();

                    //ShapeRange pastedShape = l.PasteEx(null);

                    ShapeRange pastedShape = msc.RecoverStaticIdCache(GetPageBySize(msc.PageSize));
                    if (pastedShape == null || pastedShape.Count == 0)
                        return;
                    double leftX = msc.Left;
                    double bottomY = msc.Bottom;
                    if (pastedShape != null)
                        pastedShape = pastedShape.Duplicate();
                    pastedShape.MoveToLayer(l);
                    //Shape pastedShape =  l.Paste();
                    // if (pageCopiedSize == 0)
                    //      return;
                    //  Color c = corelApp.CreateRGBColor(100, 0, 0);
                    //  pastedShape.Fill.ApplyUniformFill(c);

                    //Page oriPage = GetPageBySize(pageCopiedSize);
                    double scaleFactor = (double)size / msc.PageSize;



                    for (int i = 1; i <= pastedShape.Shapes.Count; i++)
                    {
                        ScaleOutline(pastedShape.Shapes[i]);
                    }


                    //ScaleOutline(pastedShape);
                    pastedShape.SizeWidth = pastedShape.SizeWidth * scaleFactor;
                    pastedShape.SizeHeight = pastedShape.SizeHeight * scaleFactor;
                    pastedShape.LeftX = leftX * scaleFactor;
                    pastedShape.BottomY = bottomY * scaleFactor;
                    //pastedShape.CenterX = p.CenterX;
                    //pastedShape.CenterY = p.CenterY;


                    this.corelApp.EndDraw();

                }
            }
        }
        private void CopyShapesToProp()
        {
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

        #region Wpf events
        private void btn_prepareDocument_Click(object sender, RoutedEventArgs e)
        {
            Initialize();
        }
        private void btn_ImportIcon_Click(object sender, RoutedEventArgs e)
        {
            ImportIcon();
        }
        private void btn_exportIcon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrepareWorkerFolder();
                PrepareFiles(ExportFolder);
                ExportToIco();
            }
            catch { }
        }

        private void btn_openDocumento_Click(object sender, RoutedEventArgs e)
        {
            OpenDocument();
        }

        private async void btn_previewIcon_Click(object sender, RoutedEventArgs e)
        {

#if X7
    System.Windows.Forms.MessageBox.Show("This function is available on X8 or higher");
    return;
#endif
            try
            {
                string path = string.Format("{0}bonus630\\IconCreator\\{1}.ico", Path.GetTempPath(), Guid.NewGuid());
                PrepareWorkerFolder();
                if (PrepareFiles(ExportFolder))
                {
                    ExportToIco(path);

                    await IconIsReadyAsync(path);
                    await SetIconAsync(path);
                }
                //this.Dispatcher.Invoke(() =>
                //{
                //    (sender as System.Windows.Controls.Button).IsEnabled = true;
                //});
                //try
                //{
                //    if (File.Exists(path))
                //        File.Delete(path);

                //}
                //catch
                //{
                //    return;
                //}


            }
            catch
            {
                //this.Dispatcher.Invoke(() =>
                //{
                //    (sender as System.Windows.Controls.Button).IsEnabled = true;
                //});
            }
        }
        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double res = Doc.ActivePage.SizeWidth;
                update();
                GoTo((int)res);
            }
            catch { }
        }
        private void btn_outline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double res = Doc.ActivePage.SizeWidth;
                double contourWidth = res / this.contourWidth;
                if (contourWidth < 2)
                    contourWidth = 2;
                corelApp.BeginDraw(enableEvents: true);
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

        private void img_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                System.Windows.Controls.Border border = sender as System.Windows.Controls.Border;
                int size = Int32.Parse(border.Tag.ToString());
                if (resolutions.Contains(size))
                {
                    GoTo(size);
                    // if (size != lastSize)
                    //    update();
                    lastSize = size;
                }
            }
            catch
            {
                Debug.WriteLine("teste");
            }

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

            CopyShapesToProp();


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
            PastShapesProp(size);
            GoTo(size);
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

        private void MenuItemReplacePageg_Click(object sender, RoutedEventArgs e)
        {
            corelApp.EventsEnabled = false;
            MenuItemCopyPage_Click(sender, e);
            for (int i = 0; i < resolutions.Count; i++)
            {
                if (resolutions[i] != pageCopiedSize)
                {
                    GetShapeRangeBySize(resolutions[i]).Delete();
                    PastShapesProp(resolutions[i], false);
                }
            }

            update();

            updateImgs();


            GoTo(pageCopiedSize);
            corelApp.EventsEnabled = true;
        }


        private void StackPanel_DragLeave(object sender, DragEventArgs e)
        {

            Debug.WriteLine("DragLeave - ", "Dragging");
            //Mouse.OverrideCursor = null;
        }

        #endregion

    }
}
