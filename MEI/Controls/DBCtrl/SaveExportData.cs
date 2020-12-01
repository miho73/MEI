using MEI.Controls.MyClass;
using MEI.Controls.Sche;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MEI.Controls
{
    public class SaveExportData
    {
        public async Task ExportData(StorageFile path)
        {
            JObject root = new JObject();
            try
            {
                root = await ExportClassData(root);
                root = await ExportScheData(root);
                string code = root.ToString(Formatting.None);
                await FileIO.WriteTextAsync(path, code);
            }
            catch(Exception e)
            {
                await path.DeleteAsync();
                Debug.WriteLine(e.StackTrace);
                throw e;
            }
        }

        private async Task<JObject> ExportClassData(JObject root)
        {
            ClassManager cManager = new ClassManager();
            List<ClassObj> read = await cManager.ReadData();
            JArray cl = new JArray();
            foreach (ClassObj o in read)
            {
                JObject obj = new JObject();
                obj.Add("ID", o.ID);
                obj.Add("Order", o.Order);
                obj.Add("Link", o.Link);
                obj.Add("Txt", o.DisTxt);
                cl.Add(obj);
            }
            root.Add("Classroom", cl);
            return root;
        }

        private async Task<JObject> ExportScheData(JObject root)
        {
            ScheManager sManager = new ScheManager();
            List<ScheData> read = await sManager.ReadData();
            JArray cl = new JArray();
            foreach (ScheData o in read)
            {
                JObject obj = new JObject();
                obj.Add("ID", o.ID);
                obj.Add("Cont", o.Content);
                obj.Add("Date", o.Time.ToString("yyyy/MM/dd"));
                obj.Add("Txt", o.Complete);
                cl.Add(obj);
            }
            root.Add("Schedule", cl);
            return root;
        }

        public async Task LoadData(StorageFile from)
        {
            JObject root = JObject.Parse(await FileIO.ReadTextAsync(from));
            JArray classes = (JArray)root.GetValue("Classroom");
            JArray sches = (JArray)root.GetValue("Schedule");
            ClassManager cM = new ClassManager();
            ScheManager sM = new ScheManager();
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, ClassManager.DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                await db.OpenAsync();
                SqliteTransaction transC = db.BeginTransaction();
                try
                {
                    await cM.Clear(db, transC);
                    foreach (JObject o in classes)
                    {
                        await cM.AddData(new ClassObj()
                        {
                            Order = (long)o.GetValue("Order"),
                            Link = o.GetValue("Link").ToString(),
                            DisTxt = o.GetValue("Txt").ToString()
                        }, db, transC);
                    }
                }
                catch(Exception e)
                {
                    transC.Rollback();
                    throw e;
                }
                transC.Commit();
            }

            dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, ScheManager.DB_FILE);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                await db.OpenAsync();
                SqliteTransaction transS = db.BeginTransaction();
                try
                {
                    await sM.Clear(db, transS);
                    foreach (JObject o in sches)
                    {
                        await sM.Insert(new ScheData()
                        {
                            Content = o.GetValue("Cont").ToString(),
                            Complete = (long)o.GetValue("Txt") == 1 ? true : false,
                            Time = DateTimeOffset.ParseExact(o.GetValue("Date").ToString(), "yyyy/MM/dd", new CultureInfo("ko-KR"))
                        }, db, transS);
                    }
                }
                catch(Exception e)
                {
                    transS.Rollback();
                    throw e;
                }
                transS.Commit();
            }
        }
    }
}
