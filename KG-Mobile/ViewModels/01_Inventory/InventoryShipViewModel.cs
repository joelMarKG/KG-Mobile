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
    class InventoryShipViewModel : INotifyPropertyChanged
    {
        private GraphQLApiServices _graphQLApiServices = new GraphQLApiServices();
        private GraphQLApiServicesHelper _graphQLApiServicesHelper = new GraphQLApiServicesHelper();
        private readonly SoundHelper _soundHelper;
        private MobileDatabase database = MobileDatabase.Instance;

        //current item inventory and related data
        private Location_CMMES location { get; set; }

        #region Constructor
        public InventoryShipViewModel(SoundHelper soundHelper)
        {
            _soundHelper = soundHelper;
        }
        #endregion

        #region DataGrid Handling
        //Item Inventory Tags
        private List<Lot_CMMES> _item_inv { get; set; }
        public List<Lot_CMMES> item_inv
        {
            get
            {
                return _item_inv;
            }
            set
            {
                _item_inv = value;

                //update the count message
                if (value != null)
                {
                    item_inv_count_message = "Inventory Count: " + value.Count;
                }
                else
                {
                    item_inv_count_message = "Inventory Count: 0";
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("item_inv"));
            }
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
                        database.LogAdd(DateTime.Now, "Error", "Inventory Ship", value);
                    }
                    else
                    {
                        ResultErrorVisible = false;
                        ResultMessageVisible = true;

                        //save to log
                        database.LogAdd(DateTime.Now, "Info", "Inventory Ship", value);
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

        //item_inv_count Tags
        private string _item_inv_count_message;
        public string item_inv_count_message
        {
            get
            {
                return _item_inv_count_message;
            }
            set
            {
                _item_inv_count_message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("item_inv_count_message"));
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
                        BusyMessage msg = new BusyMessage(true, "Find location");
                        MessagingCenter.Send(msg, "BusyPopup");

                        location = await _graphQLApiServicesHelper.LocationGetByLocationName(LocationName);

                        //entity found?
                        if (location != null)
                        {
                            LocationDescription = this.location.Description;

                            ResultError = false;
                            Result = "Location '" + LocationName + "' Found";                            

                            //query all inventory for the location
                            item_inv = await _graphQLApiServicesHelper.InventoryGetByLocationId(location.LocationId);
                        }
                        else
                        {
                            ResultError = true;
                            Result = "Location '" + LocationName + "' Not Found";

                            //reset related values
                            LocationName = "";
                            LocationDescription = "";
                            location = null;

                            MessagingCenter.Send("Show", "InventoryShipPage-SetMoveToLocationNameFocus");

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

        //Call Move Inventory
        public ICommand ProcessShipCommand
        {
            get
            {
                return new Command(async (Object obj) =>
                {
                    //check for no inventory or entity
                    if (location == null)
                    {
                        ResultError = true;
                        Result = "No Entity selected";                        
                    }
                    else if (item_inv?.Count == 0)
                    {
                        ResultError = true;
                        Result = "No Inventory in Location selected";                       
                    }
                    else
                    {
                        //Yes No Popup
                        bool response = await App.Current.MainPage.DisplayAlert("Ship Inventory", "Are you sure you want to ship all inventory in " + location.Name + "?", "Ok", "Cancel");

                        //If user responds yes, ship inventory
                        if (response)
                        {
                            await InventoryShip();

                            //set location name focus
                            if (Settings.AutoSelectEntryField)
                            {
                                var entry = (CustomEntry)obj;
                                entry.Focus();
                            }
                        }
                    }
                });
            }
        }

        //Ship inventory in ent
        async Task InventoryShip()
        {
            // Show Busy
            WeakReferenceMessenger.Default.Send(new BusyMessage(true, "Shipping Inventory"));

            // Build list of lots to ship from inventory
            var inventoryShipPayload = item_inv
            .Select(lot => new InventoryShipInput_CMMES { LotId = lot.LotId })
            .ToList();

            try
            {
                // GraphQL mutation to move inventory
                string mutation = @"
                    mutation ShipInventory($inventoryShip: [inventoryShip!]) {
                        inventoryShip(inventoryShip: $inventoryShip) {
                            lotId
                            productId
                            lotParentId
                            name
                            description
                            lotStateId
                            lotStatusId
                            gradeReasonId
                            purchaseOrderId
                            quantity
                            unitOfMeasureId
                            locationId
                            data
                            dateCreated
                            userCreated
                            dateUpdated
                            userUpdated
                            lotHistoryId
                        }
                    }
                ";

                // Prepare variables
                var variables = new
                {
                    inventoryShip = inventoryShipPayload
                };

                // Call GraphQL executor
                var response = await _graphQLApiServices.ExecuteAsync<List<Lot_CMMES>>(mutation, variables);

                // Handle errors
                if (response is PopupMessage popup)
                {
                    WeakReferenceMessenger.Default.Send(new PopupErrorMessage(popup));
                    return;
                }

                // Handle success
                if (response is List<Lot_CMMES> movedLots && movedLots.Count > 0)
                {
                    ResultError = false;
                    Result = "All Inventory Shipped from '" + location.Name + "'";

                    LocationName = "";
                    LocationDescription = "";
                    location = null;
                    item_inv = null;
                }
                else
                {
                    // If response is empty, treat as failure
                    ResultError = true;
                    Result = "Failed to ship from '" + location.LocationId + "'";
                    var popup2 = new PopupMessage("Inventory Ship Failed", "Inventory Ship", "No response from server", "Ok");
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
                return new Command((Object obj) =>
                {
                    //reset related values
                    LocationName = "";
                    LocationDescription = "";
                    location = null;

                    //set location name focus
                    if (Settings.AutoSelectEntryField)
                    {
                        var entry = (CustomEntry)obj;
                        entry.Focus();
                    }
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
