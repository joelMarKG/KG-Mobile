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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;

namespace KG.Mobile.ViewModels._01_Inventory
{
    public class LocationMoveViewModel : INotifyPropertyChanged
    {
        private GraphQLApiServices _graphQLApiServices = new GraphQLApiServices();
        private GraphQLApiServicesHelper _graphQLApiServicesHelper = new GraphQLApiServicesHelper();
        private MobileDatabase database = MobileDatabase.Instance;
        private readonly SoundHelper _soundHelper;

        //current item inventory and related data
        private Location_CMMES location { get; set; }
        private LocationStorage_CMMES locationStorage { get; set; }
        private Location_CMMES moveToLocation { get; set; }

        public event Action? RequestLocationFocus;
        public event Action? RequestMoveToLocationFocus;

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
            get => _Result;
            private set
            {
                _Result = value;
                OnPropertyChanged();

                ResultErrorVisible = !string.IsNullOrEmpty(value) && ResultError;
                ResultMessageVisible = !string.IsNullOrEmpty(value) && !ResultError;
            }
        }
        async Task SetResultAsync(
            string message,
            bool isError,
            string component
        )
        {
            ResultError = isError;
            Result = message;

            await database.LogAdd(
                DateTime.Now,
                isError ? "Error" : "Info",
                component,
                message
            );
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
                        WeakReferenceMessenger.Default.Send(new BusyMessage(true, "Find " + Settings.LocationMoveName));

                        location = await _graphQLApiServicesHelper.LocationGetByLocationName(LocationName);

                        //entity found?
                        if (location != null)
                        {
                            //is it a moveable location?
                            StorageExec_CMMES se;
                            se = await _graphQLApiServicesHelper.StorageExecByLocationId(location.LocationId);


                            if (se != null)
                            {
                                if (se.Type[0].Value == "Trolley Work Unit" && se.Movable[0].Value == "1")
                                {
                                    LocationDescription = this.location.Description;
                                    locationStorage = await _graphQLApiServicesHelper.LocationStorageByLocationId(location.LocationId);

                                    if (locationStorage != null)
                                    {
                                        //get the current location
                                        if (locationStorage.LocationId_StoredIn == null)
                                        {
                                            CurrentLocation = "";
                                            CurrentLocationDescription = "";
                                        }
                                        else
                                        {
                                            Location_CMMES e = await _graphQLApiServicesHelper.LocationGetByLocationId(locationStorage.LocationId_StoredIn);
                                            CurrentLocation = e.Name;
                                            CurrentLocationDescription = e.Description;

                                        }
                                    }

                                    await SetResultAsync(
                                        $"{Settings.LocationMoveName} '{LocationName}' Found",
                                        isError: false,
                                        component: $"{Settings.LocationMoveName} Move"
                                    );

                                    //reset the destiantion to move to and select the field
                                    resetMoveToLocation();
                                    RequestMoveToLocationFocus?.Invoke();
                                }
                                else
                                {
                                    await SetResultAsync(
                                        $"'{LocationName}' Not a Valid  {Settings.LocationMoveName}",
                                        isError: true,
                                        component: $"{Settings.LocationMoveName} Move"
                                    );
                                    //reset related values
                                    resetLocation();
                                    RequestLocationFocus?.Invoke();
                                }
                            }
                            else
                            {
                                await SetResultAsync(
                                        $"Couldn't Find Storage Details for '{LocationName}'",
                                        isError: true,
                                        component: $"{Settings.LocationMoveName} Move"
                                );

                                //reset related values
                                resetLocation();
                                RequestLocationFocus?.Invoke();
                            }

                        }
                        else
                        {
                            await SetResultAsync(
                                $"{Settings.LocationMoveName}: '{LocationName}' Not Found",
                                isError: true,
                                component: $"{Settings.LocationMoveName} Move"
                            );

                            //reset related values
                            resetLocation();
                            RequestLocationFocus?.Invoke();
                        }

                        //Hide Busy
                        WeakReferenceMessenger.Default.Send(new BusyMessage(false, ""));
                    }
                    else
                    {
                        await SetResultAsync(
                            $"Blank {Settings.LocationMoveName} Name",
                            isError: true,
                            component: $"{Settings.LocationMoveName} Move"
                        );
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
                        WeakReferenceMessenger.Default.Send( new BusyMessage(true, "Find location"));


                        moveToLocation = await _graphQLApiServicesHelper.LocationGetByLocationName(MoveToLocationName);

                        //entity found?
                        if (moveToLocation != null)
                        {
                            // Verify Valid storage location
                            StorageExec_CMMES se;
                            se = await _graphQLApiServicesHelper.StorageExecByLocationId(moveToLocation.LocationId);

                            if (se != null)
                            {
                                if (se.Type[0].Value != "Trolley Work Unit" && se.Movable[0].Value == "0" && se.CanStore[0].Value == "1")
                                {

                                    //Move Automatically if MultiMove is Enabled
                                    if (AutoMoveEnabled)
                                    {
                                        WeakReferenceMessenger.Default.Send(new BusyMessage(false, ""));
                                        await MoveLocationWrapper();
                                    }
                                    else
                                    {
                                        MoveToLocationDescription = this.moveToLocation.Description;

                                        await SetResultAsync(
                                            $"Move To Location ' {MoveToLocationName} ' Found",
                                            isError: false,
                                            component: $"{Settings.LocationMoveName} Move"
                                        );
                                    }
                                }
                                else
                                {
                                    await SetResultAsync(
                                        $"Move To Location '{MoveToLocationName}' is a {Settings.LocationMoveName}",
                                        isError: true,
                                        component: $"{Settings.LocationMoveName} Move"
                                    );
                                    //reset related values
                                    resetMoveToLocation();
                                    RequestMoveToLocationFocus?.Invoke();
                                }
                            }
                            else
                            {
                                await SetResultAsync(
                                        $"Couldn't Find Storage Details for Move To Location '{MoveToLocationName}'",
                                        isError: true,
                                        component: $"{Settings.LocationMoveName} Move"
                                );

                                //reset related values
                                resetMoveToLocation();
                                RequestMoveToLocationFocus?.Invoke();
                            }
                        }
                        else
                        {
                            await SetResultAsync(
                                $"Location ' {MoveToLocationName} ' Not Found",
                                isError: true,
                                component: $"{Settings.LocationMoveName} Move"
                            );

                            //reset related values
                            resetMoveToLocation();
                            RequestMoveToLocationFocus?.Invoke();
                        }

                        //Hide Busy
                        WeakReferenceMessenger.Default.Send(new BusyMessage(false, ""));
                    }
                    else
                    {
                        await SetResultAsync(
                            $"Blank Location Name",
                            isError: true,
                            component: $"{Settings.LocationMoveName} Move"
                        );
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
            //check that ent and destination are populated
            if (location != null && moveToLocation != null)
            {
                if (locationStorage.LocationId_StoredIn == moveToLocation.LocationId)
                {
                    ResultError = false;
                    Result = Settings.LocationMoveName + " Already at Selected Location";
                    await SetResultAsync(
                        $"{Settings.LocationMoveName}: {moveToLocation.Name} Already at {moveToLocation.Name}",
                        isError: false,
                        component: $"{Settings.LocationMoveName} Move"
                    );
                    resetMoveToLocation();
                    resetLocation();
                    RequestLocationFocus?.Invoke();
                }
                else
                {
                    //move the location then reset both fields leaving the Move To Location Field selected
                    if (locationStorage == null) // Add 
                    {
                        await InventoryLocationAdd();
                    }
                    else // Move
                    {
                        await InventoryLocationMove();
                    }
                    resetMoveToLocation();
                    resetLocation();
                    RequestLocationFocus?.Invoke();
                }
            }
            else if (location == null && moveToLocation == null)
            {
                await SetResultAsync(
                    $"Nothing selected",
                    isError: true,
                    component: $"{Settings.LocationMoveName} Move"
                );
            }
            else if (location == null)
            {
                await SetResultAsync(
                    $"No  {Settings.LocationMoveName} to move selected",
                    isError: true,
                    component: $"{Settings.LocationMoveName} Move"
                );
            }
            else if (moveToLocation == null)
            {
                await SetResultAsync(
                    $"No Location to move to selected",
                    isError: true,
                    component: $"{Settings.LocationMoveName} Move"
                );
            }
        }

