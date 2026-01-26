using SBMOM.Mobile.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using SBMOM_Data_Access;
using SBMOM.Mobile.Models;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using RestSharp;
using System.Web;
using SBMOM.Mobile.Helpers;
using SBMOM.Mobile.CustomControls;

namespace SBMOM.Mobile.ViewModels._01_Inventory
{
    class LocationMoveViewModel : INotifyPropertyChanged
    {
        private WebApiServices _webApiServices = new WebApiServices();
        private WebApiServicesHelper _webApiServicesHelper = new WebApiServicesHelper();
        private SoundHelper _soundHelper = new SoundHelper();
        private MobileDatabase database = MobileDatabase.Instance;

        //current item inventory and related data
        private ent ent { get; set; }
        private ent moveToEnt { get; set; }

        #region Constructor
        public LocationMoveViewModel()
        {
            //set the default state for the toggle switch in the UI
            AutoMoveEnabled = Settings.LocationAutoMoveOnByDefault;
        }
        #endregion

        #region XAML Bound Tags

        //Result Tags
        private string _Result;
        public string Result
        {
            get
            {
                return _Result;
            }
            set
            {
                _Result = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Result"));
                if (value != "")
                {
                    if(ResultError)
                    {
                        ResultErrorVisible = true;
                        ResultMessageVisible = false;

                        //save to log
                        database.LogAdd(DateTime.Now, "Error", Settings.LocationMoveName + " Move", value);
                    }
                    else
                    {
                        ResultErrorVisible = false;
                        ResultMessageVisible = true;

                        //save to log
                        database.LogAdd(DateTime.Now, "Info", Settings.LocationMoveName + " Move", value);
                    }
                    
                }
                else
                {
                    ResultErrorVisible = false;
                    ResultMessageVisible = false;
                }
            }
        }

        //ResultError Tag
        private bool ResultError;

        //ResultVisible Tags
        private bool _ResultErrorVisible;
        public bool ResultErrorVisible
        {
            get
            {
                return _ResultErrorVisible;
            }
            set
            {
                if (value)
                {
                    //Play the Error sound
                    _soundHelper.playError();
                }

                _ResultErrorVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResultErrorVisible"));              
            }
        }
        private bool _ResultMessageVisible;
        public bool ResultMessageVisible
        {
            get
            {
                return _ResultMessageVisible;
            }
            set
            {
                if (value)
                {
                    //Play the Success sound
                    _soundHelper.playSuccess();
                }

                _ResultMessageVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResultMessageVisible"));
            }
        }    

        //LocationName Tags
        private string _LocationName;
        public string LocationName
        {
            get
            {
                return _LocationName;
            }
            set
            {
                _LocationName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LocationName"));
            }
        }

        //LocationDescription Tags
        private string _LocationDescription;
        public string LocationDescription
        {
            get
            {
                return _LocationDescription;
            }
            set
            {
                _LocationDescription = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LocationDescription"));
            }
        }

        //CurrentLocation Tags
        private string _CurrentLocation;
        public string CurrentLocation
        {
            get
            {
                return _CurrentLocation;
            }
            set
            {
                _CurrentLocation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentLocation"));
            }
        }

        //CurrentLocationDescription Tags
        private string _CurrentLocationDescription;
        public string CurrentLocationDescription
        {
            get
            {
                return _CurrentLocationDescription;
            }
            set
            {
                _CurrentLocationDescription = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentLocationDescription"));
            }
        }

        //AutoMoveEnableToggle Tags
        private bool _AutoMoveEnabled;
        public bool AutoMoveEnabled
        {
            get
            {
                return _AutoMoveEnabled;
            }
            set
            {
                _AutoMoveEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AutoMoveEnabled"));
            }
        }

        //MoveToLocationName Tags
        private string _MoveToLocationName;
        public string MoveToLocationName
        {
            get
            {
                return _MoveToLocationName;
            }
            set
            {
                _MoveToLocationName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MoveToLocationName"));
            }
        }

