using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.CustomControls;
using KG.Mobile.Helpers;
using KG_Data_Access;
using KG.Mobile.Models;
using KG.Mobile.Models.CMMES_GraphQL_Models;
using KG.Mobile.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using StrawberryShake;

namespace KG.Mobile.ViewModels._01_Inventory
{
    class InventoryMoveViewModel : INotifyPropertyChanged
    {
        private GraphQLApiServices _graphQLApiServices = new GraphQLApiServices();
        private GraphQLApiServicesHelper _graphQLApiServicesHelper = new GraphQLApiServicesHelper();
        private readonly SoundHelper _soundHelper;
        private MobileDatabase database = MobileDatabase.Instance;

        //current item inventory and related data
        private Lot_CMMES lot { get; set; }
        private Product_CMMES product { get; set; }
        private Location_CMMES location { get; set; }
        private Location_CMMES toLocation { get; set; }

        #region Constructor
        public InventoryMoveViewModel(SoundHelper soundHelper)
        {
            _soundHelper = soundHelper;
        }
        #endregion

        #region XAML Bound Tags
        //Item Barcode Tags
        private string _ItemBarcode;
        public string ItemBarcode {
            get { return _ItemBarcode; }
            set
            {
                _ItemBarcode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemBarcode"));

                ProductID = "";
                ProductDescription = "";
                LocationName = "";
                LocationDescription = "";
                //item_inv = null;
                lot = null; 
                product = null;
                location = null;
            }
        }

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
                        database.LogAdd(DateTime.Now, "Error", "Inventory Move", value);
                    }
                    else
                    {
                        ResultErrorVisible = false;
                        ResultMessageVisible = true;

                        //save to log
                        database.LogAdd(DateTime.Now, "Info", "Inventory Move", value);
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


        //ItemID Tags
        private string _ProductID;
        public string ProductID
        {
            get
            {
                return _ProductID;
            }
            set
            {
                _ProductID = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemID"));
            }
        }

        //ItemDescription Tags
        private string _ProductDescription;
        public string ProductDescription
        {
            get
            {
                return _ProductDescription;
            }
            set
            {
                _ProductDescription = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemDescription"));
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
        
        //MoveToLocationDescription Tags
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
        
        //MultiMoveEnableToggle Tags
        private bool _MultiMoveEnabled;
        public bool MultiMoveEnabled
        {
            get
            {
                return _MultiMoveEnabled;
            }
            set
            {
                _MultiMoveEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MultiMoveEnabled"));
            }
        }

        #endregion

        #region WebAPI Calls and Commands

        //Command for ItemBarcode Enter key
        public ICommand ExecuteItemBarcode
        {
            get
            {
                return new Command(async (Object obj) =>
                {
                    await LoadInventoryData();

                    //select the ItemBarcode Field
                    if (Settings.AutoSelectEntryField)
                    {
                        var entry = (CustomEntry)obj;
                        entry.Focus();
                    }
                });
            }

        }

        //Command for Location Barcode Enter key
        public ICommand ExecuteLocationBarcode
        {
            get
            {
                return new Command(async (Object obj) =>
                {
                    //reset values
                    toLocation = null;
                    MoveToLocationDescription = "";

                    if (!String.IsNullOrEmpty(MoveToLocationName))
                    {
                        //Show Busy
                        WeakReferenceMessenger.Default.Send(new BusyMessage(true, "Find location"));

                        toLocation = await _graphQLApiServicesHelper.LocationGetByLocationName(MoveToLocationName);

                        //entity found?
                        if (toLocation != null)
                        {
                            MoveToLocationDescription = this.toLocation.Description;
                            ResultError = false;
                            Result = "";

                            if (Settings.InventoryAutoMoveOnByDefault)
                            {
                                MultiMoveEnabled = true;
                            }

                            //select the ItemBarcode Field
                            if (Settings.AutoSelectEntryField)
                            {
                                var entry = (CustomEntry)obj;
                                entry.Focus();
                            }

                            ResultError = false;
                            Result = "Location '" + MoveToLocationName + "' Found";
                        }
                        else
                        {
                            ResultError = true;
                            Result = "Location '" + MoveToLocationName + "' Not Found";

                            //reset related values
                            MoveToLocationName = "";
                            MoveToLocationDescription = "";
                            toLocation = null;

                            WeakReferenceMessenger.Default.Send(new FocusRequestMessage(FocusTarget.MoveToLocationName));
                        }
                        //Hide Busy
                        WeakReferenceMessenger.Default.Send(new BusyMessage(false, string.Empty));

                    }
                    else
                    {
                        ResultError = true;
                        Result = "Blank Location Name";                        
                    }
                });
            }

        }

        //Populate Item Inventory Data
        async Task LoadInventoryData()
        {
            ResultErrorVisible = false;

            if (ItemBarcode != "")
            {
                bool existsInABin = false;
                Location_CMMES binDetails = new Location_CMMES();

                //Show Busy
                WeakReferenceMessenger.Default.Send(new BusyMessage(true, "Searching for inventory"));

                //api call
                object response = await _graphQLApiServicesHelper.InventoryGetByLotName(ItemBarcode);

                //api call threw an error
                if (response.GetType() == typeof(PopupMessage))
                {
                    WeakReferenceMessenger.Default.Send((PopupMessage)response, "PopupError");
                }
                //api call responded with deseralised object
                else if (response.GetType() == typeof(List<Lot_CMMES>))
                {

                    List<Lot_CMMES> lot = (List<Lot_CMMES>)response;

                    //if inventory is found, update the gui
                    if (lot.Count != 0)
                    {

                        //handle entites with 0 qty_left follwing a move and clean up on item_inv not completed
                        foreach (Lot_CMMES i in lot)
                        {
                            if (i.Quantity > 0 & i.LocationId != "")
                            {                                
                                //check if this exists in a storage bin, if enabled
                                if (Settings.CheckBinsforInventoryBeforeMoving && await InventoryBinCheck(i.LocationId))
                                {
                                    existsInABin = true;

                                    //get details for where it is stored
                                    binDetails = await _graphQLApiServicesHelper.LocationGetByLocationId(i.LocationId);

                                    break;
                                }
                                    
                                this.lot = i;
                            }
                        }

                        //Popup Error to user on inventory existing in a bin
                        if (existsInABin)
                        {
                            //Message result back to App.xaml
                            PopupMessage msg2 = new PopupMessage("Inventory Error", "Inventory Move", "Piece of Inventory found in Storage Bin for Item Barcode = " + ItemBarcode 
                                + ". Bin Name = " + binDetails.Name
                                + ". Bin Description = " + binDetails.Description
                                + ". This piece of Inventory cannot be tracked."
                                , "Ok");
                            WeakReferenceMessenger.Default.Send(msg2, "PopupError");

                            ItemBarcode = "";
                        }
                        else
                        {
                            ProductID = this.lot.ProductId;

                            //update the other UI Values
                            await LoadProductData(this.lot.ProductId);
                            await LoadLocationData(this.lot.LocationId);
                        }
                    }
                    else //no inventory, does the lot exist but no inventory?
                    {
                        await LoadLotData();
                    }
                }

                //Hide Busy
                WeakReferenceMessenger.Default.Send(new BusyMessage(false, string.Empty));

                //Auto move the item if enabled
                if (MultiMoveEnabled && !existsInABin)
                {
                    ResultError = false;
                    Result = "";

                    ////determine if we create or move inventory or specific error to user
                    //if (toLocation != null && lot.LocationId == null && lot != null)
                    //{
                    //    await CreateInventory();
                    //    ItemBarcode = "";
                    //}
                    //else 
                    if(toLocation != null && lot != null)
                    {
                        await MoveInventory();
                        ItemBarcode = "";
                    }
                    else if (toLocation == null)
                    {
                        ResultError = true;
                        Result = "Not a valid destination to auto move";                        
                    }
                    else if (lot == null)
                    {
                        ResultError = true;
                        Result = "Not a valid piece of inventory to auto move";
                        ItemBarcode = "";
                    }

                }

            }
        }

        //Populate Item Description based on ItemId
        async Task LoadProductData(string productId)
        {

            //api call
            object response = await _graphQLApiServicesHelper.ProductGetByProductId(productId);

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                WeakReferenceMessenger.Default.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<Product_CMMES>))
            {

                List<Product_CMMES> product = (List<Product_CMMES>)response;

                //if item is found, update the gui
                if (product.Count != 0)
                {
                    this.product = product[0];

                    ProductDescription = this.product.Description;
                    Result = "";
                }
                else
                {
                    ResultError = true;
                    Result = "No Item Data Found";
                }
            }
        }

        //Populate LocationName and LocationDescription based on Ent_Id
        async Task LoadLocationData(string locationId)
        {

            //api call
            object response = await _graphQLApiServicesHelper.LocationGetByLocationId(locationId);

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                WeakReferenceMessenger.Default.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<Location_CMMES>))
            {

                List<Location_CMMES> location= (List<Location_CMMES>)response;

                //iflocationis found, update the gui
                if (location.Count != 0)
                {
                    this.location = location[0];

                    LocationName = this.location.Name;
                    LocationDescription = this.location.Description;
                }
            }         
        }

        //Populate Item Data from the Lot Definition when no Inventory Exists
        async Task LoadLotData()
        {
            //api call
            object response = await _graphQLApiServicesHelper.InventoryGetByLotName(ItemBarcode);

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                WeakReferenceMessenger.Default.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<Lot_CMMES>))
            {

                List<Lot_CMMES> lot = (List<Lot_CMMES>)response;

                //if lot is found, update the gui
                if (lot.Count != 0)
                { 
                    this.lot = lot[0];
                    ProductID = this.lot.ProductId;

                    //update the other UI Values
                    await LoadProductData(this.lot.ProductId);
                }
                else //no lot definition
                {
                    ResultError = true;
                    Result = "Lot Definition for '" + ItemBarcode + "' does not exist";
                    ItemBarcode = "";
                }
            }
        }

        //Call Move Inventory
        public ICommand ProcessMoveCommand
        {
            get
            {
                return new Command(async (Object obj) =>
                {
                    if(toLocation != null && lot != null)
                    {
                        await MoveInventory();
                        ItemBarcode = "";

                        //select the ItemBarcode Field
                        if (Settings.AutoSelectEntryField)
                        {
                            var entry = (CustomEntry)obj;
                            entry.Focus();
                        }

                    }
                    //else if(toLocation != null && lot != null)
                    //{
                    //    await CreateInventory();
                    //    ItemBarcode = "";

                    //    //select the ItemBarcode Field
                    //    if (Settings.AutoSelectEntryField)
                    //    {
                    //        var entry = (CustomEntry)obj;
                    //        entry.Focus();
                    //    }

                    //}
                    
                });
            }

        }

        // Move Inventory API Call
        async Task MoveInventory()
        {
            var response = await _graphQLApiServicesHelper.MoveInventorytoLocationId(lot.LotId, lot.Quantity, lot.UnitOfMeasureId, toLocation.LocationId);

            // Handle errors
            if (response is null)
            {
                return;
            }

            // Handle success
            if (response is List<Lot_CMMES> movedLots && movedLots.Count > 0)
            {
                ResultError = false;
                Result = $"{ItemBarcode} moved to {toLocation.Description}";
            }
        }

        // Dont believe the below is needed anymore.
        ////Create Inventory API Call
        //async Task CreateInventory()
        //{
        //    //Show Busy
        //    BusyMessage msg = new BusyMessage(true, "Creating inventory");
        //    MessagingCenter.Send(msg, "BusyPopup");

        //    //setup path
        //    string path;
        //    path = $"/api/inventory/AddInventory?" +
        //        $"lotNo=" + HttpUtility.UrlEncode(lot.lot_no.ToString(), Encoding.UTF8) +
        //        $"&entId=" + HttpUtility.UrlEncode(toLocation.ent_id.ToString(), Encoding.UTF8) +
        //        $"&addQty=" + "1" +
        //        $"&statusCd=" + HttpUtility.UrlEncode(lot.status_cd.ToString(), Encoding.UTF8) +
        //        $"&gradeCd=" + HttpUtility.UrlEncode(lot.grade_cd.ToString(), Encoding.UTF8) +
        //        $"&itemId=" + HttpUtility.UrlEncode(lot.item_id.ToString(), Encoding.UTF8) +
        //        $"&transferComments=" + "Created_By_SBMOM.Mobile" +
        //        $"&uomId=" + HttpUtility.UrlEncode(item.uom_id.ToString(), Encoding.UTF8);

        //    //api call
        //    object response = await _graphQLApiServices.WebAPICallAsyncRest(RestSharp.Method.PUT, path);

        //    //api call threw an error
        //    if (response.GetType() == typeof(PopupMessage))
        //    {
        //        MessagingCenter.Send((PopupMessage)response, "PopupError");
        //    }
        //    //check if response was ok
        //    else
        //    {
        //        var resp = (IRestResponse)response;
        //        if (resp?.StatusCode == HttpStatusCode.Created)
        //        {
        //            ResultError = false;
        //            Result = ItemBarcode + " created at " + toLocation.Description;
        //        }
        //        else
        //        {
        //            ResultError = true;
        //            Result = "Failed";

        //            //Message result back to App.xaml
        //            PopupMessage msg2 = new PopupMessage("Inventory Move Failed", "Inventory Move", resp.Content, "Ok");
        //            MessagingCenter.Send(msg2, "PopupError");
        //        }
        //    }

        //    //Hide Busy
        //    msg.visible = false;
        //    MessagingCenter.Send(msg, "BusyPopup");

        //}

        //Check if entId is Type == "bin" in storage exec
        async Task<bool> InventoryBinCheck(string locationId)
        {
            //api call
            object response = await _graphQLApiServicesHelper.StorageExecByLocationId(locationId);

            //api call threw an error
            if (response.GetType() == typeof(PopupMessage))
            {
                WeakReferenceMessenger.Default.Send((PopupMessage)response, "PopupError");
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(List<Storage_Exec_CMMES>))
            {

                List<Storage_Exec_CMMES> storage_exec = (List<Storage_Exec_CMMES>)response;

                //if storage_exec row is found
                if (storage_exec.Count != 0)
                {
                    //look for type == "bin"
                    if (storage_exec[0].Type[0].Value == "bin")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else //no storage exec definition, assume not a bin
                {
                    return false;
                }
            }

            return false; //default

        }


        //Cancel Current Location
        public ICommand CancelLocation
        {
            get
            {
                return new Command((Object obj) =>
                {
                    //reset related values
                    MoveToLocationName = "";
                    MoveToLocationDescription = "";
                    toLocation = null;

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