        async Task InventoryLocationMove()
        {
            //Show Busy
            WeakReferenceMessenger.Default.Send(new BusyMessage(true, "Moving " + Settings.LocationMoveName));
            try
            {
                // GraphQL mutation to move inventory
                string mutation = @"
                mutation LocationMove($locationId: ID!, $locationId_StoredIn: ID!) {
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
                var response = await _graphQLApiServices.ExecuteAsync<LocationMoveResponse>(mutation, variables);

                // Handle errors
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return;
                }
                //check if response was ok
                if (response is LocationMoveResponse movedLocation && movedLocation.inventoryLocationMove.Count > 0)
                {
                    await SetResultAsync(
                        $"{Settings.LocationMoveName} '{location.Name}' moved to '{moveToLocation.Name}'",
                        isError: false,
                        component: $"{Settings.LocationMoveName} Move"
                    ); 
                }
                else
                {
                    await SetResultAsync(
                        $"Failed to move {Settings.LocationMoveName} '{location.Name}' to '{moveToLocation.Name}'",
                        isError: true,
                        component: $"{Settings.LocationMoveName} Move"
                    );

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
                WeakReferenceMessenger.Default.Send(new BusyMessage(false, ""));
            }
        }

        async Task InventoryLocationAdd()
        {
            //Show Busy
            WeakReferenceMessenger.Default.Send(new BusyMessage(true, "Moving " + Settings.LocationMoveName));
            try
            {
                // GraphQL mutation to move inventory
                string mutation = @"
                mutation InventoryLocationAdd($locationId: ID!, $locationId_StoredIn: ID!) {
                    inventoryLocationAdd(
                        inventoryLocationAdd: { locationId: $locationId, locationId_StoredIn: $locationId_StoredIn }
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
                var response = await _graphQLApiServices.ExecuteAsync<LocationMoveResponse>(mutation, variables);

                // Handle errors
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return;
                }
                //check if response was ok
                if (response is LocationMoveResponse movedLocation && movedLocation.inventoryLocationMove.Count > 0)
                {
                    await SetResultAsync(
                        $"{Settings.LocationMoveName} '{location.Name}' moved to '{moveToLocation.Name}'",
                        isError: false,
                        component: $"{Settings.LocationMoveName} Move"
                    );
                }
                else
                {
                    await SetResultAsync(
                        $"Failed to move {Settings.LocationMoveName} '{location.Name}' to '{moveToLocation.Name}'",
                        isError: true,
                        component: $"{Settings.LocationMoveName} Move"
                    );

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
                WeakReferenceMessenger.Default.Send(new BusyMessage(false, ""));
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
                    RequestLocationFocus?.Invoke();
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
                    RequestMoveToLocationFocus?.Invoke();
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