        //LocationDescription Tags
        private string _MoveToLocationDescription;
        public string MoveToLocationDescription
        {
            get
            {
                return _MoveToLocationDescription;
            }
            set
            {
                _MoveToLocationDescription = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MoveToLocationDescription"));
            }
        }

        //LocationToMoveHeading Tags
        public string LocationToMoveHeading
        {
            get
            {
                return Settings.LocationMoveName + " to Move";
            }
        }

        //MoveLocationToHeading Tags
        public string MoveLocationToHeading
        {
            get
            {
                return "Move the above " + Settings.LocationMoveName + " to";
            }
        }
        #endregion

        #region WebAPI Calls and Commands

        //Command for Location Barcode Enter key
        public ICommand ExecuteLocationBarcode
        {
            get
            {
                return new Command(async () =>
                {
                    //reset values
                    ent = null;
                    LocationDescription = "";

                    if (!String.IsNullOrEmpty(LocationName))
                    {
                        //Show Busy
                        BusyMessage msg = new BusyMessage(true, "Find " + Settings.LocationMoveName);
                        MessagingCenter.Send(msg, "BusyPopup");

                        ent = await _webApiServicesHelper.EntityGetByEntName(LocationName);

                        //entity found?
                        if (ent != null)
                        {
                            //is it a moveable location?
                            List<storage_exec> se;
                            se = await _webApiServicesHelper.StorageExecByEntId(ent.ent_id);

                            if (se != null && se.Count > 0)
                            {
                                if (se[0].movable)
                                {
                                    LocationDescription = this.ent.description;

                                    //get the current location
                                    if (se[0].storage_ent_id == null)
                                    {
                                        CurrentLocation = "";
                                        CurrentLocationDescription = "";
                                    }
                                    else
                                    {
                                        ent e = await _webApiServicesHelper.EntityGetByEntId(se[0].storage_ent_id.GetValueOrDefault());
                                        CurrentLocation = e.ent_name;
                                        CurrentLocationDescription = e.description;

                                    }

                                    ResultError = false;
                                    Result = Settings.LocationMoveName + " '" + LocationName + "' Found";

                                    //reset the destiantion to move to and select the field
                                    resetMoveToLocation();
                                }
                                else
                                {
                                    ResultError = true;
                                    Result = "'" + LocationName + "' Not a Movable " + Settings.LocationMoveName;

                                    //reset related values
                                    resetLocation();
                                }
                            }
                            else
                            {
                                ResultError = true;
                                Result = "'" + LocationName + "' Not a Valid " + Settings.LocationMoveName;

                                //reset related values
                                resetLocation();
                            }

                        }
                        else
                        {
                            ResultError = true;
                            Result = Settings.LocationMoveName + " '" + LocationName + "' Not Found";

                            //reset related values
                            resetLocation();

                        }

                        //Hide Busy
                        msg.visible = false;
                        MessagingCenter.Send(msg, "BusyPopup");
                    }
                    else
                    {
                        ResultError = true;
                        Result = "Blank " + Settings.LocationMoveName + " Name";                        
                    }
                });
            }

        }

        void resetLocation()
        {
            //reset related values
            LocationName = "";
            LocationDescription = "";
            CurrentLocation = "";
            CurrentLocationDescription = "";
            ent = null;

            MessagingCenter.Send("Show", "LocationMovePage-SetLocationNameFocus");
        }

