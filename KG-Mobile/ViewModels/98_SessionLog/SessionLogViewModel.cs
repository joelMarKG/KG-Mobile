using System.Collections.Generic;
using System.ComponentModel;
using KG_Data_Access;
using System.Runtime.CompilerServices;
using KG.Mobile.Helpers;
using static KG.Mobile.Helpers.MobileDatabase;
using System.Threading.Tasks;
using System.Windows.Input;
using KG.Mobile.Models;

namespace KG.Mobile.ViewModels._98_SessionLog
{
    class SessionLogViewModel : INotifyPropertyChanged
    {
        //helpers
        private MobileDatabase database = MobileDatabase.Instance;

        #region DataGrid Handling
        //DataGrid Tags
        public IEnumerable<Log> _log;
        public IEnumerable<Log> log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("log"));
            }
        }

        public Log _logSelected;
        public Log logSelected
        {
            get
            {
                return _logSelected;
            }
            set
            {
                _logSelected = value;

                //open popup with detail
                PopupMessage msg = new PopupMessage("Log Detail","Log Detail",value.comment,"Close");
                MessagingCenter.Send(msg, "PopupMessageNoLog");

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("logSelected"));
            }
        }
        #endregion

        #region WebAPI Calls and Commands
        public SessionLogViewModel()
        {
            loadLog();
        }

        public async Task loadLog()
        {
            //Load Top 100 Log Entries in Descending Order
            log = await database.LogGetTop1000();
        }

        //Call Move Inventory
        public ICommand ResetLogCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await database.LogClear();
                    await loadLog();                    
                });
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
