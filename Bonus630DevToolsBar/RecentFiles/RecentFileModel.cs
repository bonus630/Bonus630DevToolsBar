using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace br.com.Bonus630DevToolsBar.RecentFiles
{
    public class RecentFileModel : SQLiteManager
    {
        private readonly string thumbEntry1 = "previews/thumbnail.png";
        private readonly string thumbEntry2 = "metadata/thumbnails/thumbnail.bmp";
        //private readonly string table = "files";
       // private readonly string primaryKey = "id";


        public RecentFileModel(int corelVersion):base("RecentFiles", "recent-files",corelVersion,"files","id")

        {
            
            this.CreateTable("CREATE TABLE IF NOT EXISTS files(id INTEGER PRIMARY KEY,_index INTEGER,count INTEGER,autoload INTEGER,name Varchar(50),path Varchar(256),time INTEGER);");
        }

        // int openTimes, long openedTime,
        public RecentFileViewModel InsertData(int index, string name, string fullName, bool autoload = false, int openTimes = 1, long openedTime = 0)
        {
            try
            {
                int id = GetLastId() + 1;
                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = "INSERT INTO files(id,_index,count,autoload,name,path,time) " +
                        "VALUES (:id,:index,:count,:autoload,:name,:path,:time);";
                    command.Parameters.AddWithValue(":id", id);
                    command.Parameters.AddWithValue(":name", name);
                    command.Parameters.AddWithValue(":index", index);
                    command.Parameters.AddWithValue(":count", openTimes);
                    command.Parameters.AddWithValue(":path", fullName);
                    command.Parameters.AddWithValue(":time", openedTime);
                    command.Parameters.AddWithValue(":autoload", autoload ? 1 : 0);

                    // command.Parameters.AddWithValue(":lastDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));

                    command.ExecuteNonQuery();
                }
                return new RecentFileViewModel(id) { Name = name, Index = index, FullName = fullName, OpenTimes = openTimes, OpenDate = DateTime.Now };
            }
            catch { }
            return null;
        }
        //SELECT * FROM sua_tabela ORDER BY datetime(seu_campo_de_tempo_texto) DESC
        public ObservableCollection<RecentFileViewModel> Fill(int id = -1)
        {
            //   0  |    1    |    2    |      3     |       4        |        5        |    6
            //id int,index int,count int,autoload int,name Varchar(50),path Varchar(256),time Integer
            ObservableCollection<RecentFileViewModel> datas = new ObservableCollection<RecentFileViewModel>();
            try
            {
                string condition = " ORDER BY time DESC";
                if (id != -1)
                    condition = " LIMIT " + id;
                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM files ORDER BY time DESC{0};", condition);

                    using (SQLiteDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {

                            RecentFileViewModel k = new RecentFileViewModel(dataReader.GetInt32(0));
                            k.Index = dataReader.GetInt32(1);
                            k.OpenTimes = dataReader.GetInt32(2);
                            k.AutoLoad = dataReader.GetBoolean(3);
                            k.Name = dataReader.GetString(4);
                            k.FullName = dataReader.GetString(5);
                            k.OpenedTime = dataReader.GetInt64(6);
                            datas.Add(k);
                        }
                    }
                }

            }
            catch { }
            return datas;
        }

       // bool autoload = false, int openTimes = 1, long openedTime = 0
        public void UpdateFile(int id, int index,string name,string fullName, int openTimes =1, long openedTime=0, bool autoload=false)
        {
            try
            {
                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = "UPDATE files SET _index=:index,count=:count,time=:time,autoload=:autoload,name=:name,path=:path WHERE id=:id";

                    command.Parameters.AddWithValue(":count", openTimes);
                    command.Parameters.AddWithValue(":index", index);
                    command.Parameters.AddWithValue(":time", openedTime);
                    command.Parameters.AddWithValue(":id", id);
                    command.Parameters.AddWithValue(":path", fullName);
                    command.Parameters.AddWithValue(":name", name);

                    //command.Parameters.AddWithValue(":id", id);
                    command.Parameters.AddWithValue(":autoload", autoload ? 1 : 0);
                    command.ExecuteNonQuery();
                }
            }
            catch { }
        }

      
        //public void InsertOrIncrementCount(string name, string path, int index, long time)
        //{

        //    try
        //    {
        //        int id = -1;
        //        int count = 1;
        //        using (SQLiteCommand command = CreateConnection().CreateCommand())
        //        {
        //            command.CommandText = "SELECT id,count,time FROM files ORDER BY path WHERE path=:path DESC LIMIT 1;";
        //            command.Parameters.AddWithValue(":path", path);

        //            using (SQLiteDataReader dataReader = command.ExecuteReader())
        //            {
        //                if (dataReader.Read())
        //                    id = dataReader.GetInt32(0);
        //                if (id > -1)
        //                {
        //                    count = dataReader.GetInt32(1) + 1;
        //                    time = dataReader.GetInt64(2) + time;
        //                    command.CommandText = "UPDATE files SET index=:index,count=:count,time=:time WHERE id=:id";

        //                }
        //                else
        //                {
        //                    id = GetLastId("files") + 1;
        //                    command.Parameters.AddWithValue(":count", count);
        //                    command.Parameters.AddWithValue(":index", index);
        //                }
        //                command.Parameters.AddWithValue(":time", time);
        //                command.Parameters.AddWithValue(":id", id);
        //                command.Parameters.AddWithValue(":name", name);


        //            }
        //        }

        //    }
        //    catch { }

        // }

  



        public BitmapSource GetThumb(string fullName)
        {
            BitmapSource preview = null;
            Bitmap b = null;
            using (FileStream fs = new FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (ZipArchive zipFile = new ZipArchive(fs,ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zipFile.Entries)
                    {
                        if (entry.FullName.Equals(thumbEntry1, StringComparison.OrdinalIgnoreCase))
                        {
                            b = GetBitmapFromEntry(thumbEntry1, zipFile);
                            break;
                        }
                        if (entry.FullName.Equals(thumbEntry2, StringComparison.OrdinalIgnoreCase))
                        {
                            b = GetBitmapFromEntry(thumbEntry2, zipFile);
                            break;
                        }
                    }
                    if (b != null)
                        preview = Imaging.CreateBitmapSourceFromHBitmap(b.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    b.Dispose();
                }
            }
            return preview;
        }
        private Bitmap GetBitmapFromEntry(string entry, ZipArchive zipFile)
        {
            try
            {
                ZipArchiveEntry thumbEntry = zipFile.GetEntry(entry);
                Bitmap bitmap;
                using (Stream thumbStream = thumbEntry.Open())
                {
                    bitmap = new Bitmap(thumbStream);

                }
                return bitmap;
            }
            catch { return null; }
        }

      
    }
}
