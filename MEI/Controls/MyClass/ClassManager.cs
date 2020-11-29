using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace MEI.Controls.MyClass
{
    public class ClassObj
    {
        public long ID { get; set; }
        public long Order { get; set; }
        public string DisTxt { get; set; }
        public string Link { get; set; }
    }

    public class ClassManager
    {
        private readonly string DB_FILE = "classroom.db";
        public async Task InitDB()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(DB_FILE, CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                await db.OpenAsync();
                string tableCommand = "CREATE TABLE IF NOT EXISTS Classroom " +
                    "(ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                    "OrderDis INTEGER NOT NULL," +
                    "DisTxt Text NOT NULL," +
                    "Link Text NOT NULL);";
                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                await createTable.ExecuteNonQueryAsync();
            }
        }

        public async void AddData(ClassObj obj)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                await db.OpenAsync();
                string command = "INSERT INTO Classroom (OrderDis, DisTxt, Link) VALUES (@Order, @DisTxt, @Link);";

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Parameters.AddWithValue("@Order", obj.Order);
                insertCommand.Parameters.AddWithValue("@DisTxt", obj.DisTxt);
                insertCommand.Parameters.AddWithValue("@Link", obj.Link);

                insertCommand.Connection = db;
                insertCommand.CommandText = command;
                await insertCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task RemoveData(long id)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                await db.OpenAsync();
                SqliteCommand delCommand = new SqliteCommand("DELETE FROM Classroom WHERE ID=@DID", db);
                delCommand.Parameters.AddWithValue("@DID", id);

                await delCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<ClassObj>> ReadData()
        {
            List<ClassObj> entries = new List<ClassObj>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT * FROM Classroom", db);
                SqliteDataReader query = await selectCommand.ExecuteReaderAsync();

                while (await query.ReadAsync())
                {
                    ClassObj Lobj = new ClassObj()
                    {
                        ID = (long)query["ID"],
                        Order = (long)query["OrderDis"],
                        DisTxt = query["DisTxt"].ToString(),
                        Link = query["Link"].ToString()
                    };
                    entries.Add(Lobj);
                }
            }
            return entries;
        }

        public async Task UpContent(long currentOrder)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                await db.OpenAsync();
                SqliteTransaction transaction = db.BeginTransaction();

                SqliteCommand cmd = new SqliteCommand(
                    "UPDATE Classroom SET OrderDis = -1 WHERE OrderDis=@CurrentOrder-1;" +
                    "UPDATE Classroom SET OrderDis = OrderDis-1 WHERE OrderDis=@CurrentOrder;" +
                    "UPDATE Classroom SET OrderDis = @CurrentOrder WHERE OrderDis=-1;",
                    db, transaction);
                cmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);
                await cmd.ExecuteNonQueryAsync();

                transaction.Commit();
            }
        }

        public async Task DownContent(long currentOrder)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                await db.OpenAsync();
                SqliteTransaction transaction = db.BeginTransaction();

                SqliteCommand cmd = new SqliteCommand(
                    "UPDATE Classroom SET OrderDis = -1 WHERE OrderDis=@CurrentOrder+1;" +
                    "UPDATE Classroom SET OrderDis = OrderDis+1 WHERE OrderDis=@CurrentOrder;" +
                    "UPDATE Classroom SET OrderDis = @CurrentOrder WHERE OrderDis=-1;",
                    db, transaction);
                cmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);
                await cmd.ExecuteNonQueryAsync();

                transaction.Commit();
            }
        }

        public async Task<long> GetNumberOfClasses()
        {
            long ret = -1;
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                await db.OpenAsync();
                SqliteCommand cmd = new SqliteCommand("SELECT count(*) FROM Classroom;", db);
                SqliteDataReader reader =  await cmd.ExecuteReaderAsync();
                while(await reader.ReadAsync())
                {
                    ret = long.Parse(reader["count(*)"].ToString());
                }
            }
            return ret;
        }
    }
}
