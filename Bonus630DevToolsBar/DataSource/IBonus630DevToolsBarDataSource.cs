using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DataSource
{
    [ComVisible(true)]
    [Guid("e441ec18-dfd3-4c7e-86c2-61c3cf3272cb")]
    public interface IBonus630DevToolsBarDataSource
    {
        string Caption { get; set; }
        string CqlCaption { get; set; }
        string CqlTooltip { get; set; }
        string FoldersCaption { get; set; }
        string FoldersTooltip { get; set; }
        string CQLSucessedList { get; set; }
        string CQLTempText { get; set; }
        int CQLContext { get; set; }
        bool ShortcutDockerFirstUse { get; set; }
        string ReOpenDocumentCaption { get; set; }
        string XmlItems { get; set; }

        void RunCommandDocker();
        void RunShortcutsDocker();
        void RunDrawUIExplorer();
        void RunIconCreatorHelper();
        void UnloadNDeleteUserGMS();
        void ReloadNRestoreUserGMS();
        void OpenCQLGuide();
        void RunCommandBarBuilder();
        void LoadIcon();
        void PrintScreen();
        void CallFolders();
        void CallCQL();
        void CallGMSReloader();
        void ReOpenDocument();
    }

}
