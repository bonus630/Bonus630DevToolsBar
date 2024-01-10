using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace br.com.Bonus630DevToolsBar.RecentFiles
{
    public abstract class SQLiteManager
    {
        private string DbFilePath;
        public SQLiteConnection sqliteConnection;
        protected static string component;
        protected string table;

        public SQLiteManager(string component,string dbFileName,int corelVersion)
        {
            SQLiteManager.component = component;
            this.checkFile(corelVersion,dbFileName);
        }

        private static string GetDBFDirPath()
        {
            string path = Path.Combine(
                 Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"bonus630", SQLiteManager.component);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        protected SQLiteConnection CreateConnection()
        {
            try
            {
                sqliteConnection = new SQLiteConnection(string.Format("Data Source={0}; Version=3;", DbFilePath));
                sqliteConnection.Open();
                return sqliteConnection;
            }
            catch (SQLiteException erro)
            {
                throw erro;
            }
        }
        private void checkFile(int corelVersion,string dbFileName)
        {
            try
            {
                string path = GetDBFDirPath();
                DbFilePath = string.Format("{0}\\{1}-{2}.sqlite", path,dbFileName, corelVersion);
                CreateDbFile();
               // CreateTable();


            }
            catch (IOException erro)
            {
                throw erro;
            }
        }
        private void CreateDbFile()
        {
            try
            {
                if (!File.Exists(DbFilePath))
                    SQLiteConnection.CreateFile(DbFilePath);
            }
            catch (SQLiteException erro)
            {
                throw erro;
            }
        }
        protected void CreateTable(string sql)
        {
            try
            {
                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
            catch { }
        }
        protected int GetLastId()
        {
            int result = -1;
            try
            {

                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM files ORDER BY id DESC LIMIT 1;";
                  
                    using (SQLiteDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                            result = dataReader.GetInt32(0);
                    }
                }

            }
            catch { }
            return result;
        }
        public bool CheckExits(string fullName)
        {
            try
            {
                int id = -1;

                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = "SELECT id FROM files ORDER BY id WHERE path=:path DESC LIMIT 1;";
                    command.Parameters.AddWithValue(":path", fullName);
                 
                    using (SQLiteDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                            id = dataReader.GetInt32(0);
                        if (id > -1)
                        {
                            return true;
                        }

                    }
                }

            }
            catch { }
            return false;
        }
        public void DeleteFile(int id)
        {
            try
            {
                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = "DELETE FROM files WHERE id=:id";
                    command.Parameters.AddWithValue(":id", id);
                    command.ExecuteNonQuery();
                }
            }
            catch { }
        }
    }
}
