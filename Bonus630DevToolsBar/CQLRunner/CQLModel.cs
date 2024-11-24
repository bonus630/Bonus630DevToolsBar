using br.com.Bonus630DevToolsBar.Folders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.CQLRunner
{
    internal class CQLModel : SQLiteManager
    {
        public CQLModel(int corelVersion) : base("CQLRunner", "CQLRunner", corelVersion, "cqlTable", "id")
        {
            this.CreateTable(string.Format("CREATE TABLE IF NOT EXISTS {0}({1} INTEGER PRIMARY KEY,cql TEXT);", table, primaryKey));
        }

        public int InsertData(string cql)
        {
            try
            {
                int id = GetLastId() + 1;
                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = string.Format("INSERT INTO {0}({1},cql) VALUES (:id,:cql);", table, primaryKey);
                    command.Parameters.AddWithValue(":id", id);
                    command.Parameters.AddWithValue(":cql", cql);
                    return command.ExecuteNonQuery();
                }
            }
            catch { }
            return -1;
        }

        public string[] Fill(string search, int id = 10)
        {
            try
            {
                var temp = new List<string>();
                string condition = string.Format(" ORDER BY {0} ASC", primaryKey);
                if (id != -1)
                    condition = string.Format(" WHERE cql like \'{0}%\'  LIMIT {1};",search.Replace("%", "\\%").Replace("_", "\\_"), id);
                using (SQLiteCommand command = CreateConnection().CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM {0}{1};", table, condition);

                    using (SQLiteDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                      
                            temp.Add(dataReader.GetString(1));

                        }
                    }
                }
               return temp.ToArray();
            }
            catch { }
            return null;
        }
    }
}