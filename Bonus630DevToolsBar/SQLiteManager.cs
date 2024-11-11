using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace br.com.Bonus630DevToolsBar
{
    public abstract class SQLiteManager
    {
        private string DbFilePath;
        public SQLiteConnection sqliteConnection;
        protected static string component;
        protected readonly string table;
        protected readonly string primaryKey;

        public SQLiteManager(string component,string dbFileName,int corelVersion,string table, string primaryKey)
        {
            this.table = table;
            this.primaryKey = primaryKey;
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
                    command.CommandText = string.Format("SELECT * FROM {0} ORDER BY {1} DESC LIMIT 1;",table,primaryKey);
                  
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldValue"></param>
        /// <returns>Returns -1 if not exists</returns>
        public int CheckExits(string field,string fieldValue)
        {
            try
            {
                int id = -1;

                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM {0} WHERE {1}=:path ORDER BY {2} DESC LIMIT 1;", table, field,primaryKey);
                    command.Parameters.AddWithValue(":path", fieldValue);
                 
                    using (SQLiteDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                            id = dataReader.GetInt32(0);
                        if (id > -1)
                        {
                            return id;
                        }

                    }
                }

            }
            catch { }
            return -1;
        }
        
        public void DeleteFileDataFromDB(int id)
        {
            try
            {
                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM {0} WHERE {1}=:id",table,primaryKey);
                    command.Parameters.AddWithValue(":id", id);
                    command.ExecuteNonQuery();
                }
            }
            catch { }
        }
    }
}
