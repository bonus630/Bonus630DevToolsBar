using br.com.Bonus630DevToolsBar.RecentFiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.Folders
{
    internal class FolderModel : SQLiteManager
    {
        public FolderModel(int corelVersion) : base("Folders", "folders", corelVersion, "folders", "id")
        {
            this.CreateTable(string.Format("CREATE TABLE IF NOT EXISTS {0}({1} INTEGER PRIMARY KEY,folderPath TEXT,imagePath TEXT);", table, primaryKey));
        }

        public int InsertData(Folder folder)
        {
            try
            {
                int id = GetLastId() + 1;
                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = string.Format("INSERT INTO {0}({1},folderPath,imagePath) VALUES (:id,:folderPath,:imagePath);", table, primaryKey);
                    command.Parameters.AddWithValue(":id", id);
                    command.Parameters.AddWithValue(":folderPath", folder.Path);
                    command.Parameters.AddWithValue(":imagePath", folder.GetIcone());
                    return command.ExecuteNonQuery();
                }
            }
            catch { }
            return -1;
        }
      
        public void Fill(ObservableCollection<Folder> datas, Folders parent, int id = -1)
        {
            try
            {
                string condition = string.Format(" ORDER BY {0} ASC", primaryKey);
                if (id != -1)
                    condition = " LIMIT " + id;
                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM {0}{1};", table, condition);

                    using (SQLiteDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Folder f = new Folder(parent);
                            f.Index = dataReader.GetInt32(0);
                            f.Path = dataReader.GetString(1);
                            f.SetIcone(dataReader.GetString(2));
                            datas.Add(f);

                        }
                    }
                }

            }
            catch { }

        }

        public void UpdateFile(Folder folder)
        {
            try
            {
                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = string.Format("UPDATE {0} SET folderPath=:folderPath,imagePath=:imagePath WHERE {1}=:id", table, primaryKey);
                    command.Parameters.AddWithValue(":id", folder.Index);
                    command.Parameters.AddWithValue(":folderPath", folder.Path);
                    command.Parameters.AddWithValue(":imagePath", folder.GetIcone());

                    command.ExecuteNonQuery();
                }
            }
            catch { }
        }
    }

}
