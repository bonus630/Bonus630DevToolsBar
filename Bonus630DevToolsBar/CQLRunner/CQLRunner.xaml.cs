using br.com.Bonus630DevToolsBar.RecentFiles;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using Corel.Interop.VGCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using corel = Corel.Interop.VGCore;

namespace br.com.Bonus630DevToolsBar.CQLRunner
{
    /// <summary>
    /// Interaction logic for CQLRunner.xaml
    /// </summary>
    public partial class CQLRunner : UserControl
    {

        private corel.Application corelApp;
        private StylesController stylesController;
        private System.Windows.Forms.AutoCompleteStringCollection CQLSucessedList = new System.Windows.Forms.AutoCompleteStringCollection();

        private readonly char[] separator = new char[] { '\n' };

        DataSourceProxy dsp;

        public CQLRunner(object app)
        {
            InitializeComponent();
            try
            {

                this.corelApp = app as corel.Application;
                stylesController = new StylesController(this.Resources, this.corelApp);
                txt_cql.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
                txt_cql.AutoCompleteCustomSource = CQLSucessedList;
                txt_cql.KeyUp += Txt_cql_KeyUp1;
                this.Loaded += CQLRunner_Loaded;
               

            }
            catch
            {
                MessageBox.Show("VGCore Erro");
            }
        }

        private void CQLRunner_Loaded(object sender, RoutedEventArgs e)
        {
            stylesController.LoadThemeFromPreference();
            dsp = corelApp.FrameWork.Application.DataContext.GetDataSource(ControlUI.DataSourceName);
            object o = dsp.GetProperty("CQLSucessedList");
            string list = o.ToString();
            if (!string.IsNullOrEmpty(list))
            {

                CQLSucessedList.AddRange(list.Split(separator, StringSplitOptions.RemoveEmptyEntries));
            }
            txt_cql.Text = dsp.GetProperty("CQLTempText").ToString();
            context = (int)dsp.GetProperty("CQLContext");
            switch (context)
            {
                case 0:
                    rb_app.IsChecked = true;
                    break;
                case 1:
                    rb_page.IsChecked = true;
                    break;
                case 2:
                    rb_range.IsChecked = true;
                    break;
                case 3:
                    rb_shape.IsChecked = true;
                    break;
            }
            txt_cql.Focus();
        }

        private void Txt_cql_KeyUp1(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //if (e.KeyCode.Equals(System.Windows.Forms.Keys.Escape))
            //    popup_button.IsOpen = false;
            dsp.SetProperty("CQLTempText", txt_cql.Text);
            if (e.KeyCode.Equals(System.Windows.Forms.Keys.Enter))
                RunCQL();
        }

        private void Txt_cql_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.Key.Equals(Key.Escape))
            //    popup_button.IsOpen = false;
            if (e.Key.Equals(Key.Enter))
                RunCQL();
        }

        private int context = 0;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.Clipboard.SetText(GenString());
        }
        private string GenString()
        {
            string a = "";
            for (int i = 0; i < CQLSucessedList.Count; i++)
            {
                a += CQLSucessedList[i];
                if (i < CQLSucessedList.Count - 1)
                    a += "\n";
            }
            return a;
        }
        private void RunCQL()
        {
            string cql = txt_cql.Text;
            object result = null;
            SetConsoleMessage("");
          
            if (string.IsNullOrEmpty(cql))
                return;
            try
            {
                switch (context)
                {

                    case 0:
                        result = this.corelApp.Evaluate(cql);
                        SetConsoleMessage(result);
                        if (result != null)
                            SaveCQL(cql);
                        break;
                    case 1:
                        if (this.corelApp.ActivePage != null)
                        {
                            ShapeRange sr1 = this.corelApp.ActivePage.Shapes.FindShapes(Query: cql);
                            sr1.CreateSelection();
                            if (sr1 != null)
                            {
                                sr1.Sort(cql);
                                if (sr1.Count == 0)
                                    SetConsoleMessage("nothing selected!");
                                else
                                    SetConsoleMessage(string.Format("{0} selected!",sr1.Count));
                                SaveCQL(cql);
                            }
                        }
                        break;
                    case 2:
                        ShapeRange sr = this.corelApp.ActiveSelectionRange;
                        if (sr.Count == 0)
                            return;
                      //  this.corelApp.ActiveWindow.ActiveView.SetViewArea(sr.LeftX, sr.BottomY, sr.SizeWidth , sr.SizeHeight  );
                        if (sr != null)
                        {
                            sr.Sort(cql);
                            SetConsoleMessage("Sucess");
                            SaveCQL(cql);
                            sr.RemoveFromSelection();
                            Thread th = new Thread(new ThreadStart(
                                () =>
                                {
                                    for (int i = 1; i <= sr.Count; i++)
                                    {
                                        sr[i].AddToSelection();
                                        Thread.Sleep(200);
                                        sr[i].RemoveFromSelection();
                                    }
                                    Thread.Sleep(200);
                                    sr.AddToSelection();
                                }
                                ));
                            th.IsBackground = true;
                            th.Start();

                        }
                        break;
                    case 3:
                        if (this.corelApp.ActiveShape != null)
                        {
                            result = this.corelApp.ActiveShape.Evaluate(cql);
                           SetConsoleMessage(result);
                            if (result != null)
                                SaveCQL(cql);
                        }
                        break;
                }


            }
            catch (COMException ex)
            {
               
                SetConsoleMessage(ex.Message,false);
            }
        }

        private void SetConsoleMessage(string msg,bool sucess = true)
        {
            if(sucess)
                lba_console.Foreground = Brushes.Green;
            else
                lba_console.Foreground = Brushes.Red;
            lba_console.Content = msg;
        }
        private void SetConsoleMessage(object obj,bool sucess = true)
        {
            SetConsoleMessage(obj.ToString(), sucess);
        }
        private void SaveCQL(string cql)
        {
            CQLSucessedList.Add(cql);
            dsp.SetProperty("CQLSucessedList", GenString());
            dsp.SetProperty("CQLTempText", "");
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            context = Int32.Parse((sender as RadioButton).Tag.ToString());
            dsp.SetProperty("CQLContext", context);
        }
    }

}
