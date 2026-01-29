using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.CustomControls;
using KG.Mobile.Helpers;
using KG.Mobile.Models;
using KG.Mobile.Models.CMMES_GraphQL_Models;
using KG.Mobile.Services;
using KG_Data_Access;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;

namespace KG.Mobile.ViewModels._01_Inventory
{
    class LocationMoveViewModel : INotifyPropertyChanged
    {
        private GraphQLApiServices _graphQLApiServices = new GraphQLApiServices();
        private GraphQLApiServicesHelper _graphQLApiServicesHelper = new GraphQLApiServicesHelper();
        private MobileDatabase database = MobileDatabase.Instance;
        private readonly SoundHelper _soundHelper;

        //current item inventory and related data
        private Location_CMMES location { get; set; }
        private Location_CMMES moveToLocation { get; set; }

        #region Constructor
        public LocationMoveViewModel(SoundHelper soundHelper)
        {
            //set the default state for the toggle switch in the UI
            AutoMoveEnabled = Settings.LocationAutoMoveOnByDefault;
            _soundHelper = soundHelper;
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
                    _soundHelper.PlayErrorAsync();
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
                    _soundHelper.PlaySuccessAsync();
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
                    location = null;
                    LocationDescription = "";

                    if (!String.IsNullOrEmpty(LocationName))
                    {
                        //Show Busy
                        BusyMessage msg = new BusyMessage(true, "Find " + Settings.LocationMoveName);
                        MessagingCenter.Send(msg, "BusyPopup");

                        location = await _graphQLApiServicesHelper.LocationGetByLocationName(LocationName);

                        //entity found?
                        if (location != null)
                        {
                            //is it a moveable location?
                            Storage_Exec_CMMES se;
                            se = await _graphQLApiServicesHelper.StorageExecByLocationId(location.LocationId);

                            if (se != null)
                            {
                                if (se.Movable[0].Value == "1")
                                {
                                    LocationDescription = this.location.Description;

                                    //get the current location
                                    if (se.LocationStorageDetails[0].LocationId_StoredIn == null)
                                    {
                                        CurrentLocation = "";
                                        CurrentLocationDescription = "";
                                    }
                                    else
                                    {
                                        Location_CMMES e = await _graphQLApiServicesHelper.LocationGetByLocationId(se.LocationStorageDetails[0].LocationId_StoredIn);
                                        CurrentLocation = e.Name;
                                        CurrentLocationDescription = e.Description;

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
            location = null;

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
                    moveToLocation = null;
                    MoveToLocationDescription = "";

                    if (!String.IsNullOrEmpty(MoveToLocationName))
                    {
                        //Show Busy
                        BusyMessage msg = new BusyMessage(true, "Find location");
                        MessagingCenter.Send(msg, "BusyPopup");

                        moveToLocation = await _graphQLApiServicesHelper.LocationGetByLocationName(MoveToLocationName);

                        //entity found?
                        if (moveToLocation != null)
                        {                            
                            MoveToLocationDescription = this.moveToLocation.Description;
                            
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
            moveToLocation = null;

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
            if (location != null && moveToLocation != null)
            {
                //move the location then reset both fields leaving the Move To Location Field selected
                await MoveLocation();
                resetMoveToLocation();
                resetLocation();

            }
            else if (location == null && moveToLocation == null)
            {
                ResultError = true;
                Result = "Nothing selected";
            }
            else if (location == null)
            {
                ResultError = true;
                Result = "No " + Settings.LocationMoveName + " to move selected";
            }
            else if (moveToLocation == null)
            {
                ResultError = true;
                Result = "No Location to move to selected";
            }
        }

        async Task MoveLocation()
        {
            //Show Busy
            WeakReferenceMessenger.Default.Send(new BusyMessage(true, "Shipping Inventory"));
            try
            {
                // GraphQL mutation to move inventory
                string mutation = @"
                    mutation MoveLocation($locationId: ID!, $locationId_StoredIn: ID!) {
                        inventoryLocationMove(
                            inventoryLocationMove: { locationId: $locationId, locationId_StoredIn: $locationId_StoredIn }
                        ) {
                            locationStorageId
                            locationId
                            locationId_StoredIn
                            locationStorageStateId
                            data
                            dateCreated
                            userCreated
                            dateUpdated
                            userUpdated
                        }
                    }
                ";

                // Prepare variables
                var variables = new
                {
                    locationId = location.LocationId,
                    locationId_StoredIn = moveToLocation.LocationId
                };

                //api call
                var response = await _graphQLApiServices.ExecuteAsync<List<LocationStorage_CMMES>>(mutation, variables);

                // Handle errors
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return;
                }
                //check if response was ok
                if (response is List<LocationStorage_CMMES> movedLocation && movedLocation.Count > 0)
                {
                    ResultError = false;
                    Result = Settings.LocationMoveName + " '" + location.Name + "' moved to '" + moveToLocation.Name + "'";
                }
                else
                {
                    ResultError = true;
                    Result = "Failed to move " + Settings.LocationMoveName + " '" + location.Name + "' to '" + moveToLocation.Name + "'";

                    var popup2 = new PopupMessage("Location Move Failed", "Location Move", "No response from server", "Ok");
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup2));
                }
            }
            catch (Exception ex)
            {
                // Catch unexpected exceptions
                var popup = new PopupMessage("GraphQL Exception", "ItemService", ex.Message, "Ok");
                WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
            }
            finally
            {
                // Hide Busy
                WeakReferenceMessenger.Default.Send(new BusyMessage(false, string.Empty));
            }
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