        //Command for Move To Location Barcode Enter key
        public ICommand ExecuteMoveToLocationBarcode
        {
            get
            {
                return new Command(async () =>
                {
                    //reset values
                    moveToEnt = null;
                    MoveToLocationDescription = "";

                    if (!String.IsNullOrEmpty(MoveToLocationName))
                    {
                        //Show Busy
                        BusyMessage msg = new BusyMessage(true, "Find location");
                        MessagingCenter.Send(msg, "BusyPopup");

                        moveToEnt = await _webApiServicesHelper.EntityGetByEntName(MoveToLocationName);

                        //entity found?
                        if (moveToEnt != null)
                        {                            
                            MoveToLocationDescription = this.moveToEnt.description;
                            
                            ResultError = false;
                            Result = "Location '" + MoveToLocationName + "' Found";
                            
                            //Move Automatically if MultiMove is Enabled
                            if (AutoMoveEnabled)
                            {
                                await MoveLocationWrapper();
                            }

                        }
                        else
                        {
                            ResultError = true;
                            Result = "Location '" + MoveToLocationName + "' Not Found";

                            //reset related values
                            resetMoveToLocation();

                        }

                        //Hide Busy
                        msg.visible = false;
                        MessagingCenter.Send(msg, "BusyPopup");
                    }
                    else
                    {
                        ResultError = true;
                        Result = "Blank Location Name";
                    }
                });
            }

        }

        void resetMoveToLocation()
        {
            //reset related values
            MoveToLocationName = "";
            MoveToLocationDescription = "";
            moveToEnt = null;

            MessagingCenter.Send("Show", "LocationMovePage-SetMoveToLocationNameFocus");
        }    

        //Call Move Inventory
        public ICommand ProcessMoveCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await MoveLocationWrapper();
                });
            }
        }

        async Task MoveLocationWrapper()
        {
            //check that ent and destiantion are populated
            if (ent != null && moveToEnt != null)
            {
                //move the location then reset both fields leaving the Move To Location Field selected
                await MoveLocation();
                resetMoveToLocation();
                resetLocation();

            }
            else if (ent == null && moveToEnt == null)
            {
                ResultError = true;
                Result = "Nothing selected";
            }
            else if (ent == null)
            {
                ResultError = true;
                Result = "No " + Settings.LocationMoveName + " to move selected";
            }
            else if (moveToEnt == null)
            {
                ResultError = true;
                Result = "No Location to move to selected";
            }
        }

        async Task MoveLocation()
        {
            //Show Busy
            BusyMessage msg = new BusyMessage(true, "Moving " + Settings.LocationMoveName);
            MessagingCenter.Send(msg, "BusyPopup");

            //setup path
            string path;
            path = $"/api/inventory/Update?" +
                $"entId=" + HttpUtility.UrlEncode(ent.ent_id.ToString(), Encoding.UTF8) +
                $"&storageEntId=" + HttpUtility.UrlEncode(moveToEnt.ent_id.ToString(), Encoding.UTF8);

            //api call
            object response = await _webApiServices.WebAPICallAsyncRest(RestSharp.Method.PATCH, path);

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                MessagingCenter.Send((PopupMessage)response, "PopupError");
            }
            //check if response was ok
            else
            {
                var resp = (IRestResponse)response;
                if (resp?.StatusCode == HttpStatusCode.OK)
                {
                    ResultError = false;
                    Result = Settings.LocationMoveName + " '" + ent.ent_name + "' moved to '" + moveToEnt.ent_name + "'";
                }
                else
                {
                    ResultError = true;
                    Result = "Failed to move " + Settings.LocationMoveName + " '" + ent.ent_name + "' to '" + moveToEnt.ent_name + "'"; 

                    //Message result back to App.xaml
                    PopupMessage msg2 = new PopupMessage(Settings.LocationMoveName + " Move Failed", Settings.LocationMoveName + " Move", resp.Content, "Ok");
                    MessagingCenter.Send(msg2, "PopupError");
                }
            }

            //Hide Busy
            msg.visible = false;
            MessagingCenter.Send(msg, "BusyPopup");
        }
        
        //Cancel Current Location
        public ICommand CancelLocation
        {
            get
            {
                return new Command(() =>
                {
                    //reset related values
                    resetLocation();
                });
            }

        }

        //Cancel Current Move To Location
        public ICommand CancelMoveToLocation
        {
            get
            {
                return new Command(() =>
                {
                    //reset related values
                    resetMoveToLocation();
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
