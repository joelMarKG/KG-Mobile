using KG.Mobile.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using static KG.Mobile.Helpers.MobileDatabase;

namespace KG.Mobile.ViewModels._98_SessionLog
{
    class SessionLogViewModel : INotifyPropertyChanged
    {
        private readonly MobileDatabase database = MobileDatabase.Instance;

        public ObservableCollection<Log> log { get; } = new();

        public SessionLogViewModel()
        {
        }

        public async Task LoadLog()
        {
            var logs = await database.LogGetTop200();
            log.Clear();

            foreach (var l in logs)
                log.Add(l);
        }

        #region WebAPI Calls and Commands
        public async Task InitializeAsync()
        {
            await LoadLog();
        }

        //Call Move Inventory
        public ICommand ResetLogCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await database.LogClear();
                    await LoadLog();                    
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
