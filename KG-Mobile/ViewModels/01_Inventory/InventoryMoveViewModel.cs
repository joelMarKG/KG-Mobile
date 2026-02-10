using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.CustomControls;
using KG.Mobile.Helpers;
using KG.Mobile.Models;
using KG.Mobile.Models.CMMES_GraphQL_Models;
using KG.Mobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Diagnostics;

namespace KG.Mobile.ViewModels._01_Inventory
{
    public class InventoryMoveViewModel : INotifyPropertyChanged
    {
        private readonly GraphQLApiServicesHelper _graphQLApiServicesHelper;
        private readonly SoundHelper _soundHelper;

        //current lot inventory and related data
        private Lot_CMMES lot { get; set; }
        private Product_CMMES product { get; set; }
        private Location_CMMES location { get; set; }
        private Location_CMMES toLocation { get; set; }

        public event Action? RequestLotBarcodeFocus;
        public event Action? RequestLocationFocus;

        #region Constructor
        public InventoryMoveViewModel(
            GraphQLApiServicesHelper graphQLApiServicesHelper,
            SoundHelper soundHelper)
        {
            _graphQLApiServicesHelper = graphQLApiServicesHelper;
            _soundHelper = soundHelper;
        }

        #endregion

        #region XAML Bound Tags
        //Lot Barcode Tags
        private string _LotBarcode;
        public string LotBarcode {
            get { return _LotBarcode; }
            set
            {
                _LotBarcode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LotBarcode"));

                ProductID = "";
                ProductName = "";
                ProductDescription = "";
                LocationName = "";
                LocationDescription = "";
                //Lot_inv = null;
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
                        WeakReferenceMessenger.Default.Send(new LogMessageRequest(new LogMessage("Error", "Inventory Move",value)));
                    }
                    else
                    {
                        ResultErrorVisible = false;
                        ResultMessageVisible = true;

                        //save to log
                        WeakReferenceMessenger.Default.Send(new LogMessageRequest(new LogMessage("Info", "Inventory Move", value)));
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


        //ProductID Tags
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProductID"));
            }
        }

        //ProductName Tags
        private string _ProductName;
        public string ProductName
        {
            get
            {
                return _ProductName;
            }
            set
            {
                _ProductName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProductName"));
            }
        }

        //ProductDescription Tags
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProductDescription"));
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

        //Command for LotBarcode Enter key
        public ICommand ExecuteLotBarcode
        {
            get
            {
                return new Command(async (Object obj) =>
                {
                    await LoadInventoryData();

                    //select the LotBarcode Field
                    if (Settings.AutoSelectEntryField)
                    {
                        RequestLotBarcodeFocus?.Invoke();
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

                            // Verify Valid storage location
                            StorageExec_CMMES se;
                            se = await _graphQLApiServicesHelper.StorageExecByLocationId(toLocation.LocationId);

                            if (se != null)
                            {
                                if (se.CanStore[0].Value == "1")
                                {
                                    MoveToLocationDescription = this.toLocation.Description;
                                    ResultError = false;
                                    Result = "";

                                    if (Settings.InventoryAutoMoveOnByDefault)
                                    {
                                        MultiMoveEnabled = true;
                                    }

                                    //select the LotBarcode Field
                                    if (Settings.AutoSelectEntryField)
                                    {
                                        RequestLotBarcodeFocus?.Invoke();
                                    }

                                    ResultError = false;
                                    Result = "Location '" + MoveToLocationName + "' Found";
                                }
                                else
                                {
                                    ResultError = true;
                                    Result = "Location '" + MoveToLocationName + "' Not Configured For Storage";
                                }
                            }
                            else
                            {
                                ResultError = true;
                                Result = "Location '" + MoveToLocationName + "' Storage Exec Details Not Found";
                            }
                        }
                        else
                        {
                            ResultError = true;
                            Result = "Location '" + MoveToLocationName + "' Not Found";

                            //reset related values
                            MoveToLocationName = "";
                            MoveToLocationDescription = "";
                            toLocation = null;

                            RequestLocationFocus?.Invoke();
                            WeakReferenceMessenger.Default.Send(new FocusRequestMessage(FocusTarget.MoveToLocationName));
                        }
                        //Hide Busy
                        WeakReferenceMessenger.Default.Send(new BusyMessage(false, ""));

                    }
                    else
                    {
                        ResultError = true;
                        Result = "Blank Location Name";                        
                    }
                });
            }

        }

