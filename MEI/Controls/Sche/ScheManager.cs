using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace MEI.Controls.Sche
{
    public class ScheData
    {
        public long ID { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Content { get; set; }
        public bool Complete { get; set; }
    }

    public class ScheManager
    {
        private readonly string DB_FILE = "schedules.db";
        public async Task InitDB()
        {
            await ExecuteCmd("CREATE TABLE IF NOT EXISTS sche " +
                        "(ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                        "Date TEXT NOT NULL," +
                        "Content TEXT NOT NULL," +
                        "Complete INTEGER NOT NULL);");
        }

        public async Task ExecuteCmd(string cmd)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                await db.OpenAsync();
                SqliteCommand createTable = new SqliteCommand(cmd, db);
                await createTable.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<ScheData>> GetSche(string date)
        {
            List<ScheData> entries = new List<ScheData>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT * FROM sche WHERE Date=@targetD", db);
                selectCommand.Parameters.AddWithValue("@targetD", date);
                SqliteDataReader query = await selectCommand.ExecuteReaderAsync();

                while (await query.ReadAsync())
                {
                    Debug.WriteLine(query["Date"].ToString());
                    ScheData Lobj = new ScheData()
                    {
                        ID = (long)query["ID"],
                        Content = query["Content"].ToString(),
                        Complete = (long)query["Complete"] == 1,
                        Time = DateTimeOffset.ParseExact(query["Date"].ToString(), "yyyy/MM/dd", new CultureInfo("ko-KR"))
                    };
                    entries.Add(Lobj);
                }
            }
            return entries;
        }

        public async Task Insert(ScheData data)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                await db.OpenAsync();
                SqliteCommand insert = new SqliteCommand(
                    "INSERT INTO sche (Content, Complete, Date) VALUES (@Cont, @Comp, @Date)", db);
                insert.Parameters.AddWithValue("@Cont", data.Content);
                insert.Parameters.AddWithValue("@Comp", (data.Complete ? 1 : 0));
                insert.Parameters.AddWithValue("@Date", data.Time.ToString("yyyy/MM/dd"));
                await insert.ExecuteNonQueryAsync();
            }
        }

        public async Task ChangeComplete(long id)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                await db.OpenAsync();
                SqliteCommand cmd = new SqliteCommand(
                    "UPDATE sche SET Complete=CASE WHEN Complete=0 THEN 1 ELSE 0 END WHERE ID=@id;", db);
                cmd.Parameters.AddWithValue("@id", id);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}