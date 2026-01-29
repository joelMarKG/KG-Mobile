using KG.Mobile.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using KG_Data_Access;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace KG.Mobile.Helpers
{
    //SQLite Implementation Class
    public sealed class MobileDatabase
    {
        private GraphQLApiServices _graphQLApiServices = new GraphQLApiServices();
        private readonly string DeviceId;
        private readonly string OS;

        private MobileDatabase()
        {
            // Get or create a unique device ID
            DeviceId = Preferences.Get("DeviceUniqueId", string.Empty);
            if (string.IsNullOrEmpty(DeviceId))
            {
                DeviceId = Guid.NewGuid().ToString();
                Preferences.Set("DeviceUniqueId", DeviceId);
            }

            OS = DeviceInfo.VersionString;

            setupDatabase();
        }

        #region Singleton(Sealed) Class Hanlders
        //can only have a single instance of the class read/writing to/from the file based database
        private static readonly Lazy<MobileDatabase> lazy = new Lazy<MobileDatabase>(() => new MobileDatabase());
        public static MobileDatabase Instance { get { return lazy.Value; } }
        #endregion

        //SQLite db connection
        public SQLiteAsyncConnection db;

        public async Task setupDatabase()
        {
            // Get an absolute path to the database file
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SBMOM.Mobile.Database.db");
            db = new SQLiteAsyncConnection(databasePath);

            //create tables
            await LogTableCreate();
        }

        #region LogTable
        //create Log table if it doesn't already exist (note: an uninstall will delete the database)
        public async Task LogTableCreate()
        {            
            string theQ = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'Log'";
            var result = await db.QueryAsync<Log>(theQ);
            if (result == null || result.Count == 0)
            {
                await db.CreateTableAsync<Log>();
            }
        }

        //insert a row into the log
        public async Task LogAdd(DateTime dateTime, string type, String component, String comment)
        {
            //awaits removed to not hold up the main thread during execution
            db.ExecuteAsync("INSERT INTO Log (dateTime, type, component, comment, sentToServer) VALUES (?,?,?,?,?)", dateTime, type, component, comment, false);
            //LogSendToServer();
        }

        //Load Top 100 Log Entries in Descending Order
        public async Task<IEnumerable<Log>> LogGetTop1000()
        {
            return await db.Table<Log>().OrderByDescending(x => x.Id).Take(1000).ToListAsync();
        }

        //clear the log table
        public async Task LogClear()
        {
            await db.ExecuteAsync("DELETE FROM Log");
        }

        //public async Task LogSendToServer()
        //{
        //    //query those log entries not sent to server already
        //    IEnumerable<Log> log = await db.Table<Log>().Where(x => x.sentToServer == false).ToListAsync();

        //    //mapp each to the MobileLog Model
        //    foreach (var l in log)
        //    {
        //        var m = new mobileLog();
        //        m.source = IMEI;
        //        m.sourceName = Settings.DeviceName;
        //        m.version = float.Parse(DependencyService.Get<IAppVersion>().Version());
        //        m.OS = OS;
        //        m.dateTime = l.dateTime;
        //        m.type = l.type;
        //        m.component = l.component;
        //        m.comment = l.comment;

        //        //send via body
        //        var response = await _graphQLApiServices.WebAPICallAsyncRestBody(RestSharp.Method.POST, $"/api/DiagnosticLog/post", m);


        //        //check for a response and a successful response before marking as sent
        //        if (response?.GetType() == typeof(RestSharp.RestResponse))
        //        {
        //            RestResponseBase responseCast = (RestResponseBase)response;
        //            if (responseCast.IsSuccessful)
        //            {
        //                //set the row as sent to the server
        //                await db.ExecuteAsync("UPDATE Log SET sentToServer = 1 WHERE id = " + l.Id);
        //            }
        //        }
        //    }
        //}

        //table construction
        public class Log
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            [Indexed]
            public DateTime dateTime { get; set; }
            public string type { get; set; }
            public string component { get; set; }
            public string comment { get; set; }
            public bool sentToServer { get; set; }
        }

        #endregion
    }
}