        //Populate Lot Inventory Data
        async Task LoadInventoryData()
        {
            if (AppDebug.IsDebug)
            {
                Debug.WriteLine("LoadInventoryData");
            }

            ResultErrorVisible = false;

            if (LotBarcode != "")
            {
                bool existsInABin = false;
                Location_CMMES binDetails = new Location_CMMES();

                //Show Busy
                WeakReferenceMessenger.Default.Send(new BusyMessage(true, "Searching for inventory"));

                //api call
                object response = await _graphQLApiServicesHelper.InventoryGetByLotName(LotBarcode);

                //api call threw an error
                if (response == null)
                {
                    ResultError = true;
                    Result = "Not a valid piece of inventory to move";
                    LotBarcode = "";
                }
                //api call responded with deseralised object
                else if (response.GetType() == typeof(Lot_CMMES))
                {

                    Lot_CMMES lot = (Lot_CMMES)response;

                    if (lot.Quantity > 0 & lot.LocationId != null)
                    {
                        //check if this exists in a storage bin, if enabled
                        if (Settings.CheckBinsforInventoryBeforeMoving && await InventoryBinCheck(lot.LocationId))
                        {
                            existsInABin = true;

                            //get details for where it is stored
                            binDetails = await _graphQLApiServicesHelper.LocationGetByLocationId(lot.LocationId);

                        }

                        this.lot = lot;
                        //Popup Error to user on inventory existing in a bin
                        if (existsInABin)
                        {
                            //Message result back to App.xaml
                            PopupMessage msg2 = new PopupMessage("Inventory Error", "Inventory Move", "Piece of Inventory found in Storage Bin for Lot Barcode = " + LotBarcode
                                + ". Bin Name = " + binDetails.Name
                                + ". Bin Description = " + binDetails.Description
                                + ". This piece of Inventory cannot be moved."
                                , "Ok");
                            WeakReferenceMessenger.Default.Send(new PopupErrorMessage(msg2));

                            LotBarcode = "";
                        }
                        else
                        {
                            ProductID = this.lot.ProductId;

                            //update the other UI Values
                            await LoadProductData(this.lot.ProductId);
                            await LoadLocationData(this.lot.LocationId);

                            ResultError = false;
                            Result = "Found " + LotBarcode + " as Inventory";
                        }
                    }
                    else if (lot.LocationId == null)
                    //Lot Doesn't exist as inventory yet.
                    {
                        this.lot = lot;
                        ProductID = lot.ProductId;
                        await LoadProductData(lot.ProductId);

                        ResultError = false; 
                        Result = "Found " + LotBarcode + ". Not Yet Inventory";
                    }
                }

                //Hide Busy
                WeakReferenceMessenger.Default.Send(new BusyMessage(false, ""));

                //Auto move the Lot if enabled
                if (MultiMoveEnabled && !existsInABin)
                {
                    ResultError = false;
                    Result = "";

                    if(toLocation != null && lot != null)
                    {
                        await MoveInventory();
                        LotBarcode = "";
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
                        LotBarcode = "";
                    }
                }
            }
        }

        //Populate Product Description based on ProductId
        async Task LoadProductData(string productId)
        {
            if (AppDebug.IsDebug)
            {
                Debug.WriteLine("LoadProductData using: " + productId);
            }

            var response = await _graphQLApiServicesHelper.ProductGetByProductId(productId);

            if (response is Product_CMMES prod)
            {
                this.product = prod;
                ProductDescription = prod.Description;
                ProductName = prod.Name;
            }
            else
            {
                // fallback if not found
                this.product = null;
                ProductDescription = "Product not found";
                ProductName = "Product not found";
            }

            Result = "";
        }


        //Populate LocationName and LocationDescription based on Ent_Id
        async Task LoadLocationData(string locationId)
        {
            if (AppDebug.IsDebug)
            {
                Debug.WriteLine("LoadLocationData using: " + locationId);
            }

            var response = await _graphQLApiServicesHelper.LocationGetByLocationId(locationId);

            if (response is Location_CMMES loc)
            {
                this.location = loc;
                LocationName = loc.Name;
                LocationDescription = loc.Description;
            }
            else
            {
                // fallback if not found
                this.location = null;
                LocationName = "Unknown";
                LocationDescription = "";
            }
        }


        ////Populate Product Data from the Lot Definition when no Inventory Exists
        //async Task LoadLotData()
        //{
        //    var response = await _graphQLApiServicesHelper.InventoryGetByLotName(LotBarcode);

        //    if (response is Lot_CMMES lotResp)
        //    {
        //        this.lot = lotResp;
        //        ProductID = lotResp.ProductId;
        //        await LoadProductData(lotResp.ProductId);
        //    }
        //    else
        //    {
        //        this.lot = null;
        //        ResultError = true;
        //        Result = $"Lot Definition for '{LotBarcode}' does not exist";
        //        LotBarcode = "";
        //    }
        //}


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
                        LotBarcode = "";

                        //select the LotBarcode Field
                        if (Settings.AutoSelectEntryField)
                        {
                            RequestLotBarcodeFocus?.Invoke();
                        }

                    }
                                       
                });
            }

        }

        // Move Inventory API Call
        async Task MoveInventory()
        {
            if (AppDebug.IsDebug)
            {
                Debug.WriteLine("MoveInventory");
            }
            if (lot.LocationId != toLocation.LocationId)
            {

                var response = await _graphQLApiServicesHelper.MoveInventorytoLocationId(lot.LotId, lot.Quantity, lot.UnitOfMeasureId, toLocation.LocationId);

                // Handle errors
                if (response is null)
                {
                    return;
                }

                // Handle success
                if (response is Lot_CMMES movedLots)
                {
                    ResultError = false;
                    Result = $"{LotBarcode} moved to {toLocation.Description}";
                }
            }
            else if (lot.LocationId == toLocation.LocationId)
            {
                ResultError = false;
                Result = $"{LotBarcode} already at location {toLocation.Description}";
            }
        }

        //Check if entId is Type == "bin" in storage exec
        async Task<bool> InventoryBinCheck(string locationId)
        {
            if (AppDebug.IsDebug)
            {
                Debug.WriteLine("InventoryBinCheck on: " + locationId);
            }

            //api call
            var response = await _graphQLApiServicesHelper.StorageExecByLocationId(locationId);

            //api call threw an error
            if (response is null)
            {
                return false;
            }
            //api call responded with deseralised object
            else if (response.GetType() == typeof(StorageExec_CMMES))
            {

                StorageExec_CMMES storage_exec = (StorageExec_CMMES)response;

                if (storage_exec != null)
                {

                    //look for type == "bin"
                    if (storage_exec.Type[0].Value == "BIN")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
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
                        RequestLocationFocus?.Invoke();
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
