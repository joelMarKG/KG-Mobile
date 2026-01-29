using KG.Mobile.Helpers;
using KG.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace KG.Mobile.ViewModels._99_Settings
{
	public class SettingsViewModel : ContentView
	{
        private readonly SoundHelper _soundHelper;

        #region Constructor
        public SettingsViewModel(SoundHelper soundHelper)
        {
            _soundHelper = soundHelper;
        }
        #endregion

        #region XAML Bound Tags

        //DeviceName
        public string DeviceName
        {
            get { return Settings.DeviceName; }
            set
            {
                Settings.DeviceName = value;
            }
        }

        //WebApiTimeoutSeconds
        public int WebApiTimeoutSeconds
        {
            get { return Settings.GraphQLApiTimeoutSeconds; }
            set
            {
                Settings.GraphQLApiTimeoutSeconds = value;
            }
        }

        //DisablePopupKeyboard
        public bool DisablePopupKeyboard
        {
            get { return Settings.DisablePopupKeyboard; }
            set
            {
                Settings.DisablePopupKeyboard = value;
            }
        }

        //CheckBinsforInventoryBeforeMoving
        public bool CheckBinsforInventoryBeforeMoving
        {
            get { return Settings.CheckBinsforInventoryBeforeMoving; }
            set
            {
                Settings.CheckBinsforInventoryBeforeMoving = value;
            }
        }

        //InventoryAutoMoveOnByDefault
        public bool InventoryAutoMoveOnByDefault
        {
            get { return Settings.InventoryAutoMoveOnByDefault; }
            set
            {
                Settings.InventoryAutoMoveOnByDefault = value;
            }
        }


        //LocationAutoMoveOnByDefault
        public bool LocationAutoMoveOnByDefault
        {
            get { return Settings.LocationAutoMoveOnByDefault; }
            set
            {
                Settings.LocationAutoMoveOnByDefault = value;
            }
        }

        //JobTakeOutAuto
        public bool JobTakeOutAutoOnByDefault
        {
            get { return Settings.JobTakeOutAutoOnByDefault; }
            set
            {
                Settings.JobTakeOutAutoOnByDefault = value;
            }
        }

        //QualityAutoMove
        public bool QualityMoveAutoOnByDefault
        {
            get { return Settings.QualityMoveAutoOnByDefault; }
            set
            {
                Settings.QualityMoveAutoOnByDefault = value;
            }
        }

        //QualityAutoMoveToEnt
        public string QualityMoveToEntDefault
        {
            get { return Settings.QualityMoveToEntDefault; }
            set
            {
                Settings.QualityMoveToEntDefault = value;
            }
        }

        //AutoSelectEntryField
        public bool AutoSelectEntryField
        {
            get { return Settings.AutoSelectEntryField; }
            set
            {
                Settings.AutoSelectEntryField = value;
            }
        }

        //Test Audio Button
        public ICommand TestAudioCommand
        {
            get
            {
                return new Command(async () =>
                {
                    //Play the Test sound
                    _soundHelper.PlayTest();
                });
            }

        }

        //Location Move Name
        public string LocationMoveName
        {
            get { return Settings.LocationMoveName; }
            set
            {
                Settings.LocationMoveName = value;
            }
        }
        

        //Job Takeout Wo Attr Name
        public string JobTakeoutWoAttrName
        {
            get { return Settings.JobTakeoutWoAttrName; }
            set
            {
                Settings.JobTakeoutWoAttrName = value;
            }
        }

        #endregion
    }
}